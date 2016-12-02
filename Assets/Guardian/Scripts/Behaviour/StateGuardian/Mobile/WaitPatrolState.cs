using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class WaitPatrolState : AStateGuardian
    {

        [SerializeField]
        private float TimeToWait = 5.0f;

        [SerializeField]
        private string mParameterName = "";

        [SerializeField]
        private int mParameterValue = 1;

        private float mTimeStart = 0.0f;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mTimeStart == 0.0f)
            {
                mTimeStart = Time.fixedTime;
                // Debug.Log("time set: "+mTimeStart);
            }

            if (Time.fixedTime - mTimeStart > TimeToWait)
            {
                animator.SetInteger(mParameterName, mParameterValue);
                mTimeStart = 0.0f;
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {


            //Debug.Log("is detecting mouv: " + animator.GetBehaviour<DetectionPatrolState>().mIsDetectingMovement);

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        public void Reset()
        {
            mTimeStart = 0.0f;
        }
    }
}