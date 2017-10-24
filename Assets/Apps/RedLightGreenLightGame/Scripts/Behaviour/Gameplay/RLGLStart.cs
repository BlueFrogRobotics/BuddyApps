using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLStart : AStateMachineBehaviour
    {

        //TODO : Add the canvas with yellow square to help the player to position himself

        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mIsGoodPosition;
        private float mLimit;
        private bool mIsLimitDone;
        private RGBCam mCam;
        private bool mHasTalked = false;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.TextToSpeech.SayKey("remplacementpositionningplayer");
            Debug.Log("1");
            mCam = Primitive.RGBCam;
            Debug.Log("2");
            mIsLimitDone = false;
            mLimit = 100F;
            Debug.Log("3");
            mRLGLBehaviour.Timer = 0F;
            Debug.Log("4");
            mIsGoodPosition = false;
            Debug.Log("5");
            mHasTalked = false;
            Perception.Motion.OnDetect(OnMovementDetected, 9F);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer > 5F && !mIsLimitDone)
            {
                mLimit = mRLGLBehaviour.Timer;
                mIsLimitDone = true;
            }
            Debug.Log("LIMIT : " + mLimit + " TIMER : " + mRLGLBehaviour.Timer + " GOOD POSITION : " + mIsGoodPosition);

            //If we move during this time, we start the gameplay else the robot said that we need to position ourself in front of him
            if (/*mRLGLBehaviour.Timer > mLimit + 3F &&*/ mIsGoodPosition)
            {
                Interaction.Face.SetExpression(MoodType.HAPPY);
                if (!Interaction.TextToSpeech.HasFinishedTalking && !mHasTalked)
                {
                    Interaction.TextToSpeech.SayKey("greatstartgame");
                    mHasTalked = true;
                }
                else if (Interaction.TextToSpeech.HasFinishedTalking)
                    Trigger("StartGame");
            } 
            else if(mRLGLBehaviour.Timer > mLimit + 3F && !mIsGoodPosition)
                Trigger("FailedToPositionning");

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Face.SetExpression(MoodType.NEUTRAL);
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            if(mRLGLBehaviour.Timer > 2F  && iMotions.Length > 50)
            {
                Debug.Log("Detected start");
                mIsGoodPosition = true;
                mCam.Close();
                return false;
            }
            return true;
        }
    }
}

