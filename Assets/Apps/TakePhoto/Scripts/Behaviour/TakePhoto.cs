using System;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.TakePhoto
{
    // Voir si on peut remplacer par un Shared take photo après


	public class TakePhoto : AStateMachineBehaviour
	{
		private RawImage mVideo;
		private AudioSource mPictureSound;

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
		private Mat mMat;
        private TakePhotoBehaviour mTakePhotoBH;
        
        

		public override void Start()
		{
            //Mettre le mtakePhotoBH = GetcomponentInGameObject<TakePhotoBehaviour>();
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
			Sprite lOverlaySprite = Buddy.Resources.Get<Sprite>(lRandomSpriteName);
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

			//Buddy.Sensors.HDCamera.Mode = HDCameraMode.COLOR_640x480_30FPS_RGB;
			mMat = new Mat();

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			ToastRender = false;
			mPhotoTaken = false;
			mTimer = 0;
			mSpeechId = 0;

            //int lRandomIndice = UnityEngine.Random.Range(0, mOverlaysTextures.Count - 1);

            // Random Overlay selection
            if (mOverlaysNames.Count <= 0)
                Debug.Log("OVERLAYNAMES NULL");
			string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
			if (!mOverlaysTextures.ContainsKey(lRandomSpriteName)) {
				Sprite lOverlaySprite = Buddy.Resources.Get<Sprite>(lRandomSpriteName);
                if (lOverlaySprite == null)
                    Debug.Log("SPRITE NULL");
				mOverlaysTextures[lRandomSpriteName] = lOverlaySprite.texture;
			}

			mOverlayTexture = mOverlaysTextures[lRandomSpriteName];
			mOverlay.texture = mOverlayTexture;


			if (!Buddy.Sensors.RGBCamera.IsOpen)
				Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
			mVideo.gameObject.SetActive(true);

			if (TakePhotoData.Instance.Overlay)
				mOverlay.gameObject.SetActive(true);

			Buddy.Vocal.SayKey("takephoto", true);

			Mat mMatSrc = Buddy.Sensors.RGBCamera.Frame;
			Core.flip(mMatSrc, mMat, 1);
            if (mMat.empty())
                Debug.Log("takephoto on state enter : MAT NULL ");
			mVideo.texture = Utils.MatToTexture2D(mMat);
			Debug.Log("TakePhoto 3");
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// update overlay if updated in params
			if (TakePhotoData.Instance.Overlay != mOverlay.gameObject.activeSelf)
				mOverlay.gameObject.SetActive(TakePhotoData.Instance.Overlay);

			Mat mMatSrc = Buddy.Sensors.RGBCamera.Frame;
            Core.flip(mMatSrc, mMat, 1);
			mVideo.texture = Utils.MatToTexture2D(mMat);

			if (!Buddy.Vocal.IsSpeaking) {
				//if (!mNeedExit) {
				mTimer += Time.deltaTime;

				if (mTimer > 0.5F)
					if (mSpeechId < 3) {
						Buddy.Vocal.Say((3 - mSpeechId).ToString(), true);
						if (mSpeechId == 2)
							Buddy.Vocal.Say("[200] cheese", true);
						mSpeechId++;
						mTimer = 0F;

					} else if (!mPhotoTaken) {
						if (Buddy.Sensors.RGBCamera.Width > 0) {
							mPictureSound.Play();
							Buddy.Sensors.HDCamera.TakePhotograph(OnFinish, false);
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

		Texture2D FlipTexture(Texture2D iOriginal)
		{
			Texture2D lFlipped = new Texture2D(iOriginal.width, iOriginal.height);

			int xN = iOriginal.width;
			int yN = iOriginal.height;


			for (int i = 0; i < xN; i++) {
				for (int j = 0; j < yN; j++) {
					lFlipped.SetPixel(xN - i - 1, j, iOriginal.GetPixel(i, j));
				}
			}
			lFlipped.Apply();

			return lFlipped;
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
			lFilePath = Buddy.Resources.GetRawFullPath(lFileName);

			// Take random Overlay


			//lOverlay.Resize(lTexture.width, lTexture.height, lOverlay.format, false);
			//lOverlay.Apply();

			if (TakePhotoData.Instance.Overlay) {

				Texture2D lTexture = FlipTexture(iMyPhoto.Image.texture);

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
				mPhotoSprite = Sprite.Create(lTexture, new UnityEngine.Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5F, 0.5F));

			} else
				mPhotoSprite = iMyPhoto.Image;

			Utils.SaveSpriteToFile(mPhotoSprite, lFilePath);
            mTakePhotoBH.PhotoPath = lFilePath;
			//CommonStrings["photoPath"] = lFilePath;
			Trigger("AskPhotoAgain");
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("State exit");

            //if (Primitive.RGBCam.IsOpen) {
            //	Primitive.RGBCam.Close(); 
            //}
            Action mOnClick;

            mOnClick = () => DialogerToast();
			//Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), mPhotoSprite, mShareButton, mRedoButton);
            Buddy.GUI.Toaster.Display<PictureToast>().With(mPhotoSprite, mOnClick);


            //mShareButton = new ButtonInfo()
            //{
            //    Label = Dictionary.GetString("share"),
            //    OnClick = OnButtonShare
            //};

            //mRedoButton = new ButtonInfo()
            //{
            //    Label = Dictionary.GetString("redo"),
            //    OnClick = OnButtonRedo
            //};

            //Debug.Log("State exit end camera state: " + Primitive.RGBCam.IsOpen);
        }

        private void DialogerToast()
        {
            Buddy.GUI.Toaster.Hide();
            if(!Buddy.GUI.Toaster.IsBusy)
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
                {
                    iBuilder.CreateWidget<TText>().SetLabel("Script : TakePhoto display");
                },
                () => { OnButtonShare(); }, "Share script : Takephoto",
                () => { OnButtonRedo(); } , "Redo script : Takephoto"
                );
            }
        }

		private void OnButtonShare()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);

			Play("Twitter");
		}

		private void OnButtonRedo()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
			Play("Landing");
		}

	}
}