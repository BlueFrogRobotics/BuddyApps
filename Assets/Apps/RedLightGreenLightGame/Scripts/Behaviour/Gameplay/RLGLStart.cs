using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLStart : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mIsGoodPosition;
        private float mLimit;
        private bool mIsLimitDone;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsLimitDone = false;
            mLimit = 100F;
            mRLGLBehaviour.Timer = 0F;
            mIsGoodPosition = false;

            Perception.Motion.OnDetect(OnMovementDetected, 15F);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mRLGLBehaviour.Timer > 2F && !mIsLimitDone)
            {
                mLimit = mRLGLBehaviour.Timer;
                mIsLimitDone = true;
            }
            Debug.Log("LIMIT : " + mLimit + " TIMER : " + mRLGLBehaviour.Timer);
            if (mRLGLBehaviour.Timer > mLimit + 3F && mIsGoodPosition)
            {
                if (!Interaction.TextToSpeech.HasFinishedTalking)
                    Interaction.TextToSpeech.SayKey("greatstartgame");
                else if (Interaction.TextToSpeech.HasFinishedTalking)
                    Trigger("StartGame");
            } 
            else if(mRLGLBehaviour.Timer > mLimit + 3F && !mIsGoodPosition)
                Trigger("FailedToPositionning");

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            
            if(mRLGLBehaviour.Timer > 2F  && iMotions.Length > 30)
            {
                Debug.Log("Detected start");
                mIsGoodPosition = true;
                return false;
            }
            return true;
        }
    }
}

