using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.Collections.Generic;
using BlueQuark;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that display a window to test the movement detection sensibility
    /// </summary>
    public sealed class DebugMovementState : AStateMachineBehaviour
    {
        private Animator mDebugMovementAnimator;
        private MotionDetector mMovementTracker;

        private RawImage mRaw;
        private TSlider mGauge;
        private Texture2D mTexture;
        private RGBCamera mCam;
        private float mTimer;

        private bool mHasDetectedMouv = false;
        private bool mHasInitSlider = false;
        private bool mGoBack = false;
        private bool mHasOpenedWindow = false;

        private float mMaxThreshold;
        private float mCurrentThreshold;//new
        private DebugMovementWindow mDebugMovementWindow;

        public override void Start()
        {
            mMaxThreshold = DetectionManager.MAX_MOVEMENT_THRESHOLD;
            mDebugMovementWindow = GetGameObject(DEBUG_MOVEMENT).GetComponent<DebugMovementWindow>();
            mRaw = mDebugMovementWindow.Raw;
            mGauge = mDebugMovementWindow.GaugeSensibility;
            mHasDetectedMouv = false;
            mHasInitSlider = false;
            mGoBack = false;
            mHasOpenedWindow = false;
            mDebugMovementAnimator = mDebugMovementWindow.gameObject.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Start();
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            mDebugMovementWindow.ButtonBack.onClick.AddListener(GoBack);
            mTimer = 0.0f;
            if (!mCam.IsOpen)
                mCam.Open(RGBCameraMode.COLOR_320x240_30FPS_RGB);
            mMovementTracker.OnDetect.AddP(OnMovementDetected);
            mCam.Mode = RGBCameraMode.COLOR_320x240_30FPS_RGB;
            Buddy.Vocal.SayKey("motiondetectionmessage");

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mHasOpenedWindow && mTimer > 1.3f) {
                mTimer = 0.0f;
                mHasOpenedWindow = true;
                mDebugMovementAnimator.SetTrigger("Open_WDebugs");
            } else if (mHasOpenedWindow) {

                if (!mHasInitSlider/* && mGauge.Slider*/) {
                    mHasInitSlider = true;
                    mGauge.SlidingValue = GuardianData.Instance.MovementDetectionThreshold;
                    mCurrentThreshold = mGauge.SlidingValue;
                }
                Debug.Log("Resolution : " + mCam.Frame.width());
                if (mTimer > 0.1f) {
                    mTimer = 0.0f;
                    DisplayMovement();
                    CheckMovementDetection();
                }

                if (mHasInitSlider && mDebugMovementAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack) {
                    iAnimator.SetInteger("DebugMode", -1);

                    mGoBack = false;
                    mDebugMovementWindow.IcoMouv.enabled = false;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GuardianData.Instance.FirstRunParam = false;
            GuardianData.Instance.MovementDetectionThreshold = (int)mGauge.SlidingValue;
            mDebugMovementWindow.IcoMouv.enabled = false;
            //Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            mDebugMovementWindow.ButtonBack.onClick.RemoveAllListeners();
            mMovementTracker.OnDetect.RemoveP(OnMovementDetected);
            if(mCam.IsOpen)
                mCam.Close();
            Buddy.Vocal.Stop();
        }

        private void DisplayMovement()
        {
            float lNewThreshold;
            float lMaxDetector = mMaxThreshold;
            float lValueSliderPercent = 1.0f - (mGauge.SlidingValue/ 100F);

            lNewThreshold = (lValueSliderPercent * lMaxDetector);
            
            if (lNewThreshold != mCurrentThreshold)
            {
                mCurrentThreshold = lNewThreshold;
                MotionDetectorParameter lParamDetect = new MotionDetectorParameter();
                lParamDetect.SensibilityThreshold = lNewThreshold;
                mMovementTracker.OnDetect.ModifyParameterP(OnMovementDetected, lParamDetect);
                
                Debug.Log("threshold de test: " + lNewThreshold);
            }
			Mat mMat = new Mat();
			Mat mMatSrc = mCam.Frame;
			Core.flip(mMatSrc, mMat, 1);
			mRaw.texture = Utils.MatToTexture2D(mMat);
        }


        private void GoBack()
        {
            mDebugMovementAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {

            Debug.Log(iMotions.Length + " it's Motion");

            Mat lCurrentFrame = mCam.Frame.clone();

            foreach (MotionEntity lEntity in iMotions) {
                Imgproc.circle(lCurrentFrame, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), -1);
            }

			Mat mMat = new Mat();
			Mat mMatSrc = lCurrentFrame;
			Core.flip(mMatSrc, mMat, 1);
			Utils.MatToTexture2D(mMat, Utils.ScaleTexture2DFromMat(mMat, mTexture));
            mRaw.texture = mTexture;
         
            if(iMotions.Length > 0 )
               mHasDetectedMouv = true;
            return true;
        }

        private void CheckMovementDetection()
        {
            if (mHasDetectedMouv) {
                Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
                mHasDetectedMouv = false;
                mDebugMovementWindow.IcoMouv.enabled = true;
            } else
                mDebugMovementWindow.IcoMouv.enabled = false;
        }

    }
}
