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

        private OpenCVUnity.Rect mDetectedBox;

        private const int MAXLISTENNINGITER = 3;
        private int mNumberListen;
        private float mTimeHumanDetected;

        private bool mStartTracking;

        public override void Start()
		{
            mStartTracking = false;
            mDetectedBox = new OpenCVUnity.Rect();
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

            Buddy.Navigation.Run<HumanTrackStrategy>().StaticTracking((x) => { return true; }, OnHumanDetected,
                BehaviourMovementPattern.BODY_ROTATION /*| BehaviourMovementPattern.EYES*/ | BehaviourMovementPattern.HEAD);

            //Init 
            ToastRender = false;
            mPhotoTaken = false;
            mTimer = 0;
            mSpeechId = 0;

            // Just for security
            Buddy.Vocal.StopAndClear();

            //Creation of the texture/sprite of the overlay.
            string lRandomSpriteName = mOverlaysNames[UnityEngine.Random.Range(0, mOverlaysNames.Count - 1)];
            Texture2D spriteTexture = new Texture2D(1, 1);
            spriteTexture.hideFlags = HideFlags.HideAndDontSave;
            spriteTexture.LoadImage(File.ReadAllBytes(Buddy.Resources.GetSpritesFullPath(lRandomSpriteName + ".png")));
            spriteTexture.Apply();
            Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));
            Sprite lOverlaySprite = Sprite.Create(spriteTexture, new UnityEngine.Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5F, 0.5F));
            mOverlayTexture = lOverlaySprite.texture;
            mOverlay.texture = mOverlayTexture;

            //Check if we can open the RGBCamera.
            if (Buddy.Sensors.RGBCamera.IsBusy) {
                Buddy.Sensors.RGBCamera.Close();
            }
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            if (!Buddy.Sensors.RGBCamera.IsOpen) {
                Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_15FPS_RGB);

            }

            //We don't use Toaster to display video because we want to display on the full screen and we can't with toaster right now.
            mVideo.gameObject.SetActive(true);
            if (TakePhotoData.Instance.Overlay)
                mOverlay.gameObject.SetActive(true);
            Buddy.Vocal.SayKey("takephoto", true);
        }

        private void OnHumanDetected(HumanEntity iHumans)
        {
            // Display
            // We update box , to display it later in OnNewFrame
            mTimeHumanDetected = Time.time;
            mDetectedBox = new OpenCVUnity.Rect(iHumans.BoundingBox.tl(), iHumans.BoundingBox.br());
        }

        /// <summary>
        /// Callback called at every new valid frame.
        /// </summary>
        /// <param name="iFrame">Frame delivered by the RGBCamera</param>
        private void OnFrameCaptured(RGBCameraFrame iFrame)
        {
            Mat lMatSrc = iFrame.Mat.clone();

            if ((Time.time - mTimeHumanDetected) < 0.2F) {
                // Clear all old box, from the last detection
                Imgproc.rectangle(lMatSrc, mDetectedBox.tl(), mDetectedBox.br(), new Scalar(new Color(255, 0, 0)), 3);
            }

            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mVideo.texture = Utils.ScaleTexture2DFromMat(lMatSrc, (Texture2D)mVideo.texture);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, (Texture2D)mVideo.texture);
            //mVideo.texture = iFrame.MirroredTexture;
            mIsFrameCaptured = true;
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mIsFrameCaptured) {
                if (!Buddy.Vocal.IsSpeaking) {
                    mTimer += Time.deltaTime;
                    //TODO : add a timer so even if the vocal bugs we can do the next step.
                    if (mTimer > 0.5F)
                        if (mSpeechId < 3) {
                            if (!mStartTracking) {
                                mStartTracking = true;
                                Buddy.Navigation.Run<HumanTrackStrategy>().StaticTracking(tracking => true, null, BehaviourMovementPattern.EYES | BehaviourMovementPattern.HEAD);
                            }
                            Buddy.Vocal.Say((3 - mSpeechId).ToString(), true);
                            if (mSpeechId == 2) {
                                Buddy.Vocal.SayKey("cheese", (iOutput) => Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish, true, true), true);
                            }
                            mSpeechId++;
                            mTimer = 0F;
                        } else if (!mPhotoTaken) {
                            //Take the picture.
                            if (Buddy.Sensors.RGBCamera.Width > 0) {
                                mStartTracking = false;
                                //Buddy.Navigation.Stop();
                                //mPictureSound.Play();
                                Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish, true, true);
                                mPhotoTaken = true;
                            } else {
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
            //mPictureSound.Play();
            Buddy.Navigation.Stop();

            Buddy.Sensors.RGBCamera.Close();
            
            mVideo.gameObject.SetActive(false);
			mOverlay.gameObject.SetActive(false);


            Buddy.Navigation.Stop();

            //Add the overlay to the picture taken.
            if (TakePhotoData.Instance.Overlay)
            {
                Debug.LogWarning("<color= red>+++++++++++++++++++OVERLAY+++++++++++++++</color>");
                var cols1 = mOverlayTexture.GetPixels();
                var cols2 = iMyPhoto.Image.texture.GetPixels();
                // We scale the overlay and put it on top of the picture
                int x = 0;
                int y = 0;
                int i2 = 0;
                for (var i = 0; i < cols2.Length; ++i) {

                    x = i % iMyPhoto.Image.texture.width;
                    y = i / iMyPhoto.Image.texture.width;
                    i2 = (x * mOverlayTexture.width / iMyPhoto.Image.texture.width) + (y * mOverlayTexture.height / iMyPhoto.Image.texture.height) * mOverlayTexture.width;
                    if (cols1[i2].a != 0) {
                        cols2[i] = (1 - cols1[i2].a) * cols2[i] + cols1[i2].a * cols1[i2];
                    }
                }
                iMyPhoto.Image.texture.SetPixels(cols2);
                iMyPhoto.Image.texture.Apply();
                mPhotoSprite = Sprite.Create(iMyPhoto.Image.texture, new UnityEngine.Rect(0, 0, iMyPhoto.Image.texture.width, iMyPhoto.Image.texture.height), new Vector2(0.5F, 0.5F));
            }
            else
            {
                Debug.LogWarning("<color= red>+++++++++++++++++++NO          OVERLAY+++++++++++++++</color>");
                mPhotoSprite = iMyPhoto.Image; 
            }
            iMyPhoto.Save();
            TakePhotoData.Instance.PhotoPath = iMyPhoto.FullPath;
            mIsFrameCaptured = false;
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetRandomString("redoorshare"), null, "redoorshare", OnEndListening, null);

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
            if (iInput.IsInterrupted) {
                return;
            }

            if (!string.IsNullOrEmpty(iInput.Rule)) {
                if (iInput.Rule.EndsWith("share")) {
                    ClearAndTrigger("Tweet");
                } else if (iInput.Rule.EndsWith("redo")) {
                    ClearAndTrigger("RedoPhoto");
                } else if (iInput.Rule.EndsWith("quit")) {
                    ClearAndQuit();
                }
                return;
            }
            if (mNumberListen < MAXLISTENNINGITER) {
                // if the human answer is outside of planned sentences, we increment the
                // number of listen and we listen again.
                mNumberListen++;
                Buddy.Vocal.Listen(
                    iInputRec => { OnEndListening(iInputRec); }
                    );
            } else {
                // If we launch the listen too many times, it's like a timeout and
                // we get back to the menu
                ClearAndQuit();
            }
        }

        private void OnButtonShare()
        {
            Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            ClearAndTrigger("Tweet");
        }

        private void OnButtonRedo()
        {
            Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            ClearAndTrigger("RedoPhoto");
        }

        private void ClearAndTrigger(string triggerName)
        {
            Buddy.Vocal.StopAndClear();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Trigger(triggerName);
        }

        private void ClearAndQuit()
        {
            Buddy.Vocal.StopAndClear();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            QuitApp();
        }
    }
}
