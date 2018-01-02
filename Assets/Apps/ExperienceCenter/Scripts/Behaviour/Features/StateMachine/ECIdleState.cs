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

		//	  OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;

			//To test with the tablet comment the first line and uncomment the second one
			mBehaviour.InitBehaviour();
			//mBehaviour.behaviourInit = true;
		}

		//	 OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
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

		public void SpeechToTextCallback(string iSpeech)
		{
			Debug.Log("SpeechToText : " + iSpeech);
			if (iSpeech == "c'est super de te voir" || iSpeech == "nice to see you") {
				mAnimatorManager.ActivateCmd((byte) (Command.Welcome));
			} 
			else if (iSpeech == "bavardons un peu" || iSpeech == "let's talk a little") {
				mAnimatorManager.ActivateCmd((byte) (Command.Questions));
			}
			else if (iSpeech == "as-tu envie de te dégourdir les jambes" || iSpeech == "do you want to stretch your legs") {
				mTTS.SayKey ("byemerci", true);
				mAnimatorManager.ActivateCmd((byte) (Command.ByeBye));
			}
		}
			
	}
}
