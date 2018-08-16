using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OpenCVUnity;
using BlueQuark;


namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that display a window to test the noise detection sensibility
    /// </summary>
    public class DebugSoundState : AStateMachineBehaviour
    {
        private RawImage mRaw;
        private TSlider mGauge;
        private Mat mMatShow;
        private Texture2D mTexture;
        private Animator mDebugSoundAnimator;
        private float mTimer;

        private bool mHasDetectedSound = false;
        private bool mHasInitSlider = false;
        private bool mGoBack = false;
        private bool mHasOpenedWindow = false;
        private NoiseDetector mNoiseDetection;
        private DebugSoundWindow mDebugSoundWindow;

        private float mIntensity = 0.0f;
        private Queue<float> mSoundIntensities;
        private int mNbSoundPics = 50;

        public override void Start()
        {
            mTimer = 0.0f;
            mDebugSoundWindow = GetGameObject(DEBUG_SOUND).GetComponent<DebugSoundWindow>();
            mRaw = mDebugSoundWindow.Raw;
            mGauge = mDebugSoundWindow.GaugeSensibility;
            mMatShow = new Mat(480, 640, CvType.CV_8UC3);
            mTexture = new Texture2D(640, 480);
            mHasInitSlider = false;
            mHasDetectedSound = false;
            mGoBack = false;
            mHasOpenedWindow = false;
            mDebugSoundAnimator = mDebugSoundWindow.gameObject.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Start();

            mIntensity = 0.0f;
            mSoundIntensities = new Queue<float>();
            for(int i=0; i<mNbSoundPics; i++)
            {
                mSoundIntensities.Enqueue(0.0f);
            }

            Buddy.Vocal.SayKey("audiodetectionmessage");

            mNoiseDetection.OnDetect.AddP(OnNewSound, 0.0f);
            mDebugSoundWindow.ButtonBack.onClick.AddListener(GoBack);
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if (!mHasOpenedWindow && mTimer > 1.3f)
            {
                mTimer = 0.0f;
                mHasOpenedWindow = true;
                mDebugSoundAnimator.SetTrigger("Open_WDebugs");
            }
            else if (mHasOpenedWindow)
            {
                if (!mHasInitSlider /*&& mGauge.Slider*/)
                {
                    mHasInitSlider = true;
                    mGauge.SlidingValue= GuardianData.Instance.SoundDetectionThreshold;
                }

                if (mTimer > 0.04f)
                {
                    mTimer = 0.0f;
                    DisplaySound();
                    CheckNoiseDetection();
                }

                if (mHasInitSlider && mDebugSoundAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
                {
                    animator.SetInteger("DebugMode", -1);
                    GuardianData.Instance.SoundDetectionThreshold = (int)mGauge.SlidingValue;
                    mGoBack = false;
                    mDebugSoundWindow.Ico.enabled = false;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GuardianData.Instance.SoundDetectionThreshold = (int)mGauge.SlidingValue;
            GuardianData.Instance.FirstRunParam = false;
            mDebugSoundWindow.Ico.enabled = false;
            mNoiseDetection.OnDetect.RemoveP(OnNewSound);
            mDebugSoundWindow.ButtonBack.onClick.RemoveAllListeners();
            Buddy.Vocal.Stop();
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

        private void DisplaySound()
        {
            mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));

            float lMaxThreshold = DetectionManager.MAX_SOUND_THRESHOLD;
            float lThreshold = (1.0f - mGauge.SlidingValue/ 100F) * lMaxThreshold;
            float lLevelSound =  mIntensity * 400.0F / lMaxThreshold;

            mSoundIntensities.Enqueue(lLevelSound);
            mSoundIntensities.Dequeue();
            float lWidthPic = 640.0F / mNbSoundPics;
            int lIt = 0;
            foreach (float intensity in mSoundIntensities)
            {
                Imgproc.rectangle(mMatShow, new Point(lIt * lWidthPic, 480), new Point((lIt + 1) * lWidthPic, 480.0f - intensity), new Scalar(0, 212, 209, 255), -1);
                lIt++;
            }

            Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 400 / lMaxThreshold), new Point(640, 480.0f - lThreshold * 400 / lMaxThreshold), new Scalar(237, 27, 36, 255), 3);
            Utils.MatToTexture2D(mMatShow, mTexture);
            mRaw.texture = mTexture;
        }

        private void CheckNoiseDetection()
        {
            if (mHasDetectedSound)
            {
                mHasDetectedSound = false;
                mDebugSoundWindow.Ico.enabled = true;
            }
            else
                mDebugSoundWindow.Ico.enabled = false;
        }

        private bool OnNewSound(float iNoise)
        {
            mIntensity = iNoise;
            float lThreshold = (1.0f - mGauge.SlidingValue/ 100F) * DetectionManager.MAX_SOUND_THRESHOLD;
            if (iNoise > lThreshold)
            {
                mHasDetectedSound = true;
            }

            return true;
        }

    }
}