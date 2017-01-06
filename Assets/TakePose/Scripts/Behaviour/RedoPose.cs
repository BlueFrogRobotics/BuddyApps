﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using BuddyOS.App;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.TakePose
{
	public class RedoPose : SpeechStateBehaviour
	{

		private bool mNeedListen;
		private bool mFirst;

		bool mPressedYes;
		bool mPressedNo;

		private string mLastSpeech;
		private short mErrorCount;

		private List<string> mAcceptSpeech;
		private List<string> mAnOtherSpeech;
		private List<string> mQuitSpeech;
		private List<string> mRefuseSpeech;
		private List<string> mDidntUnderstandSpeech;

		private Canvas mCanvasQuestion;
		private Canvas mCanvasBackGround;

		private AnimManager mAnimationManager;

		public override void Init()
		{
			mAnimationManager = GetComponentInGameObject<AnimManager>(1);
			mCanvasQuestion = GetComponentInGameObject<Canvas>(2);
			mCanvasBackGround = GetComponentInGameObject<Canvas>(3);
		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mErrorCount = 0;
			mNeedListen = true;
			mFirst = true;

			mPressedYes = false;
			mPressedNo = false;


			Button[] buttons = mCanvasQuestion.GetComponentsInChildren<Button>();
			buttons[0].onClick.AddListener(PressedNo);
			buttons[1].onClick.AddListener(PressedYes);

			mAcceptSpeech = new List<string>();
			mAnOtherSpeech = new List<string>();
			mQuitSpeech = new List<string>();
			mRefuseSpeech = new List<string>();
			mDidntUnderstandSpeech = new List<string>();

			mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPoseEN.xml").text;

			if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA) {
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPoseFR.xml").text;
			}

			FillListSyn("Accept", mAcceptSpeech);
			FillListSyn("AnOther", mAnOtherSpeech);
			FillListSyn("Refuse", mRefuseSpeech);
			FillListSyn("Quit", mQuitSpeech);
			FillListSyn("DidntUnderstand", mDidntUnderstandSpeech);

			mLastSpeech = "";
			SayInLang("redoPose", true);

			// starting STT with callback
			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnPartial.Add(OnPartialRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mPressedYes) {
				animator.SetTrigger("Pose");
			} else if (mPressedNo) {
				SayInLang("noRedo");
				animator.SetTrigger("Exit");
			} else {
				if (mTTS.HasFinishedTalking) {
					if ((mSTT.HasFinished || mFirst) && mNeedListen) {
						Debug.Log("pre listen");
						mSTT.Request();
						mMood.Set(MoodType.LISTENING);
						mNeedListen = false;
						mFirst = false;
						Debug.Log("post listen");
					} else if (mSTT.HasFinished && !mNeedListen) {
						Debug.Log("pre deal with speech");

						if (string.IsNullOrEmpty(mLastSpeech)) {
							mNeedListen = true;
							Debug.Log("LastAns null");

						} else {
							Debug.Log("LastAns != previous");
							if (ContainsOneOf(mLastSpeech, mAcceptSpeech) || ContainsOneOf(mLastSpeech, mAnOtherSpeech)) {
								animator.SetTrigger("Pose");
							} else if (ContainsOneOf(mLastSpeech, mRefuseSpeech) || ContainsOneOf(mLastSpeech, mQuitSpeech)) {
								SayInLang("noRedo");
								animator.SetTrigger("Exit");
							} else {
								SayInLang("srynotunderstand", true);
								mTTS.Silence(1000, true);
								SayInLang("yesOrNo", true);
								mTTS.Silence(1000, true);
								SayInLang("redoPose", true);
								mLastSpeech = "";
								mNeedListen = true;
							}
							Debug.Log("LastAns != previous end");
						}
					}

				}
			}
		}


		public void PressedYes()
		{


			BYOS.Instance.Speaker.FX.Play(FXSound.BEEP_1);
			Debug.Log("Pressed Button Yes");

			HideCanvasQuestion();
			//Stop speech
			mTTS.Silence(5, false);
			mPressedYes = true;
		}


		public void PressedNo()
		{

			BYOS.Instance.Speaker.FX.Play(FXSound.BEEP_1);
			Debug.Log("Pressed Button No");

			HideCanvasQuestion();
			//Stop speech
			mTTS.Silence(5, false);
			mPressedNo = true;
		}


		// speech reco callback
		private void OnSpeechRecognition(string iVoiceInput)
		{

			Debug.Log("OnSpeechReco");
			mMood.Set(MoodType.NEUTRAL);
			mAnimationManager.Blink();
			Debug.Log("[photo Heard] : " + iVoiceInput);

			mErrorCount = 0;
			// set active Answer in Dialog
			mLastSpeech = iVoiceInput;
		}


		private void OnPartialRecognition(string iVoiceInput)
		{
			Debug.Log("OnPartialReco");
			Debug.Log("[photo Partial Reco] : " + iVoiceInput);
			mMood.Set(MoodType.NEUTRAL);

			mErrorCount = 0;
			// set active Answer in Dialog
			mLastSpeech = iVoiceInput;
		}



		void ErrorSTT(STTError iError)
		{
			Debug.Log("[question error] : " + iError);
			++mErrorCount;
			Debug.Log("[question error] : count " + mErrorCount);
			mAnimationManager.Sigh();

			// If too much erro (no answer), ask for answer. If still no answer, get back to IDLE
			if (mErrorCount > 3) {
				//			mAnimator.SetTrigger("AskMail");
				Debug.Log("too much errors");
			} else {
				mMood.Set(MoodType.NEUTRAL);
				//string lSentence = RdmStr(mDidntUnderstandSpeech);
				string lSentence = mDictionary.GetString("srynotunderstand");

				switch (iError) {
					case STTError.ERROR_AUDIO: lSentence = mDictionary.GetString("micissue"); break;
					case STTError.ERROR_NETWORK: lSentence = mDictionary.GetString("connectissue"); break;
					case STTError.ERROR_RECOGNIZER_BUSY: lSentence = mDictionary.GetString("vrecobusy"); break;
					case STTError.ERROR_SPEECH_TIMEOUT: lSentence = mDictionary.GetString("hearnothing") + " " + mDictionary.GetString("repeatPls"); break;
				}

				if (UnityEngine.Random.value > 0.8) {
					mMood.Set(MoodType.SAD);
					mTTS.Say(lSentence);
				}
			}

			mLastSpeech = "";
			mNeedListen = true;
		}




		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mMood.Set(MoodType.NEUTRAL);

			if (!mPressedYes && !mPressedNo) {
				HideCanvasQuestion();
			}

			mSTT.OnBestRecognition.Remove(OnSpeechRecognition);
			mSTT.OnPartial.Remove(OnPartialRecognition);
			mSTT.OnErrorEnum.Remove(ErrorSTT);

		}

		/********************** question CANVAS **********************/

		public void DisplayCanvasQuestion()
		{
			Debug.Log("Display canvas Picture");

			Text[] textObjects = mCanvasQuestion.GetComponentsInChildren<Text>();

			textObjects[0].text = mDictionary.GetString("redoPose").ToUpper();
			textObjects[1].text = mDictionary.GetString("no").ToUpper();
			textObjects[2].text = mDictionary.GetString("yes").ToUpper();

			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
			mCanvasQuestion.GetComponent<Animator>().SetTrigger("Open_WQuestion");
		}

		public void HideCanvasQuestion()
		{
			Debug.Log("Hide canvas Picture");
			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
			mCanvasQuestion.GetComponent<Animator>().SetTrigger("Close_WQuestion");
		}

	}
}