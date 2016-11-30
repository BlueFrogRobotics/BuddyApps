using UnityEngine;
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
        private float mTimer;

        private bool mHasDetectedSound = false;
        private bool mHasInitSlider = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Init();
            mAnimator = animator;
            mStatePatrolManager.DebugSoundWindow.gameObject.SetActive(true);
            mStatePatrolManager.DebugSoundWindow.ButtonBack.onClick.AddListener(GoBack);
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



                float lLevelSound = (mSoundDetector.Value) * 480.0f;
                //Imgproc.line(mMatShow, new Point(0, 480.0f - lLevelSound), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255));
                Imgproc.rectangle(mMatShow, new Point(0, 480), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255), -1);
                Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 480), new Point(640, 480.0f - lThreshold * 480), new Scalar(255, 0, 0, 255), 3);

                BuddyTools.Utils.MatToTexture2D(mMatShow, mTexture);
                mRaw.texture = mTexture;
                if (mHasDetectedSound)
                {
                    mHasDetectedSound = false;
                    mStatePatrolManager.DebugSoundWindow.Ico.enabled = true;
                }
                else
                    mStatePatrolManager.DebugSoundWindow.Ico.enabled = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mSoundDetector.OnDetection -= OnSoundDetected;
            mStatePatrolManager.DebugSoundWindow.ButtonBack.onClick.RemoveAllListeners();
            mStatePatrolManager.DebugSoundWindow.gameObject.SetActive(false);
            mSoundDetector.Stop();
        }

        private void Init()
        {
            mTimer = 0.0f;
            mSoundDetector = mStatePatrolManager.DetectorManager.SoundDetector;
            mRaw = mStatePatrolManager.DebugSoundWindow.Raw;
            mGauge = mStatePatrolManager.DebugSoundWindow.GaugeSensibility;
            mMatShow = new Mat(480, 640, CvType.CV_8UC3);
            mTexture = new Texture2D(640, 480);
            mSoundDetector.Init();
            mHasInitSlider = false;
            mHasDetectedSound = false;
        }

        private void GoBack()
        {
            mAnimator.SetInteger("DebugMode", -1);
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