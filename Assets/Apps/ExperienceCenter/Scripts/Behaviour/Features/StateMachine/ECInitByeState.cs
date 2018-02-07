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
			mAddReco = false;
		}
			
		override public void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!mAddReco && mIdleBehaviour.behaviourEnd && mQuestionsBehaviour.behaviourEnd) {
				BYOS.Instance.Interaction.VocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
				BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
				mAddReco = true;
			}
		}

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
