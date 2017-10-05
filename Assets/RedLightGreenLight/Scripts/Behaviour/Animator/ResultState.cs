using UnityEngine;
using System.Collections;
using Buddy;
using System;

namespace BuddyApp.RedLightGreenLight
{
    public class ResultState : AStateMachineBehaviour
    {
        private float mTimer;
        private bool mIsSentenceDone;
        private bool mIsMovementDone;
        
       
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mIsSentenceDone = false;
            mIsMovementDone = false;
            mTimer = 0.0f;
            Primitive.Motors.Wheels.TurnAngle(-180.0F, 250.0F, 0.5F);
            Primitive.Motors.YesHinge.SetPosition(0.0F, 150.0F);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iSateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || (Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS && mTimer < 2.0F)) && !mIsMovementDone)
            {
                mIsMovementDone = true;
                Interaction.Mood.Set(MoodType.HAPPY);
            }

            
            if (Interaction.TextToSpeech.HasFinishedTalking && mTimer < 6.0f && !mIsSentenceDone && mIsMovementDone)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("resultState1"));
                //Interaction.TextToSpeech.Say("Good job you won, you have been too fast for me!");
                mIsSentenceDone = true;
            }
            if (Interaction.TextToSpeech.HasFinishedTalking && mIsSentenceDone)
                Interaction.Mood.Set(MoodType.NEUTRAL);
            if (mTimer > 6.0f)
                iAnimator.SetBool("IsReplayTrue", true);

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsReplayTrue", false);
        }

    }

}
