﻿using UnityEngine;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class DebugThermicState : AStateGuardian
    {

        private ShowTemperature mShowTemperature;
        private Animator mAnimator;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(0);
            mShowTemperature = StateManager.ShowTemperature;
            StateManager.ShowTemperature.gameObject.GetComponent<Animator>().SetTrigger("Open_WDebugs");
            //mShowTemperature.gameObject.SetActive(true);
            mAnimator = animator;
            mShowTemperature.ButtonBack.onClick.AddListener(GoBack);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //mShowTemperature.FillTemperature(null);
            mShowTemperature.UpdateTexture();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mShowTemperature.ButtonBack.onClick.RemoveAllListeners();
            StateManager.ShowTemperature.gameObject.GetComponent<Animator>().SetTrigger("Close_WDebugs");
            //mShowTemperature.gameObject.SetActive(false);
        }

        private void GoBack()
        {
            mAnimator.SetInteger("DebugMode", -1);
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