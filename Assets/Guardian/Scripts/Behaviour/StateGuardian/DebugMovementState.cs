using UnityEngine;
using UnityEngine.UI;
using OpenCVUnity;
using System.Collections.Generic;
using BuddyOS;
using BuddyOS.UI;

namespace BuddyApp.Guardian
{
    public class DebugMovementState : AStateGuardian
    {

        private Animator mAnimator;
        private Animator mDebugMovementAnimator;
        private BuddyFeature.Detection.MovementDetector mMovementDetector;

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

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetWindowAppOverBuddyColor(1);
            //Application.targetFrameRate = 10;
            mAnimator = animator;
            Init();
            //StateManager.DebugMovementWindow.gameObject.SetActive(true);
            mDebugMovementAnimator.SetTrigger("Open_WDebugs");
            StateManager.DebugMovementWindow.ButtonBack.onClick.AddListener(GoBack);
            mTimer = 0.0f;
            mMatRed = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3, new Scalar(254, 0, 0));
            mMovementDetector.OnDetection += OnMovementDetected;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mGauge.Slider)
            {
                mHasInitSlider = true;
                mGauge.Slider.value = (1.0f - (mMovementDetector.Threshold / mMovementDetector.MaxThreshold)) * mGauge.Slider.maxValue;
            }
            mTimer += Time.deltaTime;
            if (mTimer > 0.1f)
            {
                mTimer = 0.0f;
                Mat lMatMouv = new Mat();
                Mat lMatView = new Mat();
                Mat lMatCam = new Mat();

                float lMaxDetector = mMovementDetector.MaxThreshold;
                float lValueSliderPercent = 1.0f - (mGauge.Slider.value / mGauge.Slider.maxValue);
                mMovementDetector.Threshold = (lValueSliderPercent * lMaxDetector);

                mMask = mMovementDetector.MoveTracker.BinaryImage;
                Imgproc.circle(mMask, mMovementDetector.MoveTracker.PositionMoment, 5, new Scalar(254, 254, 254), -1);

                mMatRed.copyTo(lMatMouv, mMask);
                Imgproc.threshold(mMask, mMask, 200, 255, Imgproc.THRESH_BINARY_INV);
                mCam.FrameMat.copyTo(lMatCam, mMask);
                lMatView = lMatMouv + lMatCam;
                //Imgproc.putText(lMatView, "recorded by Buddy", new Point(lMatView.height()-50, lMatView.height()-10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.28, new Scalar(0, 212, 209, 255));
                BuddyTools.Utils.MatToTexture2D(lMatView, mTexture);
                mRaw.texture = mTexture;
                if (mHasDetectedMouv)
                {
                    StateManager.PlayBeep();
                    mHasDetectedMouv = false;
                    StateManager.DebugMovementWindow.IcoMouv.enabled = true;
                }
                else
                    StateManager.DebugMovementWindow.IcoMouv.enabled = false;
            }

            if(mHasInitSlider && mDebugMovementAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
            {
                mAnimator.SetInteger("DebugMode", -1);
                mGoBack = false;
                StateManager.DebugMovementWindow.IcoMouv.enabled = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StateManager.DebugMovementWindow.IcoMouv.enabled = false;
            mMovementDetector.OnDetection -= OnMovementDetected;
            StateManager.DebugMovementWindow.ButtonBack.onClick.RemoveAllListeners();
            //StateManager.DebugMovementWindow.gameObject.SetActive(false);
        }

        private void Init()
        {
            mCam = BYOS.Instance.RGBCam;
            mMovementDetector = StateManager.Detectors.MovementDetector;
            mRaw = StateManager.DebugMovementWindow.Raw;
            mGauge = StateManager.DebugMovementWindow.GaugeSensibility;
            mMask = new Mat(mCam.Height, mCam.Width, CvType.CV_8UC3);
            mTexture = new Texture2D(mCam.Width, mCam.Height);
            mHasDetectedMouv = false;
            mHasInitSlider = false;
            mGoBack = false;
            mDebugMovementAnimator = StateManager.DebugMovementWindow.gameObject.GetComponent<Animator>();
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