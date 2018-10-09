using System;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.TakePhoto
{
    // Voir si on peut remplacer par un Shared take photo après


	public sealed class TakePhoto : AStateMachineBehaviour
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
        private TakePhotoData mTakePhotoBH;
        private Mat mMatSrc;
        private bool mIsFrameCaptured;

        private XMLData mXMLData;
        private Publish mWhereToPublish;

        private const int MAXLISTENNINGITER = 3;
        private int mNumberListen;

        public override void Start()
		{
            mXMLData = new XMLData();
            mNumberListen = 0;
            mIsFrameCaptured = false;
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
            mMatSrc = new Mat();
            mMat = new Mat();

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_640_H_480;
			ToastRender = false;
			mPhotoTaken = false;
			mTimer = 0;
			mSpeechId = 0;

            //int lRandomIndice = UnityEngine.Random.Range(0, mOverlaysTextures.Count - 1);

            // Random Overlay selection
			string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
            Debug.Log("RANDOM SPRITE NAME : " + lRandomSpriteName);
			if (!mOverlaysTextures.ContainsKey(lRandomSpriteName)) {
				Sprite lOverlaySprite = Buddy.Resources.Get<Sprite>(lRandomSpriteName);
                if (lOverlaySprite == null)
                    Debug.Log("SPRITE NULL");
				mOverlaysTextures[lRandomSpriteName] = lOverlaySprite.texture;
                
			}
            mOverlayTexture = mOverlaysTextures[lRandomSpriteName];
            mOverlay.texture = mOverlayTexture;
            if (Buddy.Sensors.HDCamera.IsBusy)
            {
                Buddy.Sensors.HDCamera.Close();
            }

            if (!Buddy.Sensors.HDCamera.IsOpen)
            {
                Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
                
            }
            mVideo.gameObject.SetActive(true);
            if (TakePhotoData.Instance.Overlay)
				mOverlay.gameObject.SetActive(true);
            Buddy.Vocal.SayKey("takephoto", true);
            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            mMatSrc = Buddy.Sensors.HDCamera.Frame;
		}

        private void OnFrameCaptured(Mat iInput)
        {
            mMatSrc = iInput;
            Core.flip(mMatSrc, mMat, 1);
            mVideo.texture = Utils.MatToTexture2D(mMat);
            mIsFrameCaptured = true;
        }

        
		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            if(mIsFrameCaptured)
            {
               
                // update overlay if updated in params
                if (TakePhotoData.Instance.Overlay != mOverlay.gameObject.activeSelf)
                    mOverlay.gameObject.SetActive(TakePhotoData.Instance.Overlay);

                Mat mMatSrc = Buddy.Sensors.HDCamera.Frame;
                Core.flip(mMatSrc, mMat, 1);
                mVideo.texture = Utils.MatToTexture2D(mMat);

                if (!Buddy.Vocal.IsSpeaking)
                {
                    mTimer += Time.deltaTime;

                    if (mTimer > 0.5F)
                        if (mSpeechId < 3)
                        {
                            Buddy.Vocal.Say((3 - mSpeechId).ToString(), true);
                            if (mSpeechId == 2)
                                Buddy.Vocal.Say("[200] cheese", true);
                            mSpeechId++;
                            mTimer = 0F;

                        }
                        else if (!mPhotoTaken)
                        {
                            if (Buddy.Sensors.HDCamera.Width > 0)
                            {
                                //mPictureSound.Play();
                                Buddy.Sensors.HDCamera.TakePhotograph(OnFinish, false);
                                Debug.Log("PHOTO TAKEN");
                                mPhotoTaken = true; 
                            }
                            else
                            {
                                Debug.Log("RGBCAM with null!");
                            }
                        }
                }
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
            Buddy.Sensors.HDCamera.Close();
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
            {
                mPhotoSprite = iMyPhoto.Image;
            }
				

			Utils.SaveSpriteToFile(mPhotoSprite, lFilePath);
            TakePhotoData.Instance.PhotoPath = lFilePath;
            mIsFrameCaptured = false;
            //Action mOnClick;
            //mOnClick = () => DialogerToast();
            //Toaster.Display<PictureToast>().With(Dictionary.GetString("redoorshare"), mPhotoSprite, mShareButton, mRedoButton);
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetRandomString("redoorshare"), null,  iInput => { OnEndListening(iInput); });
            Buddy.GUI.Toaster.Display<PictureToast>().With(mPhotoSprite/*, mOnClick*/);
            FButton lLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            FButton LRightButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_share"));
            LRightButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_redo"));
            //LRightButton.SetBackgroundColor(Color.red);
            //lLeftButton.SetBackgroundColor(Color.green);
            lLeftButton.OnClick.Add(() => { OnButtonShare(); });
            LRightButton.OnClick.Add(() => { OnButtonRedo(); });

            //TEST AVANT PUSH DE VALENTIN
            //lLeftButton.OnClick.Add(() => { OnButtonRedo(); });
            //LRightButton.OnClick.Add(() => { OnButtonShare(); });

            //Trigger("AskPhotoAgain");
        }

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mIsFrameCaptured = false;
        }

        private void OnEndListening(SpeechInput iInput)
        {
            if (ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, Buddy.Resources.GetPhoneticStrings("share")))
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.GUI.Footer.Hide();
                Play("Twitter");
            }
            else if (ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, Buddy.Resources.GetPhoneticStrings("redo")))
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.GUI.Footer.Hide();
                Play("Landing");
            }
            else
            {
                if (mNumberListen < MAXLISTENNINGITER)
                {
                    // if the human answer is outside of planned sentences, we increment the
                    // number of listen and we listen again.
                    mNumberListen++;
                    Buddy.Vocal.Listen(
                        iInputRec => { OnEndListening(iInputRec); }
                        );
                }
                else
                {
                    // If we launch the listen too many times, it's like a timeout and
                    // we get back to the menu
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();

                }
            }
        }

        private void OnButtonShare()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            Buddy.Vocal.StopListening();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Play("Twitter");
		}

		private void OnButtonRedo()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            Buddy.Vocal.StopListening();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Play("Landing");
		}
        public static bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
            if(!string.IsNullOrEmpty(iSpeech))
            {
                for (int i = 0; i < iListSpeech.Length; ++i)
                {
                    string[] lWords = iListSpeech[i].Split(' ');
                    if (lWords.Length < 2 && !string.IsNullOrEmpty(lWords[0]))
                    {
                        if (lWords[0].ToLower() == iSpeech.ToLower())
                        {
                            return true;
                        }
                    }
                    else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                        return true;
                }
            }
            return false;
        }

    }
}