﻿using BlueQuark;

using OpenCVUnity;

using UnityEngine;

using System.Collections.Generic;

namespace BuddyApp.BabyPhone
{
    /// <summary>
    /// State where the user can test the sound detection sensibility and set it
    /// </summary>
    public sealed class SoundDetectionTestState : AStateMachineBehaviour
    {
        private Texture2D mTexture;
        private Mat mMatSrc;

        private FButton mLeftButton;
        private FButton mValidateButton;
        private FLabeledHorizontalSlider mSlider;

        private NoiseDetector mNoiseDetection;

        private float mIntensity = 0.0f;
        private Queue<float> mSoundIntensities;
        private int mNbSoundPics = 50;
        private float mTimer;
        private float mTimerToast = 0.0F;
        private bool mToasterVisible;


        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNoiseDetection = Buddy.Perception.NoiseDetector;
            mNoiseDetection.OnDetect.AddP(OnNewSound, 0.0F);
            mTimer = 0.0F;
            mTimerToast = 0.0F;
            mToasterVisible = false;
            
            mIntensity = 0.0F;
            mTexture = new Texture2D(640, 480);
            mSoundIntensities = new Queue<float>();
            for (int i = 0; i < mNbSoundPics; i++)
            {
                mSoundIntensities.Enqueue(0.0F);
            }
          
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            mTimerToast += Time.deltaTime;
            if (mTimer > 0.04F && mToasterVisible)
            {
                mTimer = 0.0F;
                DisplaySound();
            }
            if (!mToasterVisible && mTimerToast > 1.0F)
            {
                ShowToaster();
                mToasterVisible = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mNoiseDetection.OnDetect.RemoveP(OnNewSound);
            Buddy.GUI.Header.HideTitle();
        }


        private void SaveAndQuit()
        {
            BabyPhoneData.Instance.SoundDetectionThreshold = (int)mSlider.SlidingValue;
            Buddy.GUI.Toaster.Hide();
            CloseFooter();
            Trigger("SoundDetection");
        }

        private void CloseFooter()
        {
            Buddy.GUI.Footer.Remove(mLeftButton);
            Buddy.GUI.Footer.Remove(mValidateButton);
            Buddy.GUI.Footer.Remove(mSlider);
        }

        private bool OnNewSound(float iNoise)
        {
            Debug.Log("noise: " + iNoise);
            mIntensity = iNoise;

            return true;
        }

        private void ShowToaster()
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setnoisesensitivity"));
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);

            mSlider = Buddy.GUI.Footer.CreateOnMiddle<FLabeledHorizontalSlider>();
            mSlider.SlidingValue = BabyPhoneData.Instance.SoundDetectionThreshold;
            mSlider.SetLabel(Buddy.Resources.GetString("threshold"));
            mLeftButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();

            mLeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));

            mLeftButton.SetBackgroundColor(Color.white);
            mLeftButton.SetIconColor(Color.black);
            mLeftButton.OnClick.Add(() => { Buddy.GUI.Toaster.Hide(); CloseFooter(); Trigger("SoundDetection"); });
            mValidateButton = Buddy.GUI.Footer.CreateOnRight<FButton>();

            mValidateButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));

            mValidateButton.SetBackgroundColor(Utils.BUDDY_COLOR);
            mValidateButton.SetIconColor(Color.white);
            mValidateButton.OnClick.Add(() => { SaveAndQuit(); });
        }

        private void DisplaySound()
        {
            mMatSrc = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));

            float lMaxThreshold = DetectionManager.MAX_SOUND_THRESHOLD;
            float lThreshold = (mSlider.SlidingValue / 100F) * lMaxThreshold;
            float lLevelSound = mIntensity * 400.0F / lMaxThreshold;

            mSoundIntensities.Enqueue(lLevelSound);
            mSoundIntensities.Dequeue();
            float lWidthPic = 640.0F / mNbSoundPics;
            int lIt = 0;
            foreach (float intensity in mSoundIntensities)
            {
                Imgproc.rectangle(mMatSrc, new Point(lIt * lWidthPic, 480), new Point((lIt + 1) * lWidthPic, 480.0f - intensity), new Scalar(0, 212, 209, 255), -1);
                lIt++;
            }

            Imgproc.line(mMatSrc, new Point(0, 480.0f - lThreshold * 400 / lMaxThreshold), new Point(640, 480.0f - lThreshold * 400 / lMaxThreshold), new Scalar(237, 27, 36, 255), 3);
            Utils.MatToTexture2D(mMatSrc, mTexture);
        }
    }
}