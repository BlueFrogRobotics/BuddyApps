using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ShoppingList
{
    /// <summary>
    /// Beginning state in shopping list app
    /// </summary>
    public class ParseSentenceState : AStateMachineBehaviour
    {

        private ShoppingListManager mShopManager;
        private float mTimer = 0.0f;
        private bool mHasStartedSTT = false;
        private bool mHasCancelled = false;
        private bool mMustSwitch = false;
        private bool mHasEndedReco = false;

        public override void Start()
        {
            mShopManager = GetComponent<ShoppingListManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.SpeechToText.OnBestRecognition.Add(OnBestReco);
            Interaction.SpeechToText.OnEnd.Add(OnEndReco);
            Interaction.SpeechToText.OnError.Add(OnError);
            Debug.Log("miaou: "+ Interaction.SpeechToText.LastAnswer);
            mShopManager.ProcessCommand(Interaction.SpeechToText.LastAnswer);
            mHasStartedSTT = false;
            mHasCancelled = false;
            mMustSwitch = false;
            mHasEndedReco = false;
            mTimer = 0.0f;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            //if (mHasCancelled || (mHasStartedSTT && mTimer > 5.0f))
            if(mTimer > 7.0f && (mMustSwitch || (mHasStartedSTT && !mMustSwitch) || mHasCancelled)
                && Interaction.SpeechToText.HasFinished) 
            {
                Trigger("ChangeState");
            }

            if (!mHasStartedSTT && !Interaction.TextToSpeech.IsSpeaking)
            {
                Debug.Log("request parse sentence");
                Interaction.SpeechToText.Request();
                mHasStartedSTT = true;
            }
            
            //iAnimator.SetBool("IsHuman", mHumanDetector.IsHumanDetected);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.SpeechToText.OnBestRecognition.Remove(OnBestReco);
            Interaction.SpeechToText.OnEnd.Remove(OnEndReco);
            Interaction.SpeechToText.OnError.Remove(OnError);
            ResetTrigger("ChangeState");
        }
       
        private void OnBestReco(string iText)
        {
            if(iText.ToLower().Contains("non"))
            {
                mShopManager.Cancel();
                mHasCancelled = true;
                mMustSwitch = true;
            }
            else
            {
                mShopManager.ProcessCommand(iText);
                mTimer = 0.0f;
                mHasStartedSTT = false;
                mMustSwitch = false;
            }
            Debug.Log("best reco dans parse sentence");
        }

        /// <summary>
        /// Called when stt end
        /// </summary>
        private void OnEndReco()
        {
            mHasEndedReco = true;
            Debug.Log("end reco dans parse sentence");
        }

        private void OnError(string iText)
        {
            Debug.Log("error reco dans parse sentence: "+iText);
        }
    }
}