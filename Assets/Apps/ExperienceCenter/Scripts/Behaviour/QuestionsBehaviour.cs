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
		private VocalManager mVocalManager;
		private bool mLaunchSTTOnce;
		private int mTimeOutCount;

		public bool behaviourEnd;

		public void InitBehaviour ()
		{
			mVocalManager = BYOS.Instance.Interaction.VocalManager;
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			mIdleBehaviour = GameObject.Find("AIBehaviour").GetComponent<IdleBehaviour>();
			behaviourEnd = false;
			mVocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
			mVocalManager.EnableDefaultErrorHandling = false;
			mVocalManager.OnEndReco = SpeechToTextCallback;
			mVocalManager.OnError = ErrorCallback;
			BYOS.Instance.Interaction.SphinxTrigger.SetThreshold (1E-24f);
			mTimeOutCount = 0;

			// To test with the real robot
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
			BYOS.Instance.Interaction.Mood.Set (MoodType.LISTENING); 
			if (mVocalManager.EnableTrigger)
			{
				mVocalManager.EnableTrigger = false;
				StartCoroutine(EnableSpeechToText());

				if (ExperienceCenterData.Instance.EnableHeadMovement && !mIdleBehaviour.headPoseInit)
				{
					mAttitudeBehaviour.IsWaiting = false;
					Debug.LogWarning ("Stop BML");
					BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
					BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
					mIdleBehaviour.headPoseInit = true;
				} 
			}

			Debug.LogWarning ("Head Pose =" + mIdleBehaviour.headPoseInit);
		}
			
		public void SpeechToTextCallback (string iSpeech)
		{
			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);
			Debug.LogFormat ("SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string lKey = "";
			foreach (string lElement in mKeyList)
			{
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings(lElement);
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
					mTimeOutCount = 0;
					break;
				}
			}
			// Launch Vocal Command if any
			if (!lClauseFound)
				Debug.Log("SpeechToText : Not Found");
			else
			{
				StartCoroutine (LaunchVocalCommand (lKey));
			}
			mLaunchSTTOnce = false;
		}

		private IEnumerator LaunchVocalCommand (string key)
		{
			yield return new WaitUntil (() =>mTTS.HasFinishedTalking);
			if (key == "idlesee")
				mAnimatorManager.ActivateCmd((byte)(Command.Welcome));
			else if (key == "idleleg")
				mAnimatorManager.ActivateCmd((byte)(Command.ByeBye));
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Question Behaviour");
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			behaviourEnd = true;
			if (mLaunchSTTOnce)
			{
				mVocalManager.StopAllCoroutines();
				this.StopAllCoroutines();
			}
		}

		private IEnumerator EnableSpeechToText()
		{
			mLaunchSTTOnce = false;
			while (!mVocalManager.EnableTrigger)
			{
				if (!mLaunchSTTOnce)
				{
					if (!mVocalManager.RecognitionFinished)
					{
						// Recognition not finished yet, waiting until it ends cleanly
						yield return new WaitUntil(() => mVocalManager.RecognitionFinished);
					}
					else if(!mTTS.HasFinishedTalking)
					{
						// Buddy is answering, wait until he ends its sentence
						yield return new WaitUntil(() => mTTS.HasFinishedTalking);
					}
					else
					{
						// Initiating Vocal Manager instance reco
						mLaunchSTTOnce = true;
						mVocalManager.StartInstantReco(false);
					}
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		public void ErrorCallback(STTError iError)
		{
			Debug.LogWarningFormat ("ERROR STT: {0}", iError.ToString ());
			BYOS.Instance.Interaction.Mood.Set (MoodType.NEUTRAL);
			mTimeOutCount++;
			if (mTimeOutCount > 2)
			{
				mTimeOutCount = 0;
				mVocalManager.EnableTrigger = true;
			}
			else
				mLaunchSTTOnce = false;
		}
	}
}