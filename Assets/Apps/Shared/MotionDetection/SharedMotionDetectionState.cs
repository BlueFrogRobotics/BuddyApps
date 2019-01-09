using BlueQuark;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVUnity;
using System.IO;

namespace BuddyApp.Shared
{
    public sealed class SharedMotionDetectionState : ASharedSMB
    {

        [Header("Display Video Parameters : ")]
        [SerializeField]
        private bool VideoDisplay;
        [SerializeField]
        private bool LookForUser;
        [SerializeField]
        private bool WantToSavePicture;
        [SerializeField]
        private string NameOfPictureSaved;
        [SerializeField]
        private int QuantityBeforeSavingPicture;
        [SerializeField]
        private bool ImageWithMovementDisplayed;
        [Header("Bip Sound Parameters : ")]
        [SerializeField]
        private bool BipSound;
        [SerializeField]
        private SoundSample FxSound = SoundSample.BEEP_1;
        [Header("Display Movement Parameters : ")]
        [SerializeField]
        private bool DisplayMovement;
        [SerializeField]
        private bool WantToFlip;
        [SerializeField]
        private Color32 ColorOfDisplay;
        [SerializeField]
        private string TriggerWhenDetected;
        [SerializeField]
        private string TriggerWhenNotDetected;
        [SerializeField]
        private bool OnlyOneDetection;

        [Header("Movement Quantity Parameters : ")]
        [Tooltip("The quantity of movement represents the number you need to reach in order to move to another state.")]
        [SerializeField]
        private int QuantityMovement;
        [SerializeField]
        private bool WantChangingQuantity;
        [Header("Timer (between 0 and 20) : ")]
        [Tooltip("You do the motion detection during the timer.")]
        [Range(0F, 20F)]
        [SerializeField]
        private float Timer;
        [Tooltip("If you want to change timer each time you go in this state, check this box and create a float Timer in the animator's parameter.")]
        [SerializeField]
        private bool WantChangingTimer;
        [Header("Area in the picture/video : ")]
        [Tooltip("Area in the picture where you do your motion detection.")]
        [SerializeField]
        private bool AreaToDetect;
        [Header("Mood of Buddy when you exit the state : ")]
        [Tooltip("You can chose what mood will have Buddy when you detect enough movement and  when you quit this state.")]
        [SerializeField]
        private Mood MoodTypeWhenDetected;
        [SerializeField]
        private SoundSample SoundWhenDetected;
        [SerializeField]
        private Mood MoodTypeWhenNotDetected;
        [SerializeField]
        private SoundSample SoundWhenNotDetected = SoundSample.BEEP_1;

        private bool mIsDisplay;
        private RGBCamera mCam;
        private Mat mMatDetection;
        private MotionDetector mMotion;
        private OpenCVUnity.Rect mRect;
        private Texture2D mTexture;
        private Mat mMat;
        private Texture2D mTextureRefresh;
        private int mDetectionCount;
        private int mDetectionCountTest;
        private Mat mMatCopy;
        private Mat mMatDetectionCopy;
        private float mDurationDetection;
        private float mTimer;
        private bool mReposeDone;
        private bool mExitOne;
        private bool mExitTwo;
        private bool mSoundPlayedWhenDetected;
        private bool mIsInit = false;
        //private Sprite mSprite;

        //Position in the Image
        private float mPositionX;
        private float mPositionY;
        private Sprite mMotionDetectionSprite;
        MotionDetectorParameter mMotionDetectorParameter;

        public override void Start()
        {
            mMotion = Buddy.Perception.MotionDetector;
            mCam = Buddy.Sensors.RGBCamera;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTexture = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            mTextureRefresh = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            mIsInit = false;
            mMat = new Mat();
            mMatCopy = new Mat();
            mDetectionCountTest = 0;
            QuantityBeforeSavingPicture = 0;
            mPositionX = 0;
            mPositionY = 0;
            mReposeDone = false;
            mExitOne = false;
            mExitTwo = false;
            mIsDisplay = false;
            mDurationDetection = 0F;
            mTimer = 0F;
            mSoundPlayedWhenDetected = false;
            mDetectionCount = 0;
            mMotionDetectorParameter = new MotionDetectorParameter();

            if (WantChangingTimer) {
                if (iAnimator.GetFloat("Timer") != 0F)
                    Timer = iAnimator.GetFloat("Timer");
                else
                    Debug.Log("You didn't create a float named Timer in animtor's parameter, do it and change its value with  animator.SetFloat(\"Timer\", your value);");
            }

            if (WantChangingQuantity) {
                if (iAnimator.GetFloat("QuantityMovement") != 0F)
                    QuantityMovement = iAnimator.GetInteger("QuantityMovement");
                else
                    Debug.Log("You didn't create a integer named QuantityMovement in animator's parameter, do it and change its value with animator.SetInteger(\"QuantityMovement\", your value);");
            }

            mCam.Open(RGBCameraMode.COLOR_320X240_30FPS_RGB);
            mCam.OnNewFrame.Add((iFrame) => {
                mMat = iFrame.Mat.clone();
                mIsInit = true;
            });
            if (!AreaToDetect) {
                mMotion.OnDetect.AddP(OnMovementDetected, new MotionDetectorParameter() {
                    RegionOfInterest = new OpenCVUnity.Rect(0, 0, Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height),
                    SensibilityThreshold = 2.5F
                });
            } else {
                mRect = new OpenCVUnity.Rect(new Point((int)(320 / 3), 0), new Point((int)(Buddy.Sensors.RGBCamera.Width * 2 / 3), Buddy.Sensors.RGBCamera.Height));
                mMotionDetectorParameter.SensibilityThreshold = 2.5F;
                mMotionDetectorParameter.RegionOfInterest = mRect;
                mMotion.OnDetect.AddP(OnMovementDetected, mMotionDetectorParameter);
            }
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDurationDetection += Time.deltaTime;
            mTimer += Time.deltaTime;
            if (Timer == 0F)
                Timer = 5F;

            // Astra is open but not sending frames
            if (!mIsInit)
                return;

            if (mCam.IsOpen && VideoDisplay && !mIsDisplay) {
                mTimer = 0F;
                mIsDisplay = true;
                mMatCopy = mMat.clone();
                if (!WantToFlip)
                    Core.flip(mMatCopy, mMatCopy, 1);
                mTexture = Utils.ScaleTexture2DFromMat(mMatCopy, mTexture);
                Utils.MatToTexture2D(mMatCopy, mTexture);
                mMotionDetectionSprite = Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f));
                if (mTexture == null) {
                    mIsDisplay = false;
                } else {
                    Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);
                }
            }
            if (VideoDisplay && mIsDisplay && mTimer > 0.1F) {
                if (mMatDetectionCopy == null && !AreaToDetect) {
                    mMatCopy = mMat.clone();
                    if (!WantToFlip)
                        Core.flip(mMatCopy, mMatCopy, 1);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                } else if (mMatDetectionCopy == null && AreaToDetect) {
                    mMatCopy = mMat.clone();
                    if (!WantToFlip)
                        Core.flip(mMatCopy, mMatCopy, 1);
                    Imgproc.rectangle(mMatCopy, new Point((int)(mMatCopy.width() / 3), 0), new Point((int)(mMatCopy.width() * 2 / 3), mMatCopy.height()), new Scalar(ColorOfDisplay), 3);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                }

                if (mMatDetectionCopy != null && AreaToDetect) {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                } else if (mMatDetectionCopy != null && !AreaToDetect) {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                }
                mTexture.Apply();
                mTimer = 0F;
            }

            if ((mDetectionCount > QuantityMovement /*|| (mDetectionCountTest / 15F) > QuantityMovement*/) && !mExitTwo) {
                mExitOne = true;
                if (Buddy.Behaviour.Mood != MoodTypeWhenDetected) {
                    Buddy.Behaviour.SetMood(MoodTypeWhenDetected);
                }
                if (LookForUser) {
                    if (!mReposeDone) {
                        RePosition();
                    }
                    if (mReposeDone) {
                        if (!mSoundPlayedWhenDetected) {
                            mSoundPlayedWhenDetected = true;
                            Buddy.Actuators.Speakers.Media.Play(SoundWhenDetected);
                        }
                        Trigger(TriggerWhenDetected);
                    }
                } else {
                    if (!mSoundPlayedWhenDetected) {
                        mSoundPlayedWhenDetected = true;
                        Buddy.Actuators.Speakers.Media.Play(SoundWhenDetected);
                    }
                    Trigger(TriggerWhenDetected);
                }
            }

            if (mDurationDetection > Timer && mDetectionCount <= QuantityMovement && !mExitOne) {
                mExitTwo = true;

                if (Buddy.Behaviour.Mood != MoodTypeWhenNotDetected) {
                    Buddy.Behaviour.SetMood(MoodTypeWhenNotDetected);
                }
                if (!mSoundPlayedWhenDetected) {
                    mSoundPlayedWhenDetected = true;
                    Buddy.Actuators.Speakers.Media.Play(SoundWhenNotDetected);

                }
                if (!string.IsNullOrEmpty(TriggerWhenNotDetected))
                    Trigger(TriggerWhenNotDetected);
                else {
                    mMotion.OnDetect.RemoveP(OnMovementDetected);
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!string.IsNullOrEmpty(NameOfPictureSaved)) {
                if (!File.Exists(Buddy.Resources.GetRawFullPath(NameOfPictureSaved))) {
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.NOT_FOUND, "NO PICTURE");
                }
            }
            if (Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Toaster.Hide();
            mMotion.OnDetect.RemoveP(OnMovementDetected);
            mCam.Close();
            if (!string.IsNullOrEmpty(TriggerWhenDetected))
                ResetTrigger(TriggerWhenDetected);
            if (!string.IsNullOrEmpty(TriggerWhenNotDetected))
                ResetTrigger(TriggerWhenNotDetected);
            mReposeDone = false;
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            mMatDetection = mCam.Frame.Mat.clone();
            mMatDetectionCopy = mMatDetection.clone();
            Texture2D lTexture = new Texture2D(mCam.Width, mCam.Height);
            if (!WantToFlip)
                Core.flip(mMatDetectionCopy, mMatDetectionCopy, 1);

            if (OnlyOneDetection) {
                if (WantToSavePicture && mDetectionCount > QuantityBeforeSavingPicture) {
                    if (!string.IsNullOrEmpty(NameOfPictureSaved)) {
                        if (ImageWithMovementDisplayed) {
                            foreach (MotionEntity lEntity in iMotions) {
                                Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                                Core.flip(mMatDetection, mMatDetectionCopy, 1);
                            }
                            Utils.MatToTexture2D(mMatDetectionCopy, Utils.ScaleTexture2DFromMat(mMatDetectionCopy, lTexture));
                            File.WriteAllBytes(Buddy.Resources.GetRawFullPath(NameOfPictureSaved), lTexture.EncodeToJPG());
                        } else {
                            Core.flip(mMatDetection, mMatDetection, 1);
                            Utils.MatToTexture2D(mMatDetection, Utils.ScaleTexture2DFromMat(mMatDetection, lTexture));
                            File.WriteAllBytes(Buddy.Resources.GetRawFullPath(NameOfPictureSaved), lTexture.EncodeToJPG());
                        }
                    } else
                        ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.NULL_VALUE, "The name of the picture is empty.");
                }
                if (!string.IsNullOrEmpty(TriggerWhenDetected) && mDetectionCount > QuantityBeforeSavingPicture && File.Exists(Buddy.Resources.GetRawFullPath(NameOfPictureSaved)))
                    Trigger(TriggerWhenDetected);
                else
                    ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.INFO, LogInfo.NULL_VALUE, "Your trigger when detected is empty");
                mDetectionCount++;
            }
            if (iMotions.Length > 5) {
                if (BipSound) {
                    if (!Buddy.Actuators.Speakers.IsBusy)
                        Buddy.Actuators.Speakers.Media.Play(FxSound);
                }

                if (LookForUser) {
                    foreach (MotionEntity lEntity in iMotions) {
                        mPositionX += lEntity.RectInFrame.x;
                        mPositionY += lEntity.RectInFrame.y;
                        mDetectionCountTest++;
                    }
                }
                foreach (MotionEntity lEntity in iMotions) {
                    if (AreaToDetect) {
                        Imgproc.rectangle(mMatDetectionCopy, new Point((int)(mMatDetectionCopy.width() / 3), 0), new Point((int)(mMatDetectionCopy.width() * 2 / 3), mMatDetectionCopy.height()), new Scalar(ColorOfDisplay), 3);
                    }

                    if (DisplayMovement && VideoDisplay) {
                        if (!WantToFlip) {
                            Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                            Core.flip(mMatDetection, mMatDetectionCopy, 1);
                        } else {
                            Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                            mMatDetection.copyTo(mMatDetectionCopy);
                        }
                    }
                }
                mDetectionCount++;
            }
            return true;
        }

        private void RePosition()
        {
            Debug.Log("<color = red> REPOSITION SHARED MOTION DETECT</color>");
            mPositionX /= mDetectionCountTest;
            mPositionY /= mDetectionCountTest;
            float lAngle;
            if (!WantToFlip)
                lAngle = -(mPositionX / mCam.Width - 0.5F) * 120F;
            else
                lAngle = (mPositionX / mCam.Width - 0.5F) * 120F;
            Debug.Log("<color = red> REPOSITION SHARED MOTION DETECT : </color>" + lAngle);
            Buddy.Actuators.Head.No.SetPosition(lAngle, 95F);
            Debug.Log("<color = red> REPOSITION SHARED MOTION DETECT 2 : </color>" + ((mPositionY / mCam.Height - 0.5F) * -40F));
            Buddy.Actuators.Head.Yes.SetPosition((mPositionY / mCam.Height - 0.5F) * -40F, 95F);
            mReposeDone = true;
        }
    }
}