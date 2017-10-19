using System;
using Buddy;
using UnityEngine;
using UnityEngine.UI;
using Buddy.UI;

namespace BuddyApp.TakePhoto
{
	public class TakePhoto : AStateMachineBehaviour
	{
		private RawImage mVideo;
		private AudioSource mPictureSound;

		private ButtonInfo mShareButton;
		private ButtonInfo mRedoButton;

		private bool ToastRender;
		private Texture2D mCameraShoot;
		private float mTimer;
		private int mSpeechId;

		public override void Start()
		{
			mShareButton = new ButtonInfo() {
				Label = "share",
				OnClick = OnButtonShare
			};

			mRedoButton = new ButtonInfo() {
				Label = "redo",
				OnClick = OnButtonRedo
			};

			Debug.Log("Init TakePhoto");
			mVideo = GetComponentInGameObject<RawImage>(0);
			mPictureSound = GetComponentInGameObject<AudioSource>(1);
			Debug.Log("Init TakePhoto done");
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			ToastRender = false;
			mTimer = 0;
			mSpeechId = 0;

			//Primitive.RGBCam.Resolution = RGBCamResolution.W_320_H_240;

			if (!Primitive.RGBCam.IsOpen)
				Primitive.RGBCam.Open(RGBCamResolution.W_640_H_480);
			mVideo.gameObject.SetActive(true);

			Interaction.TextToSpeech.SayKey("takephoto", true);

			mVideo.texture = Primitive.RGBCam.FrameTexture2D;
			Debug.Log("TakePhoto 3");
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mVideo.texture = Primitive.RGBCam.FrameTexture2D;

			if (Interaction.TextToSpeech.HasFinishedTalking) {
				//if (!mNeedExit) {
				mTimer += Time.deltaTime;

				if (mTimer > 0.5F)
					if (mSpeechId < 3) {
						Interaction.TextToSpeech.Say((3 - mSpeechId).ToString(), true);
						if (mSpeechId == 2)
							Interaction.TextToSpeech.Say("[200] cheese", true);
						mSpeechId++;
						mTimer = 0F;

					} else
						OnFinish();



				// Rejected option because of camera feed back too much hidden
				//if (!ToastRender) {

				//	// DisplayTimer
				//	Toaster.Display<CountdownToast>().With(3, OnFinish, false);
				//	ToastRender = true;

				//}
			}
		}

		private void OnFinish()
		{
			//TODO: modify resolution when we got new robot
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			mPictureSound.Play();

			// Used to get the screen image
			//Texture2D lCameraShoot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			//lCameraShoot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			//lCameraShoot.Apply();

			mCameraShoot = new Texture2D(Primitive.RGBCam.FrameTexture2D.width, Primitive.RGBCam.FrameTexture2D.height);
			Graphics.CopyTexture(Primitive.RGBCam.FrameTexture2D, mCameraShoot);
			mVideo.gameObject.SetActive(false);
			//Primitive.RGBCam.Close();

			// save file 
			string lFileName = "Buddy_" + System.DateTime.Now.Day + "day" +
				System.DateTime.Now.Month + "month" + System.DateTime.Now.Hour + "h" +
				System.DateTime.Now.Minute + "min" + System.DateTime.Now.Second + "sec.png";
			string lFilePath = "";
			//#if !UNITY_EDITOR
			//						lFilePath = "/storage/emulated/0/Pictures/" + lFileName;
			//#else
			lFilePath = Resources.PathToRaw(lFileName);
			//#endif
			Utils.SaveTextureToFile(mCameraShoot, lFilePath);
			CommonStrings["photoPath"] = lFilePath;

			//mVideo.texture = lCameraShoot;

			Trigger("AskPhotoAgain");
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("State exit");

			Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), Sprite.Create((Texture2D)mCameraShoot, new Rect(0, 0, mCameraShoot.width, mCameraShoot.height), new Vector2(0.5f, 0.5f)), mShareButton, mRedoButton);

			if (Primitive.RGBCam.IsOpen) {
				Primitive.RGBCam.Close();
			}

			Debug.Log("State exit end camera state: " + Primitive.RGBCam.IsOpen);
		}

		private void OnButtonShare()
		{
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			Play("Twitter");
		}

		private void OnButtonRedo()
		{
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			Play("TakePhoto");
		}

	}
}