using UnityEngine;
using System.Collections;


namespace BuddyApp.Memory
{
	public class WaitFaceReady : LinkStateMachineBehavior
	{

		float timer = 0.0f;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			timer += Time.deltaTime;

			//if (link.mFace.IsStable && timer > 1.0f) {
			if (timer > 1.0f) {
				animator.SetTrigger("FaceReady");
			}
		}

	}
}