using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.MemoryGame
{
	public class WaitFaceReady : AStateMachineBehaviour
	{

		float timer = 0.0f;

		public override void Start()
		{
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			timer += Time.deltaTime;

			//if (link.mFace.IsStable && timer > 1.0f) {
			if (timer > 1.0f) {
				animator.SetTrigger("FaceReady");
			}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

	}
}