using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.Collections.Generic;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State that display a window to test the movement detection sensibility
    /// </summary>
    public class DebugMovementState : AStateMachineBehaviour
    {
        private Animator mDebugMovementAnimator;
        private MotionDetection mMovementTracker;

        private RawImage mRaw;
        private Gauge mGauge;
        private Mat mMask;
        private Texture2D mTexture;
        private RGBCam mCam;
        private float mTimer;

        private Mat mMatRed;
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
            mCam = Primitive.RGBCam;
            mDebugMovementWindow = GetGameObject(StateObject.DEBUG_MOVEMENT).GetComponent<DebugMovementWindow>();
            mMovementTracker = BYOS.Instance.Perception.Motion;
            mRaw = mDebugMovementWindow.Raw;
            mGauge = mDebugMovementWindow.GaugeSensibility;
            mHasDetectedMouv = false;
            mHasInitSlider = false;
            mGoBack = false;
            mHasOpenedWindow = false;
            mDebugMovementAnimator = mDebugMovementWindow.gameObject.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Start();
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            mMask = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3);
            mDebugMovementWindow.ButtonBack.onClick.AddListener(GoBack);
            mTimer = 0.0f;
            mMatRed = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3, new Scalar(254, 0, 0));
            if (!mCam.IsOpen)
                mCam.Open(RGBCamResolution.W_176_H_144);
            //Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            mMovementTracker.OnDetect(OnMovementDetected);
            mCam.Resolution = RGBCamResolution.W_176_H_144;
            Interaction.TextToSpeech.SayKey("motiondetectionmessage", true);

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (!mHasOpenedWindow && mTimer > 1.3f) {
                mTimer = 0.0f;
                mHasOpenedWindow = true;
                mDebugMovementAnimator.SetTrigger("Open_WDebugs");
            } else if (mHasOpenedWindow) {

                if (!mHasInitSlider && mGauge.Slider) {
                    mHasInitSlider = true;
                    mGauge.Slider.value = GuardianData.Instance.MovementDetectionThreshold;
                    mCurrentThreshold = mGauge.Slider.value;
                }
                Debug.Log("Resolution : " + mCam.FrameMat.width());
                if (mTimer > 0.1f) {
                    mTimer = 0.0f;
                    DisplayMovement();
                    CheckMovementDetection();
                }

                if (mHasInitSlider && mDebugMovementAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack) {
                    animator.SetInteger("DebugMode", -1);

                    mGoBack = false;
                    mDebugMovementWindow.IcoMouv.enabled = false;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GuardianData.Instance.MovementDetectionThreshold = (int)mGauge.Slider.value;
            mDebugMovementWindow.IcoMouv.enabled = false;
            //Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            mDebugMovementWindow.ButtonBack.onClick.RemoveAllListeners();
            mMovementTracker.StopOnDetect(OnMovementDetected);
            if(mCam.IsOpen)
                mCam.Close();
        }

        private void DisplayMovement()
        {
            float lNewThreshold;
            float lMaxDetector = mMaxThreshold;
            float lValueSliderPercent = 1.0f - (mGauge.Slider.value / mGauge.Slider.maxValue);

            lNewThreshold = (lValueSliderPercent * lMaxDetector);
            
            if (lNewThreshold != mCurrentThreshold)
            {
                Debug.Log("THRESHOLD = " + lNewThreshold);
                mCurrentThreshold = lNewThreshold;
                mMovementTracker.ChangeThreshold(OnMovementDetected, lNewThreshold);
            }
            mRaw.texture = mCam.FrameTexture2D;
        }


        private void GoBack()
        {
            mDebugMovementAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {

            Debug.Log(iMotions.Length + " C'est la Motion");

            Mat lCurrentFrame = mCam.FrameMat.clone();

            foreach (MotionEntity lEntity in iMotions) {
                Imgproc.circle(lCurrentFrame, Utils.Center(lEntity.RectInFrame), 10, new Scalar(255, 0, 0), -1);
            }

            Utils.MatToTexture2D(lCurrentFrame, Utils.ScaleTexture2DFromMat(lCurrentFrame, mTexture));
            mRaw.texture = mTexture;
         
            if(iMotions.Length > 0 )
               mHasDetectedMouv = true;
            return true;
        }

        private void CheckMovementDetection()
        {
            if (mHasDetectedMouv) {
                BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                mHasDetectedMouv = false;
                mDebugMovementWindow.IcoMouv.enabled = true;
            } else
                mDebugMovementWindow.IcoMouv.enabled = false;
        }

    }
}
