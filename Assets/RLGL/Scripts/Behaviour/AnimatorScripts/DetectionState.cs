using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.RLGL
{
    public class DetectionState : AStateMachineBehaviour
    {
        private bool mIsSentenceDone;
        private bool mIsDetected;
        private float mTimer;
        private bool mIsStrong;
        private bool mIsMovementActionDone;
        private bool mIsMovementDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0F;
            mIsSentenceDone = false;
            mIsDetected = false;
            mIsMovementDone = false;
            mIsStrong = false;
            mIsMovementActionDone = false;
            GetComponent<FreezeDance.MotionGame>().enabled = true;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mIsMovementActionDone) {
                mTTS.Say("Red Light");
                mWheels.TurnAngle(-180.0F, 250.0F, 0.02F);
                mIsMovementActionDone = true;
                mTimer = 0.0F;
            }

            if (mWheels.Status == MovingState.REACHED_GOAL || (mTimer > 1.5F && mWheels.Status == MovingState.MOTIONLESS)) {
                mIsMovementDone = true;
            }

            if (mTTS.HasFinishedTalking && mIsMovementDone) {
                mIsDetected = GetComponent<FreezeDance.MotionGame>().IsMoving();

                if (mIsDetected && !mIsSentenceDone && mTimer < 8.0f && mTimer > 3.0f) {
                    mMood.Set(MoodType.HAPPY);
                    iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                    mTTS.Say("I saw you moving my friend, go back at the start!");

                    mIsSentenceDone = true;
                }
                if (mIsSentenceDone && mTTS.HasFinishedTalking && mTimer > 10.0f) {
                    mMood.Set(MoodType.NEUTRAL);
                    GetComponent<FreezeDance.MotionGame>().enabled = false;
                    iAnimator.SetBool("IsDetectedTrue", true);
                }

                if (!mIsDetected && !mIsSentenceDone && mTimer > 4.0F && mTimer < 8.0F && !mIsStrong) {
                    mTTS.Say("Wow you are strong, I don't see you moving!");
                    mMood.Set(MoodType.THINKING);
                    mIsStrong = true;
                }

                if (!mIsSentenceDone && mTimer > 8.0f && mTTS.HasFinishedTalking && mIsStrong) {
                    mMood.Set(MoodType.NEUTRAL);
                    GetComponent<FreezeDance.MotionGame>().enabled = false;
                    iAnimator.GetBehaviour<CountState>().IsOneTurnDone = true;
                    iAnimator.SetBool("IsDetectedFalse", true);
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsDetectedTrue", false);
            iAnimator.SetBool("IsDetectedFalse", false);

        }
    }

}

