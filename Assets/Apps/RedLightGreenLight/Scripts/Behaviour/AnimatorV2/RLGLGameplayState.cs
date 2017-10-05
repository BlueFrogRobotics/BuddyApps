using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    public class RLGLGameplayState : AStateMachineBehaviour
    {
        private RLGLMovementManager mMovementManager;

        private float mTimer;

        private bool mIsMovementActionDone;
        private bool mIsDetected;
        private bool mIsSentenceDone;
        private bool mIsStrong;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer = 0.0F;
            mIsMovementActionDone = false;
            mIsDetected = false;
            mIsSentenceDone = false;
            mIsStrong = false;
            mMovementManager.LinkDetectorEvent();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mIsMovementActionDone)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState1"));
                //Interaction.TextToSpeech.Say("Red Light");
                Primitive.Motors.Wheels.TurnAngle(-180.0F, 250.0F, 0.02F);
                mIsMovementActionDone = true;
                mTimer = 0.0F;
            }

            if ((Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || (mTimer > 1.5F && Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS))
                && Interaction.TextToSpeech.HasFinishedTalking)
            {
                mIsDetected = mMovementManager.IsMovementDetected;

                if (mIsDetected && !mIsSentenceDone && mTimer < 8.0f && mTimer > 3.0f)
                {
                    Interaction.Mood.Set(MoodType.HAPPY);
                    animator.GetBehaviour<CountState>().IsOneTurnDone = false;
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState2"));
                    //Interaction.TextToSpeech.Say("I saw you moving my friend" /*, go back at the start*/ + "!");

                    mIsSentenceDone = true;
                }
                if (mIsSentenceDone && Interaction.TextToSpeech.HasFinishedTalking && mTimer > 10.0f)
                {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mMovementManager.UnlinkDetectorEvent();
                    //animator.SetBool("IsDetectedTrue", true);
                    animator.SetTrigger("Detected");
                }

                if (!mIsDetected && !mIsSentenceDone && mTimer > 4.0F && mTimer < 8.0F && !mIsStrong)
                {
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState3"));
                    //Interaction.TextToSpeech.Say("Wow you are strong, I don't see you moving!");
                    Interaction.Mood.Set(MoodType.THINKING);
                    mIsStrong = true;
                }

                if (!mIsSentenceDone && mTimer > 8.0f && Interaction.TextToSpeech.HasFinishedTalking && mIsStrong)
                {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mMovementManager.UnlinkDetectorEvent();
                    animator.GetBehaviour<CountState>().IsOneTurnDone = true;
                    //animator.SetBool("IsDetectedFalse", true);
                    animator.SetTrigger("NotDetected");
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

    }
}

