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

		private Dictionary<string, Texture2D> mOverlaysTextures;
		private RawImage mOverlay;
		private Texture2D mOverlayTexture;
		private List<string> mOverlaysNames;

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



			mOverlaysTextures = new Dictionary<string, Texture2D>();
			mOverlaysNames = new List<String>();

			mOverlaysNames.Add("overcrazy640480");
			mOverlaysNames.Add("overfunny640480");
			mOverlaysNames.Add("overtrendy640480");
			mOverlaysNames.Add("overgrumpy640480");
			mOverlaysNames.Add("overlovely640480");
			mOverlaysNames.Add("overangry640480");


			string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
            Sprite lOverlaySprite = Resources.Load<Sprite>(lRandomSpriteName);
			mOverlaysTextures[lRandomSpriteName] = lOverlaySprite.texture;

			//Sprite lOverlaySprite = Resources.Load<Sprite>("overcrazy");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			//lOverlaySprite = Resources.Load<Sprite>("overfunny");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			//lOverlaySprite = Resources.Load<Sprite>("overtrendy");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			//lOverlaySprite = Resources.Load<Sprite>("overgrumpy");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			//lOverlaySprite = Resources.Load<Sprite>("overlovely");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			//lOverlaySprite = Resources.Load<Sprite>("overangry");
			//mOverlaysTextures.Add(lOverlaySprite.texture);

			Debug.Log("Init TakePhoto done");

			Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			ToastRender = false;
			mPhotoTaken = false;
			mTimer = 0;
			mSpeechId = 0;

			int lRandomIndice = UnityEngine.Random.Range(0, mOverlaysTextures.Count - 1);

			// Random Overlay selection
			string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
			if (!mOverlaysTextures.ContainsKey(lRandomSpriteName)) {
				Sprite lOverlaySprite = Resources.Load<Sprite>(lRandomSpriteName);
				mOverlaysTextures[lRandomSpriteName] = lOverlaySprite.texture;
			}

			mOverlayTexture = mOverlaysTextures[lRandomSpriteName];
			mOverlay.texture = mOverlayTexture;


			if (!Primitive.RGBCam.IsOpen)
				Primitive.RGBCam.Open(RGBCamResolution.W_640_H_480);
			mVideo.gameObject.SetActive(true);

			if (TakePhotoData.Instance.Overlay)
				mOverlay.gameObject.SetActive(true);

			Interaction.TextToSpeech.SayKey("takephoto", true);

			mVideo.texture = Primitive.RGBCam.FrameTexture2D;
			Debug.Log("TakePhoto 3");
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// update overlay if updated in params
			if (TakePhotoData.Instance.Overlay != mOverlay.gameObject.activeSelf)
				mOverlay.gameObject.SetActive(TakePhotoData.Instance.Overlay);
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
						if(Primitive.RGBCam.Width > 0) {
							mPictureSound.Play();
							Primitive.RGBCam.TakePhotograph(OnFinish, false);
							mPhotoTaken = true;
						} else {
							Debug.Log("RGBCAM with null!!!!!!!!!!!!!!!!!!!!");
						}
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

			mVideo.gameObject.SetActive(false);
			mOverlay.gameObject.SetActive(false);
			//Primitive.RGBCam.Close();

			// save file 
			string lFileName = "Buddy_" + System.DateTime.Now.Day + "day" +
				System.DateTime.Now.Month + "month" + System.DateTime.Now.Hour + "h" +
				System.DateTime.Now.Minute + "min" + System.DateTime.Now.Second + "sec.png";
			string lFilePath = "";
			lFilePath = Resources.GetPathToRaw(lFileName);

			// Take random Overlay


			//lOverlay.Resize(lTexture.width, lTexture.height, lOverlay.format, false);
			//lOverlay.Apply();

			if (TakePhotoData.Instance.Overlay) {
				Texture2D lTexture = iMyPhoto.Image.texture;

				var cols1 = mOverlayTexture.GetPixels();
				var cols2 = lTexture.GetPixels();

				// We scale the overlay and put it on top of the picture
				// it's ok if you don't get it ^^
				int x = 0;
				int y = 0;
				int i2 = 0;
				for (var i = 0; i < cols2.Length; ++i) {

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

			} else
				mPhotoSprite = iMyPhoto.Image;

				Utils.SaveSpriteToFile(mPhotoSprite, lFilePath);
				CommonStrings["photoPath"] = lFilePath;
				Trigger("AskPhotoAgain");
			}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("State exit");

			//if (Primitive.RGBCam.IsOpen) {
			//	Primitive.RGBCam.Close();
			//}

			Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), mPhotoSprite, mShareButton, mRedoButton);


			//Debug.Log("State exit end camera state: " + Primitive.RGBCam.IsOpen);
		}

		private void OnButtonShare()
		{
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			Play("Twitter");
		}

		private void OnButtonRedo()
		{
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			Play("Landing");
		}

	}
}