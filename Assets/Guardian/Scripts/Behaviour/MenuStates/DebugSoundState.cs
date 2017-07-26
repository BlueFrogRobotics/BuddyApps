using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OpenCVUnity;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Guardian
{
    public class DebugSoundState : AStateMachineBehaviour
    {

        //private SoundDetector mSoundDetector;
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
        private NoiseStimulus mNoiseStimulus;
        private DebugSoundWindow mDebugSoundWindow;

        private Queue<float> mSoundIntensities;
        private int mNbSoundPics = 50;

        public override void Start()
        {
            mTimer = 0.0f;
            //mSoundDetector = BYOS.Instance.Perception.SoundDetector;
            mDebugSoundWindow = GetGameObject(StateObject.DEBUG_SOUND).GetComponent<DebugSoundWindow>();
            mRaw = mDebugSoundWindow.Raw;
            mGauge = mDebugSoundWindow.GaugeSensibility;
            mMatShow = new Mat(480, 640, CvType.CV_8UC3);
            mTexture = new Texture2D(640, 480);
            //mSoundDetector.StartMic();
            mHasInitSlider = false;
            mHasDetectedSound = false;
            mGoBack = false;
            mDebugSoundAnimator = mDebugSoundWindow.gameObject.GetComponent<Animator>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //SetWindowAppOverBuddyColor(1);
            
            mAnimator = animator;
            Start();
            mSoundIntensities = new Queue<float>();
            for(int i=0; i<mNbSoundPics; i++)
            {
                mSoundIntensities.Enqueue(0.0f);
            }
            AStimulus soundStimulus;
            BYOS.Instance.Perception.Stimuli.Controllers.TryGetValue(StimulusEvent.NOISE_LOUD, out soundStimulus);
            mNoiseStimulus = (NoiseStimulus)soundStimulus;
            //mDebugSoundWindow.gameObject.SetActive(true);
            mDebugSoundAnimator.SetTrigger("Open_WDebugs");
            mDebugSoundWindow.ButtonBack.onClick.AddListener(GoBack);
            
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            if (mNoiseStimulus.IsListening)
            {
                mNoiseStimulus.Disable();
                mNoiseStimulus.Enable();
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!mHasInitSlider && mGauge.Slider)
            {
                mHasInitSlider = true;
                //mGauge.Slider.value = (1.0f - (mSoundDetector.Threshold / mSoundDetector.MaxThreshold)) * mGauge.Slider.maxValue;
                mGauge.Slider.value = GuardianData.Instance.SoundDetectionThreshold;//(1.0f - (mNoiseStimulus.Threshold / 0.3f)) * mGauge.Slider.maxValue;
            }

            mTimer += Time.deltaTime;
            if (mTimer > 0.04f)
            {
                mTimer = 0.0f;
                mMatShow = new Mat(480, 640, CvType.CV_8UC3, new Scalar(255, 255, 255, 255));

                float lMaxThreshold = 0.3f;// mSoundDetector.MaxThreshold;
                float lThreshold = (1.0f - mGauge.Slider.value / mGauge.Slider.maxValue) * lMaxThreshold;
                //mSoundDetector.Threshold = lThreshold;
                mNoiseStimulus.Threshold = lThreshold;
                Debug.Log("le thresh: " + lThreshold);

                float lLevelSound = mNoiseStimulus.Intensity * 400.0f / lMaxThreshold;// (mSoundDetector.Value) * 400.0f / lMaxThreshold;
                //Imgproc.line(mMatShow, new Point(0, 480.0f - lLevelSound), new Point(640, 480.0f - lLevelSound), new Scalar(0, 0, 255, 255));
                //Imgproc.rectangle(mMatShow, new Point(0, 480), new Point(640, 480.0f - lLevelSound), new Scalar(0, 212, 209, 255), -1);

                mSoundIntensities.Enqueue(lLevelSound);
                mSoundIntensities.Dequeue();
                float lWidthPic = 640.0f / mNbSoundPics;
                int lIt = 0;
                foreach(float intensity in mSoundIntensities)
                {
                    Imgproc.rectangle(mMatShow, new Point(lIt * lWidthPic, 480), new Point((lIt+1) *lWidthPic, 480.0f - intensity), new Scalar(0, 212, 209, 255), -1);
                    lIt++;
                }


                Imgproc.line(mMatShow, new Point(0, 480.0f - lThreshold * 400 / lMaxThreshold), new Point(640, 480.0f - lThreshold * 400 / lMaxThreshold), new Scalar(237, 27, 36, 255), 3);
                //Debug.Log("niveau: " + lLevelSound);

                Utils.MatToTexture2D(mMatShow, mTexture);
                mRaw.texture = mTexture;
                if (mHasDetectedSound)
                {
                    mHasDetectedSound = false;
                    mDebugSoundWindow.Ico.enabled = true;
                }
                else
                    mDebugSoundWindow.Ico.enabled = false;
            }

            if (mHasInitSlider && mDebugSoundAnimator.GetCurrentAnimatorStateInfo(0).IsName("Window_Debugs_Off") && mGoBack)
            {
                mAnimator.SetInteger("DebugMode", -1);
                GuardianData.Instance.SoundDetectionThreshold = 10 - (int)(mNoiseStimulus.Threshold * 10 / 0.3f);
                mGoBack = false;
                mDebugSoundWindow.Ico.enabled = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("noise stimu thresh: " + mNoiseStimulus.Threshold);
            GuardianData.Instance.SoundDetectionThreshold = 10-(int)(mNoiseStimulus.Threshold * 10 / 0.3f);//(int)mNoiseStimulus.Threshold;
            Debug.Log("threshold son: " + GuardianData.Instance.SoundDetectionThreshold);
            mDebugSoundWindow.Ico.enabled = false;
            //mSoundDetector.OnDetection -= OnSoundDetected;
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.NOISE_LOUD, OnSoundDetected);
            mDebugSoundWindow.ButtonBack.onClick.RemoveAllListeners();
            
            //mDebugSoundWindow.gameObject.SetActive(false);
            //mSoundDetector.Stop();
        }


        private void GoBack()
        {
            mDebugSoundAnimator.SetTrigger("Close_WDebugs");
            mGoBack = true;
        }

        private void OnSoundDetected()
        {
            mHasDetectedSound = true;
            Debug.Log("son detecte");
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