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
		private TextToSpeech mTTS;
		private List <string> mKeyList;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			InitKeyList ();
		}
			
		private void InitKeyList ()
		{
			mKeyList = new List<string> ();
			mKeyList.Add ("questiongreeting");
			mKeyList.Add ("questionshy");
			mKeyList.Add ("questionability");
			mKeyList.Add ("questiondance");
			mKeyList.Add ("questionlangage");
			mKeyList.Add ("questionvibe");
			mKeyList.Add ("questionpresence");
			mKeyList.Add ("questionpresentation");
		}

		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.Log ("Questions - SpeechToText : " + iSpeech);
			bool lClauseFound = false;
			foreach (string lElement in mKeyList)
			{
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings(lElement);
				Debug.Log ("Questions - Phonetics : " + lPhonetics.Length);
				foreach (string lClause in lPhonetics)
				{
					if (iSpeech.Contains (lClause)) {
						mTTS.SayKey (lElement, true);
						lClauseFound = true;
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
			
		}
	}
}