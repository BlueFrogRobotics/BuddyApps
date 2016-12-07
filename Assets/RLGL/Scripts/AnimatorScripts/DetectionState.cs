using UnityEngine;
using System.Collections;
using BuddyOS;
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
            Debug.Log("On Enter Detection state");
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
            Debug.Log("On update Detection state");
            mTimer += Time.deltaTime;
            if (!mIsMovementActionDone)
            {
                mTTS.Say("Red Light");
                mWheels.TurnAngle(-180.0F, 250.0F, 0.3F);
                mIsMovementActionDone = true;
            }
            Debug.Log("STATUS DETECTION STATE : " + mWheels.Status);

            if (mWheels.Status == MobileBaseStatus.REACHED_GOAL )
            {
                mIsMovementDone = true;
                Debug.Log("DETECTION UPDATE : MOVEMENT DONE");
            }
            
            if ( mTTS.HasFinishedTalking() && mIsMovementDone)
            {
                mIsDetected = GetComponent<FreezeDance.MotionGame>().IsMoving();

                if(mIsDetected && !mIsSentenceDone && mTimer < 8.0f && mTimer > 3.0f)
                {
                    mFace.SetMood(FaceMood.HAPPY);
                    mFace.SetEvent(FaceEvent.SMILE);

                    mTTS.Say("I saw you moving my friend, I let you ten seconds to go back at the start!");
                    mIsSentenceDone = true;
                }
                if (mIsSentenceDone && mTTS.HasFinishedTalking() && mTimer > 20.0f)
                {
                    mFace.SetMood(FaceMood.NEUTRAL);
                    GetComponent<FreezeDance.MotionGame>().enabled = false;
                    iAnimator.SetBool("IsDetectedTrue", true);
                }

                if (!mIsDetected && !mIsSentenceDone && mTimer > 4.0F && mTimer < 8.0F && !mIsStrong)
                {
                    mTTS.Say("hum tu es fort, je ne te vois pas bouger");
                    mFace.SetMood(FaceMood.FOCUS);
                    mIsStrong = true;
                }

                if (!mIsSentenceDone && mTimer > 8.0f && mTTS.HasFinishedTalking() && mIsStrong)
                {
                    mFace.SetMood(FaceMood.NEUTRAL);
                    GetComponent<FreezeDance.MotionGame>().enabled = false;
                    iAnimator.SetBool("IsDetectedFalse", true);
                }
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("On exit Detection state");
            iAnimator.SetBool("IsDetectedTrue", false);
            iAnimator.SetBool("IsDetectedFalse", false);
            
        }        
    }

}

