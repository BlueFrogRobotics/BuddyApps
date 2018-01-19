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
		private IdleBehaviour mIdleBehaviour;
		private TextToSpeech mTTS;
		private List <string> mKeyList;

		public DateTime LastSTTCallbackTime { get; private set; }
		public bool behaviourEnd;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			mIdleBehaviour = GameObject.Find("AIBehaviour").GetComponent<IdleBehaviour>();
			behaviourEnd = false;
			BYOS.Instance.Interaction.VocalManager.EnableTrigger = true;
			BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
			BYOS.Instance.Interaction.SphinxTrigger.SetThreshold(1E-24f);
			// To test with the real robot
			//BYOS.Instance.Interaction.VocalManager.StartListenBehaviour = SpeechToTextStart;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			LastSTTCallbackTime = DateTime.Now;
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
			Debug.LogWarning ("Head Pose =" + mIdleBehaviour.headPoseInit);
			if (ExperienceCenterData.Instance.EnableHeadMovement && !mIdleBehaviour.headPoseInit)
            {
                mAttitudeBehaviour.IsWaiting = false;
				Debug.LogWarning ("Stop BML");
                BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
                BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
				mIdleBehaviour.headPoseInit = true;
            } 
		}

		public void SpeechToTextCallback (string iSpeech)
		{
			// To test with the simulator
			SpeechToTextStart ();
			Debug.LogFormat ("SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string lKey = "";
			foreach (string lElement in mKeyList)
			{
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings(lElement);
				Debug.LogFormat ("Phonetics : {0}", lPhonetics.Length);
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
			// Launch Vocal Command if any
			if (!lClauseFound)
				Debug.Log ("SpeechToText : Not Found");
			else
			{
				StartCoroutine (LaunchVocalCommand (lKey));
			}
		}

		private IEnumerator LaunchVocalCommand (string key)
		{
			yield return new WaitUntil (() =>mTTS.HasFinishedTalking);
			if (key == "idlesee")
				mAnimatorManager.ActivateCmd((byte)(Command.Welcome));
			else if (key == "idletalk")
				mAnimatorManager.ActivateCmd((byte)(Command.Questions));
			else if (key == "idleleg")
				mAnimatorManager.ActivateCmd((byte)(Command.ByeBye));
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Question Behaviour");
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			behaviourEnd = true;
		}
	}
}