using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLPositionningPlayerMovement : AStateMachineBehaviour
    {
        private RedLightGreenLightGameBehaviour mRLGLBehaviour;
        private bool mMustStop = false;
        //private Vector3 mStartingOdometry;

        public override void Start()
        {
            mRLGLBehaviour = GetComponentInGameObject<RedLightGreenLightGameBehaviour>(0);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetGameObject(1).SetActive(true);
            mMustStop = false;
            mRLGLBehaviour.StartingOdometry = Primitive.Motors.Wheels.Odometry;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mMustStop)
            {
                if (!mRLGLBehaviour.TargetClicked)
                {
                    Primitive.Motors.Wheels.SetWheelsSpeedAtMedium();
                    Vector3 lDist=Primitive.Motors.Wheels.Odometry - mRLGLBehaviour.StartingOdometry;
                    Debug.Log("distance: " + lDist.magnitude);
                }
                else
                {   
                    Trigger("WantToPlay");
                    mMustStop = true;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

    }
}

