using System;
using BuddyOS.App;
using BuddyOS;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.TakePhoto
{
	public class TakePhoto : SpeechStateBehaviour
	{


		private bool mNeedExit;
		private float mTimer;
		private int mSpeechId;

		private string mSpeech1;
		private string mSpeech2;
		private string mSpeech3;

		private Canvas mCanvasPhoto;
		private Canvas mCanvasTimer;
		private Canvas mCanvasYesNoPicture;
		private Canvas mCanvasBackGround;
		private RawImage mVideo;
		private RawImage mPicture;
		private AudioSource mPictureSound;



		public override void Init()
		{
			Debug.Log("Init TakePhoto");
			mCanvasYesNoPicture = GetComponentInGameObject<Canvas>(0);
			mCanvasPhoto = GetComponentInGameObject<Canvas>(1);
			mCanvasTimer = GetComponentInGameObject<Canvas>(2);
			mVideo = GetComponentInGameObject<RawImage>(5);
			mPicture = GetComponentInGameObject<RawImage>(6);
			mCanvasBackGround = GetComponentInGameObject<Canvas>(7);
			mPictureSound = GetComponentInGameObject<AudioSource>(9);
			Debug.Log("Init TakePhoto done");
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// To fix
			mRGBCam.Resolution = RGBCamResolution.W_640_H_480;
			mTimer = 0f;
			mSpeechId = 0;
			mNeedExit = false;

			if (!mRGBCam.IsOpen)
				mRGBCam.Open();
			mCanvasPhoto.gameObject.SetActive(true);

			SayInLang("takePhoto", true);
			//mTTS.Silence(1000, true);
			if (UnityEngine.Random.value > 0.9) {
				mTTS.Silence(1000, true);
				SayInLang("binaryCount", true);
				mTTS.Silence(1000, true);

				mSpeech1 = "11";
				mSpeech2 = "10";
				mSpeech3 = "1";
			} else {

				mSpeech1 = "3";
				mSpeech2 = "2";
				mSpeech3 = "1";
			}

			Debug.Log("TakePhoto 2");
			mVideo.texture = mRGBCam.FrameTexture2D;
			Debug.Log("TakePhoto 3");
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!mNeedExit) {
				Debug.Log("update 0");
				mVideo.texture = mRGBCam.FrameTexture2D;
				Debug.Log("update1");
			}
			if (mTTS.HasFinishedTalking) {

				if (!mNeedExit) {
					mTimer += Time.deltaTime;

					if (mTimer > 0.5f && mSpeechId == 3) {
						HideCanvasTimer();
					}
					if (mTimer > 1.0f) {

						if (mSpeechId == 0) {
							DisplayCanvasTimer(mSpeech1);
							mTTS.Say(mSpeech1);
						} else if (mSpeechId == 1) {
							UpdateCanvasTimer(mSpeech2);
							mTTS.Say(mSpeech2);
						} else if (mSpeechId == 2) {
							UpdateCanvasTimer(mSpeech3);
							mTTS.Say(mSpeech3);
						} else if (mSpeechId == 3) {
							mPictureSound.Play();
						} else if (mSpeechId == 4) {
							Texture2D lCameraShoot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
							lCameraShoot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
							lCameraShoot.Apply();

							mCanvasPhoto.gameObject.SetActive(false);
							mRGBCam.Close();

							// save file only on android
							string lFileName = "idtgv_" + System.DateTime.Now.Day + "day" +
								System.DateTime.Now.Month + "month" + System.DateTime.Now.Hour + "h" +
								System.DateTime.Now.Minute + "min" + System.DateTime.Now.Second + "sec.png";
							string lFilePath = "";
#if !UNITY_EDITOR
						lFilePath = "/storage/emulated/0/Pictures/" + lFileName;
#else
							lFilePath = Application.persistentDataPath + lFileName;
#endif
							Debug.Log(Application.persistentDataPath);
							BuddyTools.Utils.SaveTextureToFile(lCameraShoot, lFilePath);
							CommonStrings["photoPath"] = lFilePath;

							mVideo.texture = lCameraShoot;


							SayInLang("hereIsPic", true);
							mTTS.Silence(1000, true);
							mNeedExit = true;
						}

						mSpeechId += 1;
						mTimer = 0f;
					}

				} else {
					animator.SetTrigger("AskPhotoAgain");
				}
			}
		}


		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("State exit");

			mRGBCam.Resolution = RGBCamResolution.W_320_H_240;
			mCanvasPhoto.gameObject.SetActive(false);
			if (mRGBCam.IsOpen) {
				mRGBCam.Close();
			}

			Debug.Log("Display");
			DisplayCanvasPicture();
			Debug.Log("Picture");
			mPicture.texture = mVideo.texture;
			Debug.Log("State exit end");
		}

		/********************** TIMER CANVAS **********************/

		private void DisplayCanvasTimer(string text)
		{
			Debug.Log("Display canvas Timer " + text);
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Open_WTimer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Close_WTimer");
			mCanvasTimer.GetComponent<Animator>().SetTrigger("Open_WTimer");
			mCanvasTimer.GetComponentInChildren<Text>().text = text;
		}

		private void UpdateCanvasTimer(string text)
		{
			Debug.Log("Update canvas Timer " + text);
			mCanvasTimer.GetComponentInChildren<Text>().text = text;
		}

		private void HideCanvasTimer()
		{
			Debug.Log("Hide canvas Timer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Open_WTimer");
			mCanvasTimer.GetComponent<Animator>().ResetTrigger("Close_WTimer");
			mCanvasTimer.GetComponent<Animator>().SetTrigger("Close_WTimer");
			Debug.Log("Hide canvas Timer2");
		}

		/********************** PICTURE TAKEN CANVAS **********************/

		public void DisplayCanvasPicture()
		{
			Debug.Log("Display canvas Picture");

			Text[] textObjects = mCanvasYesNoPicture.GetComponentsInChildren<Text>();

			textObjects[0].text = mDictionary.GetString("redo").ToUpper();
			textObjects[1].text = mDictionary.GetString("no").ToUpper();
			textObjects[2].text = mDictionary.GetString("yes").ToUpper();

			mCanvasBackGround.GetComponent<Animator>().SetTrigger("Open_BG");
			mCanvasYesNoPicture.GetComponent<Animator>().SetTrigger("Open_WQuestion_Image");
		}

	}
}