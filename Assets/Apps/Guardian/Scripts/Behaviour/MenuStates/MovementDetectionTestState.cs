﻿using UnityEngine.UI;
using UnityEngine;
using OpenCVUnity;
using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the detection sensibility, test them and set the head orientation
    /// </summary>
    public sealed class MovementDetectionTestState : AStateMachineBehaviour
    {
        private Texture2D mTexture;
        //private RGBCamera mCam;
        private Mat mMatSrc;

        private FButton mLeftButton;
        private FButton mValidateButton;
        private FLabeledHorizontalSlider mSlider;

        private MotionDetector mMovementTracker;
        private bool mInit;

        public override void Start()
        {
            mInit = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("selectsensibility"));
            mInit = false;
            //if (Buddy.Sensors.RGBCamera.IsBusy)
            //{
            //    Buddy.Sensors.RGBCamera.Close();
            //}
            //if (!Buddy.Sensors.RGBCamera.IsOpen)
            //{
            //Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320x240_30FPS_RGB);
           // }
            //mTexture = Utils.MatToTexture2D(Buddy.Sensors.RGBCamera.Frame);
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);

            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 


            mSlider = Buddy.GUI.Footer.CreateOnMiddle<FLabeledHorizontalSlider>();
            mSlider.SlidingValue = GuardianData.Instance.MovementDetectionThreshold;
            mSlider.OnSlide.Add(OnSlideChange);
            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));

            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Trigger("MovementDetection"); });
            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();

            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));

            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);
            mValidateButton.OnClick.Add(() => { SaveAndQuit(); });

            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.SensibilityThreshold = GuardianData.Instance.MovementDetectionThreshold * 5.0F / 100;
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 640, 480);

            mMovementTracker = Buddy.Perception.MotionDetector;
            mMovementTracker.OnDetect.AddP(OnMovementDetected, lMotionParam);
            mTexture = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mInit)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);
                mInit = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Sensors.RGBCamera.OnNewFrame.Remove(OnFrameCaptured);
            mMovementTracker.OnDetect.RemoveP(OnMovementDetected);
            mSlider.OnSlide.Remove(OnSlideChange);
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Remove(mLeftButton);
            Buddy.GUI.Footer.Remove(mValidateButton);
            Buddy.GUI.Footer.Remove(mSlider);
        }

        //private void OnFrameCaptured(Mat iInput)
        //{
        //    Debug.Log("frame");
        //    mMatSrc = iInput.clone();
        //    //iInput.copyTo(mMatSrc);
        //    Core.flip(mMatSrc, mMatSrc, 1);
        //    mTexture = Utils.ScaleTexture2DFromMat(mMatSrc, mTexture);
        //    Debug.Log("on new frame "+mTexture.width+" x "+mTexture.height);
        //    Utils.MatToTexture2D(mMatSrc, mTexture);
        //}

        private void OnFrameCaptured(Mat iInput)
        {
            Debug.Log("frame");
            Mat lTest = iInput.clone();
            if (lTest.empty())
                Debug.Log("on new frame empty");
            Debug.Log("on new frame " + mTexture.width + " x " + mTexture.height);
            mMatSrc = lTest.clone();
            Core.flip(mMatSrc, mMatSrc, 1);
            mTexture = Utils.ScaleTexture2DFromMat(mMatSrc, mTexture);
            Utils.MatToTexture2D(mMatSrc, mTexture);
        }

        private void SaveAndQuit()
        {
            GuardianData.Instance.MovementDetectionThreshold = (int)mSlider.SlidingValue;
            Trigger("MovementDetection");
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {

            Debug.Log(iMotions.Length + " it's Motion");
            if (iMotions.Length > 1)
                Buddy.Actuators.Speakers.Media.Play(Buddy.Resources.Get<AudioClip>("os_tone_beep"));

            Core.flip(mMatSrc, mMatSrc, 1);

            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatSrc, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), -1);
            }

            //Mat lMat = new Mat();
            Core.flip(mMatSrc, mMatSrc, 1);
            Utils.MatToTexture2D(mMatSrc, Utils.ScaleTexture2DFromMat(mMatSrc, mTexture));

            //if (iMotions.Length > 0)
            //    mHasDetectedMouv = true;
            return true;
        }

        private void OnSlideChange(float iValue)
        {
            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.SensibilityThreshold = iValue * 5.0F / 100;
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 640, 480);

            mMovementTracker.OnDetect.ModifyParameterP(OnMovementDetected, lMotionParam);
        }
    }
}