using UnityEngine;
using System.Collections;
using BuddyOS.App;
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
            Debug.Log("RESULT STATE : ON ENTER");
            mIsSentenceDone = false;
            mIsMovementDone = false;
            mTimer = 0.0f;
            mWheels.TurnAngle(-180.0F, 250.0F, 0.5F);
            mYesHinge.SetPosition(0.0F, 150.0F);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            Debug.Log("RESULT STATE : ON UPDATE");
            mTimer += Time.deltaTime;
            if((mWheels.Status == MovingState.REACHED_GOAL || (mWheels.Status == MovingState.MOTIONLESS && mTimer < 2.0F)) && !mIsMovementDone)
            {
                mIsMovementDone = true;
                mMood.Set(MoodType.HAPPY);
            }

            
            if (mTTS.HasFinishedTalking && mTimer < 6.0f && !mIsSentenceDone && mIsMovementDone)
            {

                mTTS.Say("Good job you won, you have been too fast for me!");
                mIsSentenceDone = true;
            }
            if (mTTS.HasFinishedTalking && mIsSentenceDone)
                mMood.Set(MoodType.NEUTRAL);
            if (mTimer > 6.0f)
                iAnimator.SetBool("IsReplayTrue", true);

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("RESULT STATE : ON EXIT");
            iAnimator.SetBool("IsReplayTrue", false);
        }

    }

}
