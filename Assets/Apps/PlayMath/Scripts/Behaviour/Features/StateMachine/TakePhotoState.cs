using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.PlayMath{
    public class TakePhotoState : AStateMachineBehaviour {

		private Animator mTakePhotoAnimator;

        private TakePhotoBehaviour mTakePhotoBehaviour;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mTakePhotoAnimator = GameObject.Find("UI/Take_Photo").GetComponent<Animator>();
			mTakePhotoAnimator.SetTrigger("open");

            mTakePhotoBehaviour = GameObject.Find("UI/Take_Photo").GetComponent<TakePhotoBehaviour>();
            mTakePhotoBehaviour.DisplayCamera();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mTakePhotoAnimator.SetTrigger("close");
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
