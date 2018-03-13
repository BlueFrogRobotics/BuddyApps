using Buddy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVUnity;
using Buddy.UI;

namespace BuddyApp.SandboxApp
{
    public class MotionDetectionState : AStateMachineBehaviour {

        /// <summary>
        /// Parameters for the developer
        /// </summary>
        //[Header("PARAMETERS OF THE MOTION DETECTION")]
        //[Space]
        //[Header(" ")]
        //[Space]
        [Header("Display Video Parameters : ")]
        [SerializeField]
        private bool VideoDisplay;
        [Header("Bip Sound Parameters : ")]
        [SerializeField]
        private bool BipSound;
        [SerializeField]
        private FXSound FxSound;
        [Header("Display Movement Parameters (only if display video is checked) : ")]
        [SerializeField]
        private bool DisplayMovement;
        [SerializeField]
        private Color32 ColorOfDisplay;
        [SerializeField]
        private string Key;
        [Header("Movement Quantity Parameters : ")]
        [Tooltip("The quantity of movement represents the number you need to reach in order to move to another state.")]
        [SerializeField]
        private int QuantityMovement;
        [Header("Timer : ")]
        [Tooltip("You do the motion detection during the timer.")]
        [Range(0F, 8F)]
        [SerializeField]
        private float Timer;
        [Header("Area in the picture/video : ")]
        [Tooltip("Area in the picture where you do your motion detection.")]
        [SerializeField]
        private bool AreaToDetect;
        [Header("Mood of Buddy when you exit the state : ")]
        [Tooltip("You can chose what mood will have Buddy when you detect enough movement and  when you quit this state.")]
        [SerializeField]
        private  MoodType MoodType;
        
        private bool mIsDisplay;
        private RGBCam mCam;
        private Mat mMatDetection;
        private MotionDetection mMotion;
        private OpenCVUnity.Rect mRect;
        private Texture2D mTexture;
        private Mat mMat;
        private Texture2D mTextureRefresh;
        private int mDetectionCount;
        private Mat mMatCopy;
        private Mat mMatDetectionCopy;
        private float mDurationDetection;
        private float mTimer;

        public override void Start()
        {
            mMotion = Perception.Motion;
            mCam = Primitive.RGBCam;
            mIsDisplay = false;
            //VideoDisplay = false;
            //BipSound = false;
            //FxSound = FXSound.NONE;
            //DisplayMovement = false;
            //ColorOfDisplay = new Color(255, 0, 0);
            //Timer = 0F;
            //AreaToDetect = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //if (mCam.IsOpen)
            //    mCam.Close();
            mCam.Open(RGBCamResolution.W_320_H_240);
            if (!AreaToDetect)
                mMotion.OnDetect(OnMovementDetected, 3F);
            else
            {
                mRect = new OpenCVUnity.Rect(new Point((int)(320 / 3), 0), new Point((int)(320 * 2 / 3), 240));
                mMotion.OnDetect(OnMovementDetected, mRect, 3F);
            }
                
            
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDurationDetection += Time.deltaTime;
            mTimer += Time.deltaTime;
            if (Timer == 0F)
                Timer = 5F;
            

            if (mCam.IsOpen && VideoDisplay && !mIsDisplay)
            {
                Timer = 0F;
                mIsDisplay = true;
                mMat = mCam.FrameMat.clone();
                mMatCopy = mMat.clone();
                Core.flip(mMatCopy, mMatCopy, 1);
                mTexture = Utils.MatToTexture2D(mMatCopy);
                Toaster.Display<PictureToast>().With(Dictionary.GetString(Key), Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
            }
            if (VideoDisplay && mIsDisplay && mTimer > 0.1F)
            {
                if(mMatDetectionCopy == null && !AreaToDetect)
                {
                    mMat = mCam.FrameMat.clone();
                    mMatCopy = mMat.clone();
                    Core.flip(mMatCopy, mMatCopy, 1);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                }
                else if(mMatDetectionCopy == null && AreaToDetect)
                {
                    mMat = mCam.FrameMat.clone();
                    mMatCopy = mMat.clone();
                    Core.flip(mMatCopy, mMatCopy, 1);
                    Imgproc.rectangle(mMatCopy, new Point((int)(mMatCopy.width() / 3), 0), new Point((int)(mMatCopy.width() * 2 / 3), mMatCopy.height()), new Scalar(ColorOfDisplay), 3);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                }
                
                if(mMatDetectionCopy != null && AreaToDetect)
                {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                }
                else if(mMatDetectionCopy != null && !AreaToDetect)
                {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                }
                mTexture.Apply();
                mTimer = 0F;
            }

            //if (!VideoDisplay )
            //{
            //    mMat = mCam.FrameMat.clone();
            //    mTextureRefresh = Utils.MatToTexture2D(mMat);
            //    mTexture.SetPixels(mTextureRefresh.GetPixels());
            //    mTexture.Apply();
            //}

          

            //if (Timer > mDurationDetection && mDetectionCount < 15 && mIsDisplay)
            //    Debug.Log("lol");

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Toaster.Hide();
            mMotion.StopOnDetect(OnMovementDetected);
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            //if (iMotions.Length > 2 /*&& !mHasTriggered && mHasShowWindow*/)
            //{
            //    BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
            //    mMatDetection = mCam.FrameMat.clone();
            //    //Core.flip(mMatSrc, mMatDetection, 1);
            //    Imgproc.rectangle(mMatDetection, new Point((int)(mMatDetection.width() / 3), 0), new Point((int)(mMatDetection.width() * 2 / 3), mMatDetection.height()), new Scalar(255, 0, 0), 3);

            //    bool lInRectangle = false;
            //    foreach (MotionEntity lEntity in iMotions)
            //    {
            //        Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), 3);
            //        if (lEntity.RectInFrame.x > (mMatDetection.width() / 3) && lEntity.RectInFrame.x < (mMatDetection.width() * 2 / 3))
            //            lInRectangle = true;
            //    }
            //    if (lInRectangle)
            //        mDetectionCount++;

            //    //mMatDetection = mMatSrc.clone();
            //}
            //mTexture = Utils.MatToTexture2D(mMat);
            mMatDetection = mCam.FrameMat.clone();
            mMatDetectionCopy = mMatDetection.clone();
            Core.flip(mMatDetectionCopy, mMatDetectionCopy, 1);
            if (iMotions.Length > 2)
            {
                if(BipSound)
                {
                    if(FxSound == FXSound.NONE)
                    {
                        FxSound = FXSound.BEEP_1;
                    }
                    Primitive.Speaker.FX.Play(FxSound);
                }

                if(AreaToDetect)
                    Imgproc.rectangle(mMatDetectionCopy, new Point((int)(mMatDetectionCopy.width() / 3), 0), new Point((int)(mMatDetectionCopy.width() * 2 / 3), mMatDetectionCopy.height()), new Scalar(ColorOfDisplay), 3);
                if(DisplayMovement)
                {
                    foreach (MotionEntity lEntity in iMotions)
                    {
                        Imgproc.circle(mMatDetectionCopy, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                    }  
                }
                
            }
            return true;
        }
    }
}

