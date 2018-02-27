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

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			mQuestionBehaviour = GameObject.Find ("AIBehaviour").GetComponent<QuestionsBehaviour> ();

			mQuestionBehaviour.InitBehaviour ();
			mIdleBehaviour.InitBehaviour ();
		}

		override public void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mQuestionBehaviour.StopBehaviour ();
			mIdleBehaviour.StopBehaviour ();

            BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
		}

		public void SpeechToTextError (STTError iError)
		{
			Debug.LogWarningFormat ("[EXCENTER][ECIDLESTATE] ERROR STT: {0}", iError.ToString ());
			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);
		}
	}
}
