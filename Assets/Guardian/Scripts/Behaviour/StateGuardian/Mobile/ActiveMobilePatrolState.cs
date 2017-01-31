using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class ActiveMobilePatrolState : AStateGuardian
    {

        private BuddyFeature.Navigation.RoombaNavigation mRoomba;
        private Motors mMotors;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRoomba = StateManager.Roomba;
            mMotors = BYOS.Instance.Motors;

            animator.SetInteger("mobileState", 0);
            //animator.SetBool("TurnHead", true);
            mRoomba.enabled = true;
            animator.GetBehaviour<DetectionPatrolState>().IsDetectingMovement = false;
            animator.GetBehaviour<DetectionPatrolState>().IsDetectingKidnapping = false;
            mMotors.YesHinge.SetPosition(20, 80);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("roomba active");
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