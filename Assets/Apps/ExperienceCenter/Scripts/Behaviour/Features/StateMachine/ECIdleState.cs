using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECIdleState : StateMachineBehaviour
	{
		private AnimatorManager mAnimatorManager;
		private IdleBehaviour mIdleBehaviour;
		private QuestionsBehaviour mQuestionBehaviour;

		private System.Action mOriginalListenBehaviour;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			mQuestionBehaviour = GameObject.Find("AIBehaviour").GetComponent<QuestionsBehaviour>();

			mOriginalListenBehaviour = BYOS.Instance.Interaction.VocalManager.StartListenBehaviour;

			mQuestionBehaviour.InitBehaviour();
			mIdleBehaviour.InitBehaviour ();
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mQuestionBehaviour.StopBehaviour();
			mIdleBehaviour.StopBehaviour ();

			BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
			BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
			BYOS.Instance.Interaction.VocalManager.StartListenBehaviour = mOriginalListenBehaviour;
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;

			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);

		}

		public void SpeechToTextError (STTError iError)
		{
			Debug.LogWarningFormat ("ERROR STT: {0}", iError.ToString ());
			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);
		}
	}
}
