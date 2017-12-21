using UnityEngine;
using System.Collections;
using System;

namespace BuddyApp.MemoryGame
{
	public class WaitFaceReady : AStateMachineBehaviour
	{

		float timer = 0.0f;



		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			timer += Time.deltaTime;

			//if (link.mFace.IsStable && timer > 1.0f) {
			if (timer > 1.0f) {
				animator.SetTrigger("FaceReady");
			}
		}

	}
}