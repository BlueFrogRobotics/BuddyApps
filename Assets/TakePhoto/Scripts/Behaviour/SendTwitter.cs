using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using BuddyOS.App;
using UnityEngine.UI;
using BuddyOS;
using System.IO;

namespace BuddyApp.TakePhoto
{
	public class SendTwitter : SpeechStateBehaviour
	{

		private bool mNeedListen;
		private bool mFirst;

		bool mPressedYes;
		bool mPressedNo;

		private string mLastSpeech;
		private short mErrorCount;

		private const string mToken = "3107499058-DtOkSKQVm9aXk7g8DsT9ZyNKeixWCdQ5bnkuB5y";
		private const string mTokenSecret = "tszMyp6cFjeBb9k9raT7fxuHTCsw0g70eiMhJOmZYeJAG";
		private const string mConsumerKey = "HbjgvAlxXb4F9vPcDHKtxOC6t";
		private const string mConsumerSecret = "PQQrjxJcTs40QA9h5Rwr8rpQuoMp1J6gexgfjNXfJS8wTlC1Ey";


		private List<string> mAcceptSpeech;
		private List<string> mAnOtherSpeech;
		private List<string> mQuitSpeech;
		private List<string> mRefuseSpeech;
		private List<string> mDidntUnderstandSpeech;
		private List<string> mTweetMsg;

		private const string mHashtag = "!#BuddyCES";

		private Canvas mCanvasYesNo;
		private Canvas mCanvasBackGround;

		private AnimManager mAnimationManager;

		public override void Init()
		{
			mCanvasYesNo = GetComponentInGameObject<Canvas>(4);
			mCanvasBackGround = GetComponentInGameObject<Canvas>(7);
			mAnimationManager = GetComponentInGameObject<AnimManager>(8);
		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("OnEnter SendTwitter");
			mErrorCount = 0;
			mNeedListen = true;
			mFirst = true;

			mPressedYes = false;
			mPressedNo = false;


			Button[] buttons = mCanvasYesNo.GetComponentsInChildren<Button>();
			buttons[0].onClick.AddListener(PressedNo);
			buttons[1].onClick.AddListener(PressedYes);

			mAcceptSpeech = new List<string>();
			mAnOtherSpeech = new List<string>();
			mQuitSpeech = new List<string>();
			mRefuseSpeech = new List<string>();
			mDidntUnderstandSpeech = new List<string>();
			mTweetMsg = new List<string>();

			mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoEN.xml").text;

			if (BYOS.Instance.VocalActivation.CurrentLanguage == Language.FRA) {
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoFR.xml").text;
			}

			FillListSyn("Accept", mAcceptSpeech);
			FillListSyn("AnOther", mAnOtherSpeech);
			FillListSyn("Refuse", mRefuseSpeech);
			FillListSyn("Quit", mQuitSpeech);
			FillListSyn("DidntUnderstand", mDidntUnderstandSpeech);
			FillListSyn("TweetMsg", mTweetMsg);

			mLastSpeech = "";
			SayInLang("allowtweet", true);

			DisplayCanvasYesNo();

			// starting STT with callback
			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnPartial.Add(OnPartialRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mPressedYes) {
				mPressedYes = false;
				SayInLang("sendtweet", true);
				mTTS.Say(mHashtag, true);
				BYOS.Instance.NotManager.Display<SimpleNot>().With(mHashtag,
				BYOS.Instance.SpriteManager.GetSprite("Ico_Twitter"), Color.blue);

				SendTweet(RdmStr(mTweetMsg) + " " + mHashtag);
				animator.SetTrigger("AskMail");
			} else if (mPressedNo) {
				mPressedNo = false;
				animator.SetTrigger("AskMail");
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

								SayInLang("sendtweet", true);
								mTTS.Say(mHashtag, true);
								BYOS.Instance.NotManager.Display<SimpleNot>().With(mHashtag,
								BYOS.Instance.SpriteManager.GetSprite("Ico_Twitter"), Color.blue);

								SendTweet(RdmStr(mTweetMsg) + " " + mHashtag);

								HideCanvasYesNo();
								animator.SetTrigger("AskMail");
							} else if (ContainsOneOf(mLastSpeech, mRefuseSpeech) || ContainsOneOf(mLastSpeech, mQuitSpeech)) {
								HideCanvasYesNo();
								animator.SetTrigger("AskMail");
							} else {
								SayInLang("srynotunderstand", true);
								mTTS.Silence(1000, true);
								SayInLang("yesOrNo", true);
								mTTS.Silence(1000, true);
								SayInLang("allowtweet", true);
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

			BYOS.Instance.SoundManager.Play(SoundType.BEEP_2);
			Debug.Log("Pressed Button Yes");

			HideCanvasYesNo();
			//Stop speech
			mTTS.Silence(5, false);
			mPressedYes = true;
		}


		public void PressedNo()
		{

			BYOS.Instance.SoundManager.Play(SoundType.BEEP_2);
			Debug.Log("Pressed Button No");

			HideCanvasYesNo();
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
				HideCanvasYesNo();
			}

			mSTT.OnBestRecognition.Remove(OnSpeechRecognition);
			mSTT.OnPartial.Remove(OnPartialRecognition);
			mSTT.OnErrorEnum.Remove(ErrorSTT);

		}


		public void SendTweet(string iMsg)
		{
			Debug.Log("Sending tweet");
			byte[] bytes = File.ReadAllBytes(CommonStrings["photoPath"]);
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(bytes);

			Twitter.AccessTokenResponse accessToken = new Twitter.AccessTokenResponse();
			accessToken.Token = mToken;
			accessToken.TokenSecret = mTokenSecret;
			StartCoroutine(Twitter.API.UploadMedia(texture, iMsg, mConsumerKey, mConsumerSecret, accessToken,
													 new Twitter.PostTweetCallback(this.OnPostTweet)));
		}



		void OnPostTweet(bool success)
		{
			Debug.Log("OnPostTweet - " + (success ? "succeeded." : "failed."));
		}


		/********************** ask twitter CANVAS **********************/

		private void DisplayCanvasYesNo()
		{
			Debug.Log("Display canvas yesno");

			Text[] textObjects = mCanvasYesNo.GetComponentsInChildren<Text>();
			textObjects[0].text = mDictionary.GetString("allowtweet").ToUpper();
			textObjects[1].text = mDictionary.GetString("no").ToUpper();
			textObjects[2].text = mDictionary.GetString("yes").ToUpper();


			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
			mCanvasYesNo.GetComponent<Animator>().SetTrigger("Open_WQuestion");
		}

		private void HideCanvasYesNo()
		{
			Debug.Log("Hide canvas yesno");
			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
			mCanvasYesNo.GetComponent<Animator>().SetTrigger("Close_WQuestion");
		}
	}
}