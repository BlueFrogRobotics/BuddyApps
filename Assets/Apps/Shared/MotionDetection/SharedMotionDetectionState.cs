﻿using Buddy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVUnity;
using Buddy.UI;
using System.IO;

namespace BuddyApp.Shared
{
    public class SharedMotionDetectionState : ASharedSMB
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
        private int QuantityBeforeTrigger;
        [SerializeField]
        private bool ImageWithMovementDisplayed;
        [Header("Bip Sound Parameters : ")]
        [SerializeField]
        private bool BipSound;
        [SerializeField]
        private FXSound FxSound;
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
        private string Key;
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
        private MoodType MoodTypeWhenDetected;
        [SerializeField]
        private VoiceSound SoundWhenDetected;
        [SerializeField]
        private MoodType MoodTypeWhenNotDetected;
        [SerializeField]
        private VoiceSound SoundWhenNotDetected;

        private bool mIsDisplay;
        private RGBCam mCam;
        private Mat mMatDetection;
        private MotionDetection mMotion;
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
        //private Sprite mSprite;

        //Position in the Image
        private float mPositionX;
        private float mPositionY;

        public override void Start()
        {
            mMotion = Perception.Motion;
            mCam = Primitive.RGBCam;

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            QuantityBeforeTrigger = 0;
            mPositionX = 0;
            mPositionY = 0;
            mReposeDone = false;
            mExitOne = false;
            mExitTwo = false;
            mIsDisplay = false;
            mDurationDetection = 0F;
            mDetectionCountTest = 0;
            mTimer = 0F;
            mSoundPlayedWhenDetected = false;
            mDetectionCount = 0;
            if (WantChangingTimer && iAnimator.GetFloat("Timer") != 0F)
            {
                Timer = iAnimator.GetFloat("Timer");
            }
            else
                Debug.Log("You didn't create a float named Timer in animtor's parameter, do it and change its value with  animator.SetFloat(\"Timer\", your value);");
            if (WantChangingQuantity && iAnimator.GetFloat("QuantityMovement") != 0)
            {
                QuantityMovement = iAnimator.GetInteger("QuantityMovement");
            }
            else
                Debug.Log("You didn't create a integer named QuantityMovement in animator's parameter, do it and change its value with animator.SetInteger(\"QuantityMovement\", your value);");
            //mCam.Resolution = RGBCamResolution.W_320_H_240; 
            mCam.Open(RGBCamResolution.W_320_H_240);
            if (!AreaToDetect)
            {
                mMotion.OnDetect(OnMovementDetected, 3F);
            }
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
                mTimer = 0F;
                mIsDisplay = true;
                mMat = mCam.FrameMat.clone();
                mMatCopy = mMat.clone();
                if (!WantToFlip)
                    Core.flip(mMatCopy, mMatCopy, 1);
                mTexture = Utils.MatToTexture2D(mMatCopy);
                Toaster.Display<PictureToast>().With(Dictionary.GetString(Key), Sprite.Create(mTexture, new UnityEngine.Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f)));
            }


            if (VideoDisplay && mIsDisplay && mTimer > 0.1F)
            {

                if (mMatDetectionCopy == null && !AreaToDetect)
                {
                    mMat = mCam.FrameMat.clone();
                    mMatCopy = mMat.clone();
                    if (!WantToFlip)
                        Core.flip(mMatCopy, mMatCopy, 1);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                }
                else if (mMatDetectionCopy == null && AreaToDetect)
                {
                    mMat = mCam.FrameMat.clone();
                    mMatCopy = mMat.clone();
                    if (!WantToFlip)
                        Core.flip(mMatCopy, mMatCopy, 1);
                    Imgproc.rectangle(mMatCopy, new Point((int)(mMatCopy.width() / 3), 0), new Point((int)(mMatCopy.width() * 2 / 3), mMatCopy.height()), new Scalar(ColorOfDisplay), 3);
                    mTextureRefresh = Utils.MatToTexture2D(mMatCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                }

                if (mMatDetectionCopy != null && AreaToDetect)
                {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                }
                else if (mMatDetectionCopy != null && !AreaToDetect)
                {
                    mTextureRefresh = Utils.MatToTexture2D(mMatDetectionCopy);
                    mTexture.SetPixels(mTextureRefresh.GetPixels());
                    mMatDetection = null;
                }
                mTexture.Apply();
                mTimer = 0F;
            }

            if ((mDetectionCount > QuantityMovement || (mDetectionCountTest / 15F) > QuantityMovement) && !mExitTwo)
            {
                mExitOne = true;

                if (Interaction.Mood.CurrentMood != MoodTypeWhenDetected)
                {
                    Interaction.Mood.Set(MoodTypeWhenDetected);
                }
                if (LookForUser)
                {
                    if (!mReposeDone)
                        RePosition();
                    if (mReposeDone)
                    {
                        if (SoundWhenDetected != VoiceSound.NONE && !mSoundPlayedWhenDetected)
                        {
                            mSoundPlayedWhenDetected = true;
                            Primitive.Speaker.Voice.Play(SoundWhenDetected);

                        }
                        Trigger(TriggerWhenDetected);
                    }
                }
                else
                {
                    if (SoundWhenDetected != VoiceSound.NONE && !mSoundPlayedWhenDetected)
                    {
                        mSoundPlayedWhenDetected = true;
                        Primitive.Speaker.Voice.Play(SoundWhenDetected);

                    }
                    Trigger(TriggerWhenDetected);
                }

            }

            if (mDurationDetection > Timer && mDetectionCount <= QuantityMovement && !mExitOne)
            {
                mExitTwo = true;

                if (Interaction.Mood.CurrentMood != MoodTypeWhenNotDetected)
                {
                    Interaction.Mood.Set(MoodTypeWhenNotDetected);
                }
                if (SoundWhenNotDetected != VoiceSound.NONE && !mSoundPlayedWhenDetected)
                {
                    mSoundPlayedWhenDetected = true;
                    Primitive.Speaker.Voice.Play(SoundWhenNotDetected);

                }
                if (!string.IsNullOrEmpty(TriggerWhenNotDetected))
                    Trigger(TriggerWhenNotDetected);
                else
                {
                    mMotion.StopOnDetect(OnMovementDetected);
                }
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(!File.Exists(BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved)))
            {
                Debug.Log("SHARED PAS DE PICTURE");
            }
            if (Toaster.IsDisplayed)
                Toaster.Hide();
            mMotion.StopOnDetect(OnMovementDetected);
            mCam.Close();
            if (!string.IsNullOrEmpty(TriggerWhenDetected)) 
                ResetTrigger(TriggerWhenDetected);
            if (!string.IsNullOrEmpty(TriggerWhenNotDetected))
                ResetTrigger(TriggerWhenNotDetected);
            mReposeDone = false;
        } 

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            mMatDetection = mCam.FrameMat.clone();
            mMatDetectionCopy = mMatDetection.clone();
            Texture2D lTexture = new Texture2D(mCam.Width, mCam.Height);
            if (!WantToFlip)
                Core.flip(mMatDetectionCopy, mMatDetectionCopy, 1);

            if (OnlyOneDetection)
            {
                if (WantToSavePicture)
                {
                    if (!string.IsNullOrEmpty(NameOfPictureSaved))
                    {
                        if (ImageWithMovementDisplayed)
                        {
                            foreach (MotionEntity lEntity in iMotions)
                            {
                                Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);

                                //Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                                Core.flip(mMatDetection, mMatDetectionCopy, 1);
                            }
                            //Core.flip(mMatDetection, mMatDetection, 1);

                            Utils.MatToTexture2D(mMatDetectionCopy, Utils.ScaleTexture2DFromMat(mMatDetectionCopy, lTexture));
                            File.WriteAllBytes(BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved), lTexture.EncodeToJPG());
                            //mSprite = Sprite.Create(lTexture, new UnityEngine.Rect(0,0,lTexture.width, lTexture.height), new Vector2(0.5F, 0.5F));
                            //Utils.SaveSpriteToFile(mSprite, BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved));
                        }
                        else
                        {
                            Core.flip(mMatDetection, mMatDetection, 1);

                            Utils.MatToTexture2D(mMatDetection, Utils.ScaleTexture2DFromMat(mMatDetection, lTexture));
                            File.WriteAllBytes(BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved), lTexture.EncodeToJPG());
                            //mSprite = Sprite.Create(lTexture, new UnityEngine.Rect(0, 0, lTexture.width, lTexture.height), new Vector2(0.5F, 0.5F));
                            //Utils.SaveSpriteToFile(mSprite, BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved));
                        }
                    }
                    else
                        Debug.Log("The name of the picture is empty.");
                }
                if (!string.IsNullOrEmpty(TriggerWhenDetected) && mDetectionCount > QuantityBeforeTrigger && File.Exists(BYOS.Instance.Resources.GetPathToRaw(NameOfPictureSaved)))
                    Trigger(TriggerWhenDetected);
                else
                    Debug.Log("your trigger when detected is empty.");
                mDetectionCount++;
            }
            if (iMotions.Length > 5)
            {
                bool lInRectangle = false;
                if (BipSound)
                {
                    if (FxSound == FXSound.NONE)
                    {
                        FxSound = FXSound.BEEP_1;
                    }
                    Primitive.Speaker.FX.Play(FxSound);
                }

                MotionBlob[] lBlob = iMotions.GetBlobs();
                MotionBlob lMainBlob = iMotions.GetMainBlob(lBlob);
                if (LookForUser)
                {
                    foreach (MotionEntity lEntity in lMainBlob.MotionEntityArray)
                    {
                        mPositionX += lEntity.RectInFrame.x;
                        mPositionY += lEntity.RectInFrame.y;
                        mDetectionCountTest++;
                    }
                }
                foreach (MotionEntity lEntity in iMotions)
                {
                    if (AreaToDetect)
                    {
                        Imgproc.rectangle(mMatDetectionCopy, new Point((int)(mMatDetectionCopy.width() / 3), 0), new Point((int)(mMatDetectionCopy.width() * 2 / 3), mMatDetectionCopy.height()), new Scalar(ColorOfDisplay), 3);
                        if (lEntity.RectInFrame.x > (mMatDetection.width() / 3) && lEntity.RectInFrame.x < (mMatDetection.width() * 2 / 3))
                            lInRectangle = true;
                    }

                    if (DisplayMovement && VideoDisplay)
                    {
                        if (!WantToFlip)
                        {
                            Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                            Core.flip(mMatDetection, mMatDetectionCopy, 1);
                        }
                        else
                        {
                            Imgproc.circle(mMatDetection, Utils.Center(lEntity.RectInFrame), 3, new Scalar(ColorOfDisplay), 3);
                            mMatDetection.copyTo(mMatDetectionCopy);
                        }
                    }
                }
                //if (lInRectangle)
                mDetectionCount++;
                Debug.Log("DETECTION MOTION SHARED COUNT : " + mDetectionCount);
            }
            return true;
        }

        private void RePosition()
        {
            Debug.Log("DETECTION COUNT : " + mDetectionCount + " TEST DETECTION : " + mDetectionCountTest / 15);
            mReposeDone = true;
            mPositionX /= mDetectionCountTest;
            mPositionY /= mDetectionCountTest;
            Debug.Log("POSITION X : " + mPositionX + " POSITION Y : " + mPositionY);
            float lAngle;
            if (!WantToFlip)
                lAngle = -(mPositionX / mCam.Width - 0.5F) * 120F;
            else
                lAngle = (mPositionX / mCam.Width - 0.5F) * 120F;

            Debug.Log("ANGLE : " + lAngle);

            Primitive.Motors.NoHinge.SetPosition(lAngle, 200F);
            Primitive.Motors.YesHinge.SetPosition((mPositionY / mCam.Height - 0.5F) * 40F, 200F);

        }
    }
}