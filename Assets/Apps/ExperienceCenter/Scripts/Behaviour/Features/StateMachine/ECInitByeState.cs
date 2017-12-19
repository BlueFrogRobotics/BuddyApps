using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
namespace BuddyApp.ExperienceCenter
{
	public class ECInitByeState : StateMachineBehaviour
	{
		private Animator mMainAnimator;
		private TextToSpeech mTTS;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mMainAnimator = GameObject.Find ("AIBehaviour").GetComponent<Animator> ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.Log ("SpeechToText : " + iSpeech);
			if (iSpeech == "viens dans mes bras" || iSpeech == "come into my arms") {
				mMainAnimator.SetTrigger ("MoveForward");
				Debug.Log ("[VOICE] Switch to MoveForward State");
			} 

		}
	}
}
