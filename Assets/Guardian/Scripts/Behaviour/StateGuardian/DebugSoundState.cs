﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OpenCVUnity;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class DebugSoundState : AStateGuardian
    {

        private SoundDetector mSoundDetector;
        private RawImage mRaw;
        private Gauge mGauge;
        private Mat mMatShow;
        private Texture2D mTexture;
        private Animator mAnimator;
        private Animator mDebugSoundAnimator;
        private float mTimer;

        private bool mHasDetectedSound = false;
        private bool mHasInitSlider = false;
        private bool mGoBack = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            
            mAnimator = animator;
            Init();
            //StateManager.DebugSoundWindow.gameObject.SetActive(true);
            mDebugSoundAnimator.SetTrigger("Open_WDebugs");
            StateManager.DebugSoundWindow.ButtonBack.onClick.AddListener(GoBack);
            mSoundDetector.OnDetection += OnSoundDetected;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mGauge.Slider)
            {
                mHasInitSlider = true;
                mGauge.Slider.value = (1.0f - (mSoundDetector.GetThreshold() / mSoundDetector.GetMaxThreshold())) * mGauge.Slider.maxValue;
            }

            mTimer += Time.deltaTime;
            if (mTimer > 0.04f)
            {
                mTimer = 0.0f;
                mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));

                float lMaxThreshold = mSoundDetector.GetMaxThreshold();
                float lThreshold = (1.0f - mGauge.Slider.value / mGauge.Slider.maxValue) * lMaxThreshold;
                mSoundDetector.SetThreshold(lThreshold);



                float lLevelSound = (mSoundDetector.Value) * 400.0f / lMaxThreshold;
                //Imgproc.line(mMatShow, new Point(0, 480.0f - lLevelSound), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255));
                Imgproc.rectangle(mMatShow, new Point(0, 480), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255), -1);
                Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 400 / lMaxThreshold), new Point(640, 480.0f - lThreshold * 400 / lMaxThreshold), new Scalar(0, 212, 209, 255), 3);
                Debug.Log("niveau: " + lThreshold);

                BuddyTools.Utils.MatToTexture2D(mMatShow, mTexture);
                mRaw.texture = mTexture;
                if (mHasDetectedSound)
                {
                    mHasDetectedSound = false;
                    StateManager.DebugSoundWindow.Ico.enabled = true;
                }
                else
                    StateManager.DebugSoundWindow.Ico.enabled = false;
            }

            if (mHasInitSlider && mDebugSoundAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
            {
                mAnimator.SetInteger("DebugMode", -1);
                mGoBack = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mSoundDetector.OnDetection -= OnSoundDetected;
            StateManager.DebugSoundWindow.ButtonBack.onClick.RemoveAllListeners();
            
            //StateManager.DebugSoundWindow.gameObject.SetActive(false);
            mSoundDetector.Stop();
        }

        private void Init()
        {
            mTimer = 0.0f;
            mSoundDetector = StateManager.DetectorManager.SoundDetector;
            mRaw = StateManager.DebugSoundWindow.Raw;
            mGauge = StateManager.DebugSoundWindow.GaugeSensibility;
            mMatShow = new Mat(480, 640, CvType.CV_8UC3);
            mTexture = new Texture2D(640, 480);
            mSoundDetector.Init();
            mHasInitSlider = false;
            mHasDetectedSound = false;
            mGoBack = false;
            mDebugSoundAnimator= StateManager.DebugSoundWindow.gameObject.GetComponent<Animator>();
        }

        private void GoBack()
        {
            mDebugSoundAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private void OnSoundDetected()
        {
            mHasDetectedSound = true;
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}