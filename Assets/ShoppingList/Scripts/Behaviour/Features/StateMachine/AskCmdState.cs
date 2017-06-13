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
            mSTT.OnBestRecognition.Add(OnBestReco);
            mSTT.OnEnd.Add(OnEndReco);
            mTTS.Say("Veut tu ajouter ou supprimer des éléments a la liste de courses");
            Debug.Log("miaou: " + mSTT.LastAnswer);
            mNbSTTCalled = 0;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!mTTS.IsSpeaking && mSTT.HasFinished && !mHasAnsweredYes && !mMustQuit)
            {
                Debug.Log("1er request");
                mSTT.Request();
            }

            if(!mTTS.IsSpeaking && mHasAnsweredYes && mSTT.HasFinished & !mSwitchState)
            {
                Debug.Log("2eme request");
                mSTT.Request();
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
            mSTT.OnBestRecognition.Remove(OnBestReco);
            mSTT.OnEnd.Remove(OnEndReco);
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
                    mTTS.Say("Ok");
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