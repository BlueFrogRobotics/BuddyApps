using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{


	public class QuestionsBehaviour : MonoBehaviour
	{
		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		private TextToSpeech mTTS;
		private List <string> mKeyList;

		public DateTime LastSTTCallbackTime { get; private set; }

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			BYOS.Instance.Interaction.VocalManager.StartListenBehaviour = SpeechToTextStart;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			InitKeyList ();
		}
			
		private void InitKeyList ()
		{
			mKeyList = new List<string>
			{
				"idlesee",
				"idleleg",
				"questiongreeting",
				"questionshy",
				"questionability",
				"questiondance",
				"questionlangage",
				"questionvibe",
				"questionpresence",
				"questionpresentation"
			};
		}

		public void SpeechToTextStart()
		{
			LastSTTCallbackTime = DateTime.Now;
            if (ExperienceCenterData.Instance.EnableHeadMovement)
            {
                mAttitudeBehaviour.IsWaiting = false;
                BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
                BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
            }
		}

		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.LogFormat ("Questions - SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string lKey = "";
			foreach (string lElement in mKeyList)
			{
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings(lElement);
				Debug.LogFormat ("Questions - Phonetics : {0}", lPhonetics.Length);
				foreach (string lClause in lPhonetics)
				{
					if (iSpeech.Contains (lClause)) {
						if (ExperienceCenterData.Instance.EnableHeadMovement) 
							mAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);
						mTTS.SayKey (lElement, true);
						lClauseFound = true;
						lKey = lElement;
						break;
					}
				}
				if (lClauseFound) {
					BYOS.Instance.Interaction.SpeechToText.Stop ();
					break;
				}
			}
			if (!lClauseFound)
				Debug.Log ("Questions - SpeechToText : Not Found");
			else
			{
				if (lKey == "idlesee")
					mAnimatorManager.ActivateCmd((byte)(Command.Welcome));
				else if (lKey == "idletalk")
					mAnimatorManager.ActivateCmd((byte)(Command.Questions));
				else if (lKey == "idleleg")
				{
					mTTS.SayKey(lKey, true);
					mAnimatorManager.ActivateCmd((byte)(Command.ByeBye));
				}
			}
		}

		public void StopBehaviour ()
		{
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
		}
	}
}