using UnityEngine;
using BuddyOS.App;
using BuddyFeature.Vision;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.TakePhoto
{
	[RequireComponent(typeof(FaceCascadeTracker))]
	public class VocalCommand : SpeechStateBehaviour
	{
		private bool mFollowFace;
		private bool mFirst;
		private bool mNeedListen;
		private int mRGBCamWidthCenter;
		private int mRGBCamHeightCenter;

		private string mLastSpeech;

		private float mHeadYesAngle;
		private float mHeadNoAngle;

		private BuddyMotion mLastMotion;

		private FaceCascadeTracker mFaceTracker;
		private List<OpenCVUnity.Rect> mTrackedObjects;
		private GameObject mCanvasHeadControl;
		private Motors mMotors;

		private List<string> mUpSpeech;
		private List<string> mDownSpeech;
		private List<string> mLeftSpeech;
		private List<string> mRightSpeech;
		private List<string> mForwardSpeech;
		private List<string> mBackSpeech;
		private List<string> mMoreSpeech;
		public List<string> mAgainSpeech;
		private List<string> mPhotoSpeech;

		public enum BuddyMotion : int
		{
			HEAD_LEFT,
			HEAD_RIGHT,
			HEAD_DOWN,
			HEAD_UP,
			WHEEL_LEFT,
			WHEEL_RIGHT,
            WHEEL_BACK,
			WHEEL_FORWARD
		}


		//TODO move robot's head according to user vocal command, until it finds a head

		public override void Init()
		{
			mFollowFace = false;
			mMotors = BYOS.Instance.Motors;
			mCanvasHeadControl = GetGameObject(10);
		}

		protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mFaceTracker = GetComponent<FaceCascadeTracker>();
			DisplayCanvasHeadControl();

			mNeedListen = true;
			mFirst = true;

			mUpSpeech = new List<string>();
			mDownSpeech = new List<string>();
			mLeftSpeech = new List<string>();
			mRightSpeech = new List<string>();
			mAgainSpeech = new List<string>();
			mMoreSpeech = new List<string>();
			mBackSpeech = new List<string>();
			mForwardSpeech = new List<string>();
			mPhotoSpeech = new List<string>();

			if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA)
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoFR.xml").text;
			else
				mSynonymesFile = Resources.Load<TextAsset>("Lang/synonymesPhotoEN.xml").text;


			FillListSyn("Up", mUpSpeech);
			FillListSyn("Down", mDownSpeech);
			FillListSyn("Left", mLeftSpeech);
			FillListSyn("Right", mRightSpeech);
			FillListSyn("Back", mBackSpeech);
			FillListSyn("Forward", mForwardSpeech);
			FillListSyn("Photo", mPhotoSpeech);
			FillListSyn("More", mMoreSpeech);
			FillListSyn("Again", mAgainSpeech);

			mLastSpeech = "";
			mTTS.SayKey("vocalmotion", true);
			Debug.Log("key vocalmotion: " + mDictionary.GetString("vocalmotion"));

			// starting STT with callback
			mSTT.OnBestRecognition.Add(OnSpeechRecognition);
			mSTT.OnPartial.Add(OnPartialRecognition);
			mSTT.OnErrorEnum.Add(ErrorSTT);

		}

		protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if (mFollowFace) {
				// Center the face before escape
				mTrackedObjects = mFaceTracker.TrackedObjects;

				if (mTrackedObjects.Count > 0) {
					float lXCenter = 0.0f;
					float lYCenter = 0.0f;
					for (int i = 0; i < mTrackedObjects.Count; ++i) {
						lXCenter = mTrackedObjects[i].x + mTrackedObjects[i].width / 2;
						lYCenter = mTrackedObjects[i].y + mTrackedObjects[i].height / 2;
					}
					lXCenter = lXCenter / mTrackedObjects.Count;
					lYCenter = lYCenter / mTrackedObjects.Count;


					Debug.Log("Tracking face : XCenter " + lXCenter + " / YCenter " + lYCenter);


					if (!(mRGBCamWidthCenter - 25 < lXCenter && lXCenter < mRGBCamWidthCenter + 5))
						mHeadNoAngle -= Mathf.Sign(lXCenter - mRGBCamWidthCenter) * 1.5F;
					if (!(mRGBCamHeightCenter - 5 < lYCenter && lYCenter < mRGBCamHeightCenter + 25))
						mHeadYesAngle += Mathf.Sign(lYCenter - mRGBCamHeightCenter) * 1.5F;

					Debug.Log("Setting angles Yes : " + Mathf.Sign(lYCenter - mRGBCamHeightCenter) +
						" / No : " + Mathf.Sign(lXCenter - mRGBCamWidthCenter));

					mYesHinge.SetPosition(mHeadYesAngle);
					mNoHinge.SetPosition(mHeadNoAngle);


					if ((mRGBCamWidthCenter - 25 < lXCenter && lXCenter < mRGBCamWidthCenter + 5) && 
						(mRGBCamHeightCenter - 5 < lYCenter && lYCenter < mRGBCamHeightCenter + 25)) {

						Debug.Log("Face centered");
						iAnimator.SetTrigger("Photo");
					}

				}

			} else {
				if (mFaceTracker.TrackedObjects.Count > 0) {
					//Exit, vocal command
					Debug.Log("face found!");
					HideCanvasHeadControl();
					mHeadNoAngle = mNoHinge.CurrentAnglePosition;
					mHeadYesAngle = mYesHinge.CurrentAnglePosition;
					mRGBCamWidthCenter = BYOS.Instance.RGBCam.Width / 2;
					mRGBCamHeightCenter = BYOS.Instance.RGBCam.Height / 2;
					mFollowFace = true;
				} else {
					//move head according to vocal command
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
								if (ContainsOneOf(mLastSpeech, mUpSpeech)) {
									ControlBuddy(BuddyMotion.HEAD_UP);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mDownSpeech)) {
									ControlBuddy(BuddyMotion.HEAD_DOWN);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mRightSpeech)) {
									ControlBuddy(BuddyMotion.HEAD_LEFT);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mRightSpeech)) {
									ControlBuddy(BuddyMotion.HEAD_RIGHT);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mBackSpeech)) {
									ControlBuddy(BuddyMotion.WHEEL_BACK);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mForwardSpeech)) {
									ControlBuddy(BuddyMotion.WHEEL_FORWARD);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mMoreSpeech) || ContainsOneOf(mLastSpeech, mAgainSpeech)) {
									ControlBuddy(mLastMotion);
									mLastSpeech = "";
									mNeedListen = true;
								} else if (ContainsOneOf(mLastSpeech, mPhotoSpeech)) {
									Debug.Log("User asked to take photo");
									iAnimator.SetTrigger("Photo");
								} else {

									mTTS.SayKey("didntUnderstand", true);
									mTTS.Silence(1000, true);
									mTTS.SayKey("vocalmotion", true);
									mLastSpeech = "";
									mNeedListen = true;
								}
								Debug.Log("LastAns != previous end");
							}
						}
					}
				}
			}
		}




		// speech reco callback
		private void OnSpeechRecognition(string iVoiceInput)
		{

			Debug.Log("OnSpeechReco");
			mMood.Set(MoodType.NEUTRAL);
			Debug.Log("[photo Heard] : " + iVoiceInput);

			// set active Answer in Dialog
			mLastSpeech = iVoiceInput;
		}


		private void OnPartialRecognition(string iVoiceInput)
		{
			Debug.Log("OnPartialReco");
			Debug.Log("[photo Partial Reco] : " + iVoiceInput);
			mMood.Set(MoodType.NEUTRAL);

			// set active Answer in Dialog
			mLastSpeech = iVoiceInput;
		}



		void ErrorSTT(STTError iError)
		{
			Debug.Log("[question error] : " + iError);


			mMood.Set(MoodType.NEUTRAL);
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

			mLastSpeech = "";
			mNeedListen = true;
		}








		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}


		private void ControlBuddy(BuddyMotion iMotion)
		{
			mLastMotion = iMotion;
            float lNoAngle = 0.0f;
			float lYesAngle = 0.0f;
			if (iMotion == BuddyMotion.HEAD_LEFT) {
				lNoAngle = mMotors.NoHinge.CurrentAnglePosition + 5.0f;
			} else if (iMotion == BuddyMotion.HEAD_RIGHT) {
				lNoAngle = mMotors.NoHinge.CurrentAnglePosition - 5.0f;
			} else if (iMotion == BuddyMotion.HEAD_DOWN) {
				lYesAngle = mMotors.YesHinge.CurrentAnglePosition + 5.0f;
			} else if (iMotion == BuddyMotion.HEAD_UP) {
				lYesAngle = mMotors.YesHinge.CurrentAnglePosition - 5.0f;
			}
			mMotors.NoHinge.SetPosition(lNoAngle, 100.0f);
			mMotors.YesHinge.SetPosition(lYesAngle, 100.0f);
		}



		/********************* Canvas Head control *********************/

		public void DisplayCanvasHeadControl()
		{
			Debug.Log("Display canvas HeadControl");

			//mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
			mCanvasHeadControl.GetComponent<Animator>().SetTrigger("Open_WHeadController");
		}

		public void HideCanvasHeadControl()
		{
			Debug.Log("Hide canvas HeadControl");
			//mCanvasBackGround.GetComponent<Animator>().SetTrigger("Close_BG");
			mCanvasHeadControl.GetComponent<Animator>().SetTrigger("Close_WHeadController");
		}


	}
}
