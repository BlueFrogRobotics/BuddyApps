using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
	public class ECIdleState : StateMachineBehaviour
	{
		
		private AnimatorManager mAnimatorManager;
		private IdleBehaviour mIdleBehaviour;
		private QuestionsBehaviour mQuestionBehaviour;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            Debug.Log("[EXCENTER] on state ECIdleState");
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

            //BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);     
            //BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
            //BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
            Buddy.Vocal.EnableTrigger = true;
		}

		//public void SpeechToTextError (STTError iError)
		//{
		//	Debug.LogWarningFormat ("[EXCENTER][ECIDLESTATE] ERROR STT: {0}", iError.ToString ());
		//	Buddy.Behaviour.Mood = Mood.NEUTRAL;
		//}
	}
}
