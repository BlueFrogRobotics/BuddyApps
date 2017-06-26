using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ShoppingList
{
    public class AskCmdState : AStateMachineBehaviour
    {
        private ShoppingListManager mShopManager;
        private bool mHasAnsweredYes = false;
        private bool mMustQuit = false;
        private bool mSwitchState = false;
        private int mNbSTTCalled = 0;

        public override void Start()
        {
            mShopManager = GetComponent<ShoppingListManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mHasAnsweredYes = false;
            mMustQuit = false;
            mSwitchState = false;
            Interaction.SpeechToText.OnBestRecognition.Add(OnBestReco);
            Interaction.SpeechToText.OnEnd.Add(OnEndReco);
            Interaction.TextToSpeech.Say("Veut tu ajouter ou supprimer des éléments a la liste de courses");
            Debug.Log("miaou: " + Interaction.SpeechToText.LastAnswer);
            mNbSTTCalled = 0;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!Interaction.TextToSpeech.IsSpeaking && Interaction.SpeechToText.HasFinished && !mHasAnsweredYes && !mMustQuit)
            {
                Debug.Log("1er request");
                Interaction.SpeechToText.Request();
            }

            if(!Interaction.TextToSpeech.IsSpeaking && mHasAnsweredYes && Interaction.SpeechToText.HasFinished & !mSwitchState)
            {
                Debug.Log("2eme request");
                Interaction.SpeechToText.Request();
            }
            else if(mSwitchState)
            {
                Trigger("ChangeState");
            }

            else if(mMustQuit || mNbSTTCalled>1)
            {
                Trigger("Quit");
                ResetTrigger("ChangeState");
                mShopManager.QuitApplication();
            }
            //iAnimator.SetBool("IsHuman", mHumanDetector.IsHumanDetected);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.SpeechToText.OnBestRecognition.Remove(OnBestReco);
            Interaction.SpeechToText.OnEnd.Remove(OnEndReco);
            ResetTrigger("ChangeState");
        }

        private void OnBestReco(string iText)
        {
            Debug.Log("recu dans ask cmd state");
            if (!mHasAnsweredYes)
            {
                if (iText.ToLower().Contains("oui"))
                {
                    Debug.Log("c est ok");
                    Interaction.TextToSpeech.Say("Ok");
                    mHasAnsweredYes = true;
                    mMustQuit = false;
                    mNbSTTCalled = 0;
                }
                else
                {
                    mMustQuit = true;
                    Debug.Log("c est bof");
                }
            }

            else
            {
                Debug.Log("on switch");
                mSwitchState = true;
            }
        }

        private void OnEndReco()
        {
            mNbSTTCalled++;
        }
    }
}