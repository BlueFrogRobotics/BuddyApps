using System;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;
using System.Collections.Generic;
using OpenCVUnity;
using System.IO;

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
        
        private Publish mWhereToPublish;

        private const int MAXLISTENNINGITER = 3;
        private int mNumberListen;

        public override void Start()
		{
            mNumberListen = 0;
            mIsFrameCaptured = false;
			mVideo = GetComponentInGameObject<RawImage>(0);
			mPictureSound = GetComponentInGameObject<AudioSource>(1);
			mOverlay = GetComponentInGameObject<RawImage>(2);

            mOverlaysNames = new List<String>();

            mOverlaysNames.Add("overcrazy640480");
            mOverlaysNames.Add("overfunny640480");
            mOverlaysNames.Add("overtrendy640480");
            mOverlaysNames.Add("overgrumpy640480");
            mOverlaysNames.Add("overlovely640480");
            mOverlaysNames.Add("overangry640480");

            mMatSrc = new Mat();
            mMat = new Mat();

		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            //Init 
            ToastRender = false;
			mPhotoTaken = false;
			mTimer = 0;
			mSpeechId = 0;

            //Creation of the texture/sprite of the overlay.
            string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
            Texture2D spriteTexture = new Texture2D(1, 1);
            spriteTexture.hideFlags = HideFlags.HideAndDontSave;
            spriteTexture.LoadImage(File.ReadAllBytes(Buddy.Resources.GetSpritesFullPath(lRandomSpriteName + ".png") ));
            spriteTexture.Apply();
            Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));
            Sprite lOverlaySprite = Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));
            mOverlayTexture = lOverlaySprite.texture;
            mOverlay.texture = mOverlayTexture;

            //Check if we can open the RGBCamera.
            if (Buddy.Sensors.RGBCamera.IsBusy)
            {
                Buddy.Sensors.RGBCamera.Close();
            }
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            if (!Buddy.Sensors.RGBCamera.IsOpen)
            {
                Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_640x480_30FPS_RGB);
                
            }

            //We don't use Toaster to display video because we want to display on the full screen and we can't with toaster right now.
            mVideo.gameObject.SetActive(true);
            if (TakePhotoData.Instance.Overlay)
                mOverlay.gameObject.SetActive(true);
            Buddy.Vocal.SayKey("takephoto", true);
		}

        /// <summary>
        /// Callback called at every new valid frame.
        /// </summary>
        /// <param name="iFrame">Frame delivered by the RGBCamera</param>
        private void OnFrameCaptured(RGBCameraFrame iFrame)
        {
            mVideo.texture = iFrame.MirroredTexture;
            mIsFrameCaptured = true;
        }

        
		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            if (mIsFrameCaptured)
            {
                if (!Buddy.Vocal.IsSpeaking)
                {
                    mTimer += Time.deltaTime;
                    //TODO : add a timer so even if the vocal bugs we can do the next step.
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
                        //Take the picture.
                        if (Buddy.Sensors.RGBCamera.Width > 0)
                        {
                            mPictureSound.Play();
                            Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish, false, true);
                            mPhotoTaken = true; 
                        }
                        else
                        {
                            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.NULL_VALUE, "RGBCam is Null or there is a problem with TakePhotograph");
                        }
                    }
                }
            }
		}

        /// <summary>
        /// Callback called when we take a picture.
        /// </summary>
        /// <param name="iMyPhoto">Photo sent by CORE with takephotograph</param>
        private void OnFinish(Photograph iMyPhoto)
		{
            Buddy.Sensors.RGBCamera.Close();

			mVideo.gameObject.SetActive(false);
			mOverlay.gameObject.SetActive(false);

            //Add the overlay to the picture taken.
            if (TakePhotoData.Instance.Overlay)
            {
                var cols1 = mOverlayTexture.GetPixels();
                var cols2 = iMyPhoto.Image.texture.GetPixels();
                // We scale the overlay and put it on top of the picture
                int x = 0;
                int y = 0;
                int i2 = 0;
                for (var i = 0; i < cols2.Length; ++i)
                {

                    x = i % iMyPhoto.Image.texture.width;
                    y = i / iMyPhoto.Image.texture.width;
                    i2 = (x * mOverlayTexture.width / iMyPhoto.Image.texture.width) + (y * mOverlayTexture.height / iMyPhoto.Image.texture.height) * mOverlayTexture.width;
                    if (cols1[i2].a != 0)
                    {
                        cols2[i] = (1 - cols1[i2].a) * cols2[i] + cols1[i2].a * cols1[i2];
                    }
                }
                iMyPhoto.Image.texture.SetPixels(cols2);
                iMyPhoto.Image.texture.Apply();
                mPhotoSprite = Sprite.Create(iMyPhoto.Image.texture, new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            }
            else
            {
                mPhotoSprite = iMyPhoto.Image;
            }
            iMyPhoto.Save();
            TakePhotoData.Instance.PhotoPath = iMyPhoto.FullPath;
            mIsFrameCaptured = false;
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetRandomString("redoorshare"), null, "redoorshare", OnEndListening, null, false);

            //UI : display the photo taken with two buttons on the footer to share or redo a photo.
            Buddy.GUI.Toaster.Display<PictureToast>().With(mPhotoSprite);
            FButton lLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            FButton LRightButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_share"));
            LRightButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_redo"));
            lLeftButton.OnClick.Add(() => { OnButtonShare(); });
            LRightButton.OnClick.Add(() => { OnButtonRedo(); });
        }

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mIsFrameCaptured = false;
        }


        //Callback called at the end of the listening.
        private void OnEndListening(SpeechInput iInput)
        {
            if (!iInput.IsInterrupted)
            {
                if (ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, Buddy.Resources.GetPhoneticStrings("share")))
                {
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();
                    Trigger("Tweet");
                }
                else if (ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, Buddy.Resources.GetPhoneticStrings("redo")))
                {
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();
                    Trigger("RedoPhoto");
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
                        QuitApp();
                    }
                }
            }

        }

        private void OnButtonShare()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            Buddy.Vocal.StopListening();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Trigger("Tweet");
        }

		private void OnButtonRedo()
		{
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            Buddy.Vocal.StopListening();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Trigger("RedoPhoto");
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