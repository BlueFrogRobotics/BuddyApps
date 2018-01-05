using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECMoveForwardState : StateMachineBehaviour
	{

		private AnimatorManager mAnimatorManager;
		private MoveForwardBehaviour mBehaviour;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<MoveForwardBehaviour> ();
			mBehaviour.InitBehaviour ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBehaviour.StopBehaviour ();
		}

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
			Debug.Log ("MoveForward - SpeechToText : " + iSpeech);
			bool lClauseFound = false;
			string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings ("movego");
			Debug.Log ("MoveForward - Phonetics : " + lPhonetics.Length);
			foreach (string lClause in lPhonetics) {
				if (iSpeech.Contains (lClause)) {
					lClauseFound = true;
					break;
				}
			}

			if (!lClauseFound)
				Debug.Log ("MoveForward - SpeechToText : Not Found");
			else
				mAnimatorManager.ActivateCmd ((byte)(Command.IOT));
		}
	}
}
