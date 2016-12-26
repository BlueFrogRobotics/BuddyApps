using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BuddyOS.App;
using System;
using BuddyOS;


namespace BuddyApp.TakePhoto
{
	public class AskMail : SpeechStateBehaviour
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

		private Canvas mCanvasYesNo;
		private Canvas mCanvasBackGround;

		bool canvasDisplayed;


		public override void Init()
		{
			Debug.Log("init mail");
			mCanvasYesNo = GetComponentInGameObject<Canvas>(4);
			mCanvasBackGround = GetComponentInGameObject<Canvas>(7);
			Debug.Log("init mail done");
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mErrorCount = 0;
			mNeedListen = true;
			mFirst = true;

			mPressedYes = false;
			mPressedNo = false;

			canvasDisplayed = false;

			Button[] buttons = mCanvasYesNo.GetComponentsInChildren<Button>();
			buttons[0].onClick.AddListener(PressedNo);
			buttons[1].onClick.AddListener(PressedYes);

			mAcceptSpeech = new List<string>();
			mAnOtherSpeech = new List<string>();
			mQuitSpeech = new List<string>();
			mRefuseSpeech = new List<string>();
			mDidntUnderstandSpeech = new List<string>();

			if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA)
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoFR.xml").text;
			else
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoEN.xml").text;


			FillListSyn("Accept", mAcceptSpeech);
			FillListSyn("AnOther", mAnOtherSpeech);
			FillListSyn("Refuse", mRefuseSpeech);
			FillListSyn("Quit", mQuitSpeech);
			FillListSyn("DidntUnderstand", mDidntUnderstandSpeech);

			mLastSpeech = "";
			SayInLang("mail", true);

			// starting STT with callback
			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnPartial.Add(OnPartialRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mPressedYes) {
				animator.SetTrigger("SendMailAccepted");
			} else if (mPressedNo) {
				SayInLang("okNP", true);
				animator.SetTrigger("SendMailRefused");
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
								animator.SetTrigger("SendMailAccepted");
							} else if (ContainsOneOf(mLastSpeech, mRefuseSpeech) || ContainsOneOf(mLastSpeech, mQuitSpeech)) {
								SayInLang("okNP", true);
								animator.SetTrigger("SendMailRefused");
							} else {

								SayInLang("didntUnderstand", true);
								mTTS.Silence(1000, true);
								SayInLang("yesOrNo", true);
								mTTS.Silence(1000, true);
								SayInLang("mail", true);
								mLastSpeech = "";
								mNeedListen = true;
								if (!canvasDisplayed) {
									DisplayCanvasYesNo();
									canvasDisplayed = true;
								}
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

			//Stop speech
			mTTS.Silence(5);
			mPressedYes = true;
		}


		public void PressedNo()
		{

			BYOS.Instance.Speaker.FX.Play(FXSound.BEEP_1);
			Debug.Log("Pressed Button No");

			//Stop speech
			mTTS.Silence(5);
			mPressedNo = true;
		}


		// speech reco callback
		private void OnSpeechRecognition(string iVoiceInput)
		{

			Debug.Log("OnSpeechReco");
			mMood.Set(MoodType.NEUTRAL);
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


			// If too much erro (no answer), ask for answer. If still no answer, get back to IDLE
			if (mErrorCount > 3) {
				//			mAnimator.SetTrigger("SendMailRefused");
				Debug.Log("too much errors");
			} else {

				mMood.Set(MoodType.NEUTRAL);
				//string lSentence = RdmStr(mDidntUnderstandSpeech);
				string lSentence = mDictionary.GetString("didntUnderstand");

				switch (iError) {
					case STTError.ERROR_AUDIO: lSentence = "Il y a un problème avec le micro !"; break;
					case STTError.ERROR_NETWORK: lSentence = "Il y a un problème de connexion !"; break;
					case STTError.ERROR_RECOGNIZER_BUSY: lSentence = "La reconaissance vocale est déjà occupée !"; break;
					case STTError.ERROR_SPEECH_TIMEOUT: lSentence = "Je n'ai rien entendu. Pouvez vous répéter ?"; break;
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
			if (canvasDisplayed) {
				HideCanvasYesNo();
			}
			mMood.Set(MoodType.NEUTRAL);

			mSTT.OnBestRecognition.Remove(OnSpeechRecognition);
			mSTT.OnPartial.Remove(OnPartialRecognition);
			mSTT.OnErrorEnum.Remove(ErrorSTT);

		}

		/********************* Canvas Yes No *********************/

		public void DisplayCanvasYesNo()
		{
			Debug.Log("Display canvas yesno");

			Text[] textObjects = mCanvasYesNo.GetComponentsInChildren<Text>();

			textObjects[0].text = mDictionary.GetString("mail").ToUpper();
			textObjects[1].text = mDictionary.GetString("no").ToUpper();
			textObjects[2].text = mDictionary.GetString("yes").ToUpper();

			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
			mCanvasYesNo.GetComponent<Animator>().SetTrigger("Open_WQuestion");
		}

		public void HideCanvasYesNo()
		{
			Debug.Log("Hide canvas yesno");
			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
			mCanvasYesNo.GetComponent<Animator>().SetTrigger("Close_WQuestion");
		}

	}

}