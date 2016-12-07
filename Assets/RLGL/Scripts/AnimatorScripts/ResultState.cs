using UnityEngine;
using System.Collections;
using BuddyOS;
using System;

namespace BuddyApp.RLGL
{
    public class ResultState : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsMovementDone;
        
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("On Enter result state");
            mIsSentenceDone = false;
            mIsMovementDone = false;
            mTimer = 0.0f;
            mWheels.TurnAngle(-180.0F, 250.0F, 0.5F);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            Debug.Log("On update result state");
            mTimer += Time.deltaTime;
            if(mWheels.Status == MobileBaseStatus.REACHED_GOAL && !mIsMovementDone)
            {
                mIsMovementDone = true;
                mFace.SetMood(FaceMood.HAPPY);
            }

            
            if (mTTS.HasFinishedTalking() && mTimer < 6.0f && !mIsSentenceDone && mIsMovementDone)
            {
                mTTS.Say("Good job you won, you have been too fast for me!");
                mIsSentenceDone = true;
            }
            if (mTTS.HasFinishedTalking() && mIsSentenceDone)
                mFace.SetMood(FaceMood.NEUTRAL);
            if (mTimer > 6.0f)
                iAnimator.SetBool("IsReplayTrue", true);

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("On exit result state");
            iAnimator.SetBool("IsReplayTrue", false);
        }

    }

}
