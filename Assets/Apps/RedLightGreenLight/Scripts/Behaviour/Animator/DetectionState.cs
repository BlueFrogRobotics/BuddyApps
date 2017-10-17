using UnityEngine;
using System.Collections;
using Buddy;
using System;

namespace BuddyApp.RedLightGreenLight
{
    public class DetectionState : AStateMachineBehaviour
    {
        private bool mIsSentenceDone;
        private bool mIsDetected;
        private float mTimer;
        private bool mIsStrong;
        private bool mIsMovementActionDone;
        private bool mIsMovementDone;
        private RLGLMovementManager mMovementManager;

       //MANY TODO : Function for the update for the time which changes when the player is strong and its been a long time without detection

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0F;
            mIsSentenceDone = false;
            mIsDetected = false;
            mIsMovementDone = false;
            mIsStrong = false;
            mIsMovementActionDone = false;
            mMovementManager.LinkDetectorEvent();
            //GetComponent<FreezeDance.MotionGame>().enabled = true;
            //GetComponent<FreezeDance.MotionGame>().IsRLGL = true;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mIsMovementActionDone) {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState1"));
                //Interaction.TextToSpeech.Say("Red Light");
                Primitive.Motors.Wheels.TurnAngle(-180.0F, 250.0F, 0.02F);
                mIsMovementActionDone = true;
                mTimer = 0.0F;
            }
             
            if (Primitive.Motors.Wheels.Status == MovingState.REACHED_GOAL || (mTimer > 1.5F && Primitive.Motors.Wheels.Status == MovingState.MOTIONLESS)) {
                mIsMovementDone = true;
            }

            if (Interaction.TextToSpeech.HasFinishedTalking && mIsMovementDone) {

                //TODO :
                //NEEDS TO BE DONE : ADD DIFFICULTY ABOUT MOVEMENT DETECTION
                mIsDetected = mMovementManager.IsMovementDetected;

                if (mIsDetected && !mIsSentenceDone && mTimer < 8.0f && mTimer > 3.0f) {
                    Interaction.Mood.Set(MoodType.HAPPY);
                    iAnimator.GetBehaviour<CountState>().IsOneTurnDone = false;
                    //MAYBE ADD A SILENCE IN CASE OF THE PLAYER HAS BEEN DETECTED AFTER THE SENTENCE WHICH SAID HE IS STRONG
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState2"));
                    //Interaction.TextToSpeech.Say("I saw you moving my friend" /*, go back at the start*/ + "!");

                    mIsSentenceDone = true;
                }
                if (mIsSentenceDone && Interaction.TextToSpeech.HasFinishedTalking && mTimer > 10.0f) {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mMovementManager.UnlinkDetectorEvent();
                    iAnimator.SetBool("IsDetectedTrue", true);
                }

                if (!mIsDetected && !mIsSentenceDone && mTimer > 4.0F && mTimer < 8.0F && !mIsStrong) {
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("detectionState3"));
                    //Interaction.TextToSpeech.Say("Wow you are strong, I don't see you moving!");
                    Interaction.Mood.Set(MoodType.THINKING);
                    mIsStrong = true;
                }

                if (!mIsSentenceDone && mTimer > 8.0f && Interaction.TextToSpeech.HasFinishedTalking && mIsStrong) {
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                    mMovementManager.UnlinkDetectorEvent();
                    iAnimator.GetBehaviour<CountState>().IsOneTurnDone = true;
                    iAnimator.SetBool("IsDetectedFalse", true);
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetBool("IsDetectedTrue", false);
            iAnimator.SetBool("IsDetectedFalse", false);

        }
    }

}

