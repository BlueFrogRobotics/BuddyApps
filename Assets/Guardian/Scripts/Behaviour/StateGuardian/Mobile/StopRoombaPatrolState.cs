﻿using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Guardian
{
    public class StopRoombaPatrolState : AStateGuardian
    {

        private BuddyFeature.Navigation.RoombaNavigation mRoomba;
        private Motors mMotors;
        private NoHinge mNoHinge;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRoomba = StateManager.Roomba;
            mNoHinge = BYOS.Instance.Motors.NoHinge;
            mMotors = BYOS.Instance.Motors;
            mRoomba.enabled = false;
            mMotors.Wheels.StopWheels();
            //animator.SetBool("TurnHead", false);
            mNoHinge.SetPosition(0);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRoomba.enabled = false;
            mMotors.Wheels.StopWheels();
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
    }
}