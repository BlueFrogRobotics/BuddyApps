using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class ReinitialisationMobileState : AStateGuardian
    {

        [SerializeField]
        private string mParameterName = "";

        [SerializeField]
        private int mParameterValue = 1;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            WaitPatrolState[] lWaitStates = animator.GetBehaviours<WaitPatrolState>();
            for (int i = 0; i < lWaitStates.Length; i++)
            {
                lWaitStates[i].Reset();
            }

            animator.SetInteger("Angle", 0);
            animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = false;
            animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = false;

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(mParameterName, mParameterValue);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}