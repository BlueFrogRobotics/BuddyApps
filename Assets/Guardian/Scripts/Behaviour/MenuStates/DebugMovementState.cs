using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.Collections.Generic;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Guardian
{
    public class DebugMovementState : AStateMachineBehaviour
    {

        private Animator mAnimator;
        private Animator mDebugMovementAnimator;
        //private MovementDetector mMovementDetector;
        private MovementTracker mMovementTracker;

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

        private DebugMovementWindow mDebugMovementWindow;

        public override void Start()
        {
            mCam = BYOS.Instance.Primitive.RGBCam;
            mDebugMovementWindow = GetGameObject(StateObject.DEBUG_MOVEMENT).GetComponent<DebugMovementWindow>();
            //mMovementDetector = BYOS.Instance.Perception.MovementDetector;//StateManager.Detectors.MovementDetector;
            mMovementTracker = BYOS.Instance.Perception.MovementTracker;
            mRaw = mDebugMovementWindow.Raw;
            mGauge = mDebugMovementWindow.GaugeSensibility;
            
            
            mHasDetectedMouv = false;
            mHasInitSlider = false;
            mGoBack = false;
            mDebugMovementAnimator = mDebugMovementWindow.gameObject.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Start();
            mMovementTracker.Enable();
            //mMovementDetector.Enable();
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            mMask = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3);
            //SetWindowAppOverBuddyColor(1);
            //Application.targetFrameRate = 10;
            mAnimator = animator;
            
            //mDebugMovementWindow.gameObject.SetActive(true);
            mDebugMovementAnimator.SetTrigger("Open_WDebugs");
            mDebugMovementWindow.ButtonBack.onClick.AddListener(GoBack);
            mTimer = 0.0f;
            mMatRed = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3, new Scalar(254, 0, 0));
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            //mMovementDetector.OnDetection += OnMovementDetected;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mGauge.Slider)
            {
                mHasInitSlider = true;
                //mGauge.Slider.value = (1.0f - (mMovementDetector.Threshold / mMovementDetector.MaxThreshold)) * mGauge.Slider.maxValue;
            }
            mTimer += Time.deltaTime;
            if (mTimer > 0.1f)
            {
                mTimer = 0.0f;
                Mat lMatMouv = new Mat();
                Mat lMatView = new Mat();
                Mat lMatCam = new Mat();

                float lMaxDetector = 0;// mMovementDetector.MaxThreshold;
                float lValueSliderPercent = 1.0f - (mGauge.Slider.value / mGauge.Slider.maxValue);
                //mMovementDetector.Threshold = (lValueSliderPercent * lMaxDetector);

                mMask = mMovementTracker.BinaryImage;
                Imgproc.circle(mMask, mMovementTracker.PositionMoment, 5, new Scalar(254, 254, 254), -1);

                mMatRed.copyTo(lMatMouv, mMask);
                Imgproc.threshold(mMask, mMask, 200, 255, Imgproc.THRESH_BINARY_INV);
                mCam.FrameMat.copyTo(lMatCam, mMask);
                lMatView = lMatMouv + lMatCam;
                //Imgproc.putText(lMatView, "recorded by Buddy", new Point(lMatView.height()-50, lMatView.height()-10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.28, new Scalar(0, 212, 209, 255));
                Utils.MatToTexture2D(lMatView, mTexture);
                mRaw.texture = mTexture;
                if (mHasDetectedMouv)
                {
                    BYOS.Instance.Primitive.Speaker.FX.Play(FXSound.BEEP_1);
                    //StateManager.PlayBeep();
                    mHasDetectedMouv = false;
                    mDebugMovementWindow.IcoMouv.enabled = true;
                }
                else
                    mDebugMovementWindow.IcoMouv.enabled = false;
            }

            if(mHasInitSlider && mDebugMovementAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
            {
                mAnimator.SetInteger("DebugMode", -1);
                mGoBack = false;
                mDebugMovementWindow.IcoMouv.enabled = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //GuardianData.Instance.MovementDetectionThreshold = (int)mMovementDetector.Threshold;
            mDebugMovementWindow.IcoMouv.enabled = false;
            //mMovementDetector.OnDetection -= OnMovementDetected;
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.MOVING, OnMovementDetected);
            mDebugMovementWindow.ButtonBack.onClick.RemoveAllListeners();
            //mDebugMovementWindow.gameObject.SetActive(false);
        }


        private void GoBack()
        {
            mDebugMovementAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private void OnMovementDetected()
        {
            mHasDetectedMouv = true;
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