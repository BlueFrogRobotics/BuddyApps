using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECIdleState : StateMachineBehaviour
	{
		public const float ANGLE_THRESHOLD = 0.1f;

		private AnimatorManager mAnimatorManager;
		private IdleBehaviour mBehaviour;
		private TextToSpeech mTTS;
		private List <string> mKeyList;

        private BMLManager mBMLManager;

		//	  OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
            mBMLManager = BYOS.Instance.Interaction.BMLManager;
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			InitKeyList ();
			//To test with the tablet comment the first line and uncomment the second one
			//mBehaviour.InitBehaviour ();
			mBehaviour.behaviourInit = true;
		}

		//	 OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            if(mBMLManager.DonePlaying)
                mBMLManager.LaunchRandom("Idle");
        }

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
//			if (!mTTS.HasFinishedTalking)
//				mTTS.Stop ();
			mBehaviour.StopBehaviour ();
            mBMLManager.StopAllBehaviors();
		}

		// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//
		//}

		private void InitKeyList ()
		{
			mKeyList = new List<string> ();
			mKeyList.Add ("idlesee");
			mKeyList.Add ("idletalk");
			mKeyList.Add ("idleleg");
		}

		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.Log ("Idle - SpeechToText : " + iSpeech);
			bool lClauseFound = false;
			string lKey = "";
			foreach (string lElement in mKeyList) {
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings (lElement);
				Debug.Log ("Idle - Phonetics : " + lPhonetics.Length);
				foreach (string lClause in lPhonetics) {
					if (iSpeech.Contains (lClause)) {
						mTTS.SayKey (lElement, true);
						lClauseFound = true;
						lKey = lElement;
						break;
					}
				}
				if (lClauseFound) {
					mTTS.Stop ();
					break;
				}
			}
			if (!lClauseFound) {
				Debug.Log ("Idle - SpeechToText : Not Found");
			} else {
				if (lKey == "idlesee") {
					mAnimatorManager.ActivateCmd ((byte)(Command.Welcome));
				} else if (lKey == "idletalk") {
					mAnimatorManager.ActivateCmd ((byte)(Command.Questions));
				} else if (lKey == "idleleg") {
					mAnimatorManager.ActivateCmd ((byte)(Command.ByeBye));
				}
			}

		}
			
	}
}
