using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class ECInitByeState : StateMachineBehaviour
	{
		
		private AnimatorManager mAnimatorManager;
		private IdleBehaviour mIdleBehaviour;
		private QuestionsBehaviour mQuestionsBehaviour;
		private TextToSpeech mTTS;

		private bool mAddReco;

		override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			mQuestionsBehaviour = GameObject.Find ("AIBehaviour").GetComponent<QuestionsBehaviour> ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
            BYOS.Instance.Interaction.VocalManager.EnableDefaultErrorHandling = false;
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Clear();
            //BYOS.Instance.Interaction.SpeechToText.OnErrorEnum.Add(SpeechToTextError);
			BYOS.Instance.Interaction.VocalManager.OnError = SpeechToTextError;
			mAddReco = false;
		}

		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!mAddReco && mIdleBehaviour.behaviourEnd && mQuestionsBehaviour.behaviourEnd) {
                //if (ExperienceCenterData.Instance.VoiceTrigger)
                //    BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
                BYOS.Instance.Interaction.VocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Clear();
                //BYOS.Instance.Interaction.SpeechToText.OnBestRecognition.Add(SpeechToTextCallback);
                BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
				mAddReco = true;
			}
		}

		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.LogFormat ("[EXCENTER] ByeBye - SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings ("byecome");
			foreach (string lClause in lPhonetics) {
				if (iSpeech.Contains (lClause)) {
					lClauseFound = true;
					break;
				}
			} 
				
			if (!lClauseFound)
				Debug.Log ("[EXCENTER] ByeBye - SpeechToText : Not Found");
			else {
				mAnimatorManager.ActivateCmd ((byte)(Command.MoveForward));
			}
		}

		public void SpeechToTextError (STTError iError)
		{
			Debug.LogWarningFormat ("[EXCENTER][BYE] ERROR STT: {0}", iError.ToString ());
			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);
		}
	}
}
