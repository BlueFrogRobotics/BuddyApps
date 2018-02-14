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
		private VocalManager mVocalManager;

		private List <string> mKeyList;

		private bool mLaunchSTTOnce;
		private int mTimeOutCount;

		private bool mRestartSTT;
		private bool mStartSTTCoroutine;

		public bool behaviourEnd;

		public void InitBehaviour ()
		{
			mVocalManager = BYOS.Instance.Interaction.VocalManager;
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();
			mIdleBehaviour = GameObject.Find ("AIBehaviour").GetComponent<IdleBehaviour> ();
			behaviourEnd = false;

			mVocalManager.EnableTrigger = ExperienceCenterData.Instance.VoiceTrigger;
			mVocalManager.EnableDefaultErrorHandling = false;
			mVocalManager.OnEndReco = SpeechToTextCallback;
			mVocalManager.OnError = ErrorCallback;

			BYOS.Instance.Interaction.SphinxTrigger.SetThreshold (1E-24f);
			mTimeOutCount = 0;

			mTTS = BYOS.Instance.Interaction.TextToSpeech;

			BYOS.Instance.Interaction.Face.OnClickMouth.Add (MouthClicked);
			StartCoroutine (WatchSphinxTrigger ());
			StartCoroutine(ForceNeutralMood());

			InitKeyList ();
		}

		private void InitKeyList ()
		{
			mKeyList = new List<string> {
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

		private IEnumerator WatchSphinxTrigger()
		{
			mStartSTTCoroutine = true;
			while (!behaviourEnd)
			{
				if (!mVocalManager.RecognitionFinished && mStartSTTCoroutine)
				{
					OnSphinxTrigger();

					yield return new WaitForSeconds(0.5f);
					yield return new WaitUntil(() => mVocalManager.RecognitionFinished);
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

		private void OnSphinxTrigger()
		{ 
			StartCoroutine(EnableSpeechToText());

			if (ExperienceCenterData.Instance.EnableHeadMovement && !mIdleBehaviour.headPoseInit)
			{
				mAttitudeBehaviour.IsWaiting = false;
				BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
				BYOS.Instance.Interaction.BMLManager.LaunchByName("Reset01");
				mIdleBehaviour.headPoseInit = true;
			} 
		}
			
		public void SpeechToTextCallback (string iSpeech)
		{
			Debug.LogFormat ("[EXCENTER] SpeechToText : {0}", iSpeech);
			bool lClauseFound = false;
			string lKey = "";
			foreach (string lElement in mKeyList) {
				string[] lPhonetics = BYOS.Instance.Dictionary.GetPhoneticStrings (lElement);
				foreach (string lClause in lPhonetics) {
					if (iSpeech.Contains (lClause)) {
						if (ExperienceCenterData.Instance.EnableHeadMovement)
							mAttitudeBehaviour.MoveHeadWhileSpeaking (-10, 10);
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
				Debug.Log ("[EXCENTER] SpeechToText : Not Found");
			else {
				StartCoroutine (LaunchVocalCommand (lKey));
			}
			mLaunchSTTOnce = false;
		}

		private IEnumerator LaunchVocalCommand (string key)
		{
			yield return new WaitUntil (() => mTTS.HasFinishedTalking);
			if (key == "idlesee")
				mAnimatorManager.ActivateCmd ((byte)(Command.Welcome));
			else if (key == "idleleg")
				mAnimatorManager.ActivateCmd ((byte)(Command.ByeBye));
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Question Behaviour");
			BYOS.Instance.Interaction.Face.OnClickMouth.Remove (MouthClicked);

			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			behaviourEnd = true;

			mVocalManager.StopAllCoroutines ();
			this.StopAllCoroutines ();
		}

		private IEnumerator EnableSpeechToText ()
		{
			mVocalManager.EnableTrigger = false;
			mStartSTTCoroutine = false;
			mRestartSTT = true;
			mLaunchSTTOnce = false;
			while (mRestartSTT)
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
						yield return new WaitForSeconds(1.0f);
						mLaunchSTTOnce = true;
						mVocalManager.StartInstantReco(false);
					}
				}
				yield return new WaitForSeconds(0.5f);
			}
			yield return new WaitForSeconds(1.0f);
			mStartSTTCoroutine = true;
			mVocalManager.EnableTrigger = true;
		}

		public void ErrorCallback (STTError iError)
		{
			Debug.LogWarningFormat ("[EXCENTER] ERROR STT: {0}", iError.ToString ());
			mTimeOutCount++;
			if (mTimeOutCount >= 2) {
				mTimeOutCount = 0;
				mRestartSTT = false;
			} else
				mLaunchSTTOnce = false;
		}

		private IEnumerator ForceNeutralMood()
		{
			Mood buddyMood = BYOS.Instance.Interaction.Mood;
			MoodType current;
			while(!behaviourEnd)
			{
				current = buddyMood.CurrentMood;
				if (mVocalManager.RecognitionFinished && current!=MoodType.NEUTRAL)
					buddyMood.Set(MoodType.NEUTRAL);
				yield return new WaitForSeconds(0.5f);
			}
		}

		public void MouthClicked()
		{
			ExperienceCenterData.Instance.RunTrigger = true;
		}
	}
}
