using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECInitByeState : StateMachineBehaviour
	{
		private AnimatorManager mAnimatorManager;
		private TextToSpeech mTTS;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
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
			Debug.LogFormat ("ByeBye - SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings ("byecome");
			Debug.LogFormat ("ByeBye - Phonetics : {0}", lPhonetics.Length);
			foreach (string lClause in lPhonetics) {
				if (iSpeech.Contains (lClause)) {
					lClauseFound = true;
					break;
				}
			}
				
			if (!lClauseFound)
				Debug.Log ("ByeBye - SpeechToText : Not Found");
			else
				mAnimatorManager.ActivateCmd ((byte)(Command.MoveForward));
		}

	}
}
