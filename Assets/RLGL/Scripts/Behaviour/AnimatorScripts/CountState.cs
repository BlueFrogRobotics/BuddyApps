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
        private bool mFirstSentenceNotDetected;
        private bool mIsReachedGoal;

        private bool mIsOneTurnDone;
        public bool IsOneTurnDone { get { return mIsOneTurnDone; } set { mIsOneTurnDone = value; } }

        private int mCount;
        private int mCountGreenLight;


        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("COUNT STATE : ON ENTER");
            mCanvasUIToWin = GetGameObject(0);
            mIsCoroutineDone = false;
            mIsMovementDone = false;
            mFirstSentence = false;
            mSecondSentence = false;
            mIsReachedGoal = false;
            mFirstSentenceNotDetected = false;
            mCount = 0;
            mCountGreenLight = 0;
            iAnimator.SetBool("IsCountDone", false);
            iAnimator.SetBool("IsWon", false);
            //mMood.Set(MoodType.HAPPY);

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            if (!mIsOneTurnDone) {
                if (mTTS.HasFinishedTalking && !mFirstSentence) {
                    StartCoroutine(WaitTenSecondsAtStart());
                }
                if (mTTS.HasFinishedTalking && mFirstSentence && !mSecondSentence) {
                    mMood.Set(MoodType.NEUTRAL);
                    mCount = 0;
                    mTTS.Say("Ok let's go!");
                    mSecondSentence = true;
                }
            }
            if (mIsOneTurnDone) {
                if (mTTS.HasFinishedTalking && !mFirstSentenceNotDetected) {
                    StartCoroutine(NotDetected());

                }

            }
            if (mSecondSentence || mFirstSentenceNotDetected) {
                if (mTTS.HasFinishedTalking) {
                    StartCoroutine(GreenLightMomentAndTurn());
                    mCountGreenLight = 0;
                }

                if ((mWheels.Status == MovingState.REACHED_GOAL || (mWheels.Status == MovingState.MOTIONLESS && mIsReachedGoal)) && mIsCoroutineDone) {
                    mIsMovementDone = true;
                }

                if (mIsMovementDone && mTTS.HasFinishedTalking) {
                    StartCoroutine(ChangeState(3.0F, iAnimator));
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("COUNT STATE : ON EXIT");
            mCanvasUIToWin.SetActive(false);
        }

        private IEnumerator WaitTenSecondsAtStart()
        {
            if (mCount == 0 && mTTS.HasFinishedTalking) {
                mTTS.Say("Okay let's play together! You have ten seconds to go away by about fifteen feet, I will wait ten seconds gogo! ");
                mCount++;
                mYesHinge.SetPosition(45.0F, 150.0F);
            }
            yield return new WaitForSeconds(15.0F);
            mFirstSentence = true;
        }

        private IEnumerator NotDetected()
        {
            if (mTTS.HasFinishedTalking && mCountGreenLight == 0 && !mFirstSentenceNotDetected) {
                mTTS.Say("You are really good at this game! Go again!");
                mCountGreenLight++;
            }
            yield return new WaitForSeconds(2.0F);
            mFirstSentenceNotDetected = true;

        }

        private IEnumerator GreenLightMomentAndTurn()
        {
            yield return new WaitForSeconds(3.0F);
            if (mTTS.HasFinishedTalking && mCount == 0 && mCountGreenLight == 0) {
                mCanvasUIToWin.SetActive(true);
                mTTS.Say("Green Light !");
                mWheels.TurnAngle(180.0F, 250.0F, 0.02F);
                mCount++;
                mIsCoroutineDone = true;
            }
            yield return new WaitForSeconds(1.5F);
            mIsReachedGoal = true;


        }

        private IEnumerator ChangeState(float iSecondToWait, Animator iAnimator)
        {
            mMood.Set(MoodType.NEUTRAL);
            yield return new WaitForSeconds(iSecondToWait);
            iAnimator.SetBool("IsCountDone", true);
        }

    }
}

