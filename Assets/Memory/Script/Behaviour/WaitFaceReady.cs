using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.Memory
{
	public class WaitFaceReady : LinkStateMachineBehavior
	{

		float timer = 0.0f;

		public override void Init()
		{
			mOnEnterDone = false;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mOnEnterDone = true;

		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mOnEnterDone) {
				timer += Time.deltaTime;

				//if (link.mFace.IsStable && timer > 1.0f) {
				if (timer > 1.0f) {
					animator.SetTrigger("FaceReady");
				}

			}
		}


		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

	}
}