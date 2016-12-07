using UnityEngine;
using System.Collections;
using BuddyOS.App;
using UnityEngine.UI;
using System;
namespace BuddyApp.RLGL
{
    public class CountState : AStateMachineBehaviour
    {
        private GameObject mCanvasUIToWin;
        private Button mButtonToWin;
        private float mTimer;
        private bool mFirstSentence;
        private bool mSecondSentence;
        private bool mIsCoroutineDone;
        private bool mIsMovementDone;

        private int mCount;


        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("COUNT STATE : ON ENTER");
            mCanvasUIToWin = GetGameObject(5);
            mIsCoroutineDone = false;
            mIsMovementDone = false;
            mFirstSentence = false;
            mSecondSentence = false;
            mCount = 0;
            iAnimator.SetBool("IsCountDone", false);
            iAnimator.SetBool("IsWon", false);
            mMood.Set(MoodType.HAPPY);
 
            mTTS.Say("Okay let's play together!");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("COUNT STATE : ON UPDATE");
            if (mTTS.HasFinishedTalking() && !mFirstSentence)
            {
                StartCoroutine(WaitTenSecondsAtStart());
            }
            
            if(mTTS.HasFinishedTalking() && mFirstSentence && !mSecondSentence)
            {
                mMood.Set(MoodType.NEUTRAL);
                mCount = 0;
                mTTS.Say("Ok let's go!");
                mSecondSentence = true;
            }

            if(mTTS.HasFinishedTalking() && mSecondSentence)
            {
                StartCoroutine(GreenLightMomentAndTurn());
            }
            
            if (mWheels.Status == MobileBaseStatus.REACHED_GOAL && mIsCoroutineDone)
            {
                mIsMovementDone = true;
            }

            if (mIsMovementDone && mTTS.HasFinishedTalking())
            {
                StartCoroutine(ChangeState(6.0F, iAnimator));
            }

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("COUNT STATE : ON EXIT");
            mCanvasUIToWin.SetActive(false);
            //iAnimator.SetBool("IsCountDone", false);
            //iAnimator.SetBool("IsWon", false);
            //mIsCoroutineDone = false;
            //mIsMovementDone = false;
            //mFirstSentence = false;
            //mSecondSentence = false;
            //mCount = 0;
        }

        private IEnumerator WaitTenSecondsAtStart()
        {
            if(mCount == 0 && mTTS.HasFinishedTalking())
            {
                mTTS.Say("You have ten seconds to go away by about fifteen feet, I will wait ten seconds gogo! ");
                mCount++;
                mYesHinge.SetPosition(45.0F, 150.0F);
            }
            yield return new WaitForSeconds(15.0F);
            mFirstSentence = true;
        }

        private IEnumerator GreenLightMomentAndTurn()
        {
            Debug.Log("COUNTSTATE : COROUTINE GREENLIGHT BEFORE WAIT FOR SECOND");
            yield return new WaitForSeconds(3.0F);
            if(mTTS.HasFinishedTalking() && mCount == 0)
            {
                Debug.Log("COUNTSTATE : COROUTINE GREENLIGHT AFTER WAIT FOR SECOND");
                mCanvasUIToWin.SetActive(true);
                mTTS.Say("Green Light !");
                mWheels.TurnAngle(180.0F, 250.0F, 0.3F);
                mCount++;
                mIsCoroutineDone = true;
            }
                        
        }

        private IEnumerator ChangeState(float iSecondToWait, Animator iAnimator)
        {
            Debug.Log("COUNTSTATE : COROUTINE CHANGESTATE BEFORE WAIT FOR SECOND");
            yield return new WaitForSeconds(iSecondToWait);
            Debug.Log("COUNTSTATE : COROUTINE CHANGESTATE AFTER WAIT FOR SECOND");
            iAnimator.SetBool("IsCountDone", true);
        }

    }
}

