using System;
using Buddy;
using UnityEngine;
using UnityEngine.UI;
using Buddy.UI;
using System.Collections.Generic;

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
		private Sprite mPhotoSprite;
		private bool mPhotoTaken;

		private List<string> mOverlaysName;
		private RawImage mOverlay;
		private Texture2D mOverlayTexture;

		public override void Start()
		{
			mShareButton = new ButtonInfo() {
				Label = Dictionary.GetString("share"),
				OnClick = OnButtonShare
			};

			mRedoButton = new ButtonInfo() {
				Label = Dictionary.GetString("redo"),
				OnClick = OnButtonRedo
			};

			Debug.Log("Init TakePhoto");
			mVideo = GetComponentInGameObject<RawImage>(0);
			mPictureSound = GetComponentInGameObject<AudioSource>(1);
			mOverlay = GetComponentInGameObject<RawImage>(2);


			Debug.Log("Init TakePhoto done");

			mOverlaysName = new List<string>();
			mOverlaysName.Add("overcrazy");
			mOverlaysName.Add("overangry");
			mOverlaysName.Add("overfunny");
			mOverlaysName.Add("overtrendy");
			mOverlaysName.Add("overgrumpy");
			mOverlaysName.Add("overlovely");

			
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			ToastRender = false;
			mPhotoTaken = false;
			mTimer = 0;
			mSpeechId = 0;

			//Primitive.RGBCam.Resolution = RGBCamResolution.W_320_H_240;

			// Random Overlay selection
			Sprite lOverlaySprite = Resources.Load<Sprite>(mOverlaysName[UnityEngine.Random.Range(0, mOverlaysName.Count - 1)]);
			mOverlayTexture = new Texture2D(lOverlaySprite.texture.width, lOverlaySprite.texture.height);
			mOverlayTexture = lOverlaySprite.texture;


			mOverlay.texture = mOverlayTexture;


			if (!Primitive.RGBCam.IsOpen)
				Primitive.RGBCam.Open(RGBCamResolution.W_640_H_480);
			mVideo.gameObject.SetActive(true);
			mOverlay.gameObject.SetActive(true);

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

					} else if (!mPhotoTaken) {
						mPictureSound.Play();
						Primitive.RGBCam.TakePhotograph(OnFinish, false);
						mPhotoTaken = true;
					}



				// Rejected option because of camera feed back too much hidden
				//if (!ToastRender) {

				//	// DisplayTimer
				//	Toaster.Display<CountdownToast>().With(3, OnFinish, false);
				//	ToastRender = true;

				//}
			}
		}

		private void OnFinish(Photograph iMyPhoto)
		{

			// Used to get the screen image
			//Texture2D lCameraShoot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			//lCameraShoot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			//lCameraShoot.Apply();

			Debug.Log("finish 1");

			//mPhotoSprite = iMyPhoto.Image;
			//mCameraShoot = new Texture2D(Primitive.RGBCam.FrameTexture2D.width, Primitive.RGBCam.FrameTexture2D.height);
			//Graphics.CopyTexture(Primitive.RGBCam.FrameTexture2D, mCameraShoot);

			// TODO remove this when os fixed
			Texture2D lTexture = new Texture2D(iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height);
			Graphics.CopyTexture(iMyPhoto.Image.texture, lTexture);
			mVideo.gameObject.SetActive(false);
			mOverlay.gameObject.SetActive(false);
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

			Utils.SaveTextureToFile(lTexture, lFilePath);

			// Take random Overlay
			

			//lOverlay.Resize(lTexture.width, lTexture.height, lOverlay.format, false);
			//lOverlay.Apply();
			//Debug.Log("finish 3.1 " + lOverlay + " \n format " + lOverlay.format.ToString());
			//Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), Sprite.Create(lOverlay, new Rect(0, 0, lOverlay.width, lOverlay.height), new Vector2(0.5F, 0.5F)), mShareButton, mRedoButton);

			var cols1 = mOverlayTexture.GetPixels();
			var cols2 = lTexture.GetPixels();

			// We scale the overlay and put it on top of the picture
			// it's ok if you don't get it ^^
			int x = 0;
			int y = 0;
			int i2 = 0;
			for (var i = 0; i <cols2.Length; ++i) {
				
					x = i % lTexture.width;
					y = i / lTexture.width;
					i2 = (x * mOverlayTexture.width / lTexture.width) + (y * mOverlayTexture.height / lTexture.height) * mOverlayTexture.width;
				if (cols1[i2].a != 0) {
					cols2[i] = (1 - cols1[i2].a) * cols2[i] + cols1[i2].a * cols1[i2];
				}
			}

			lTexture.SetPixels(cols2);
			lTexture.Apply();
			mPhotoSprite = Sprite.Create(lTexture, new Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5F, 0.5F));

			Utils.SaveSpriteToFile(mPhotoSprite, Resources.PathToRaw("Overlay" + lFileName));

			Trigger("AskPhotoAgain");
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("State exit");

			if (Primitive.RGBCam.IsOpen) {
				Primitive.RGBCam.Close();
			}

			Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), mPhotoSprite, mShareButton, mRedoButton);


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