using UnityEngine.UI;
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
        private HDCamera mCam;
        private Mat mMatSrc;

        private FButton mLeftButton;
        private FButton mValidateButton;
        private FLabeledHorizontalSlider mSlider;

        private MotionDetector mMovementTracker;

        public override void Start()
        {

        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("selectsensibility"));

            if (Buddy.Sensors.HDCamera.IsBusy)
            {
                Buddy.Sensors.HDCamera.Close();
            }
            if (!Buddy.Sensors.HDCamera.IsOpen)
            {
                Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            }
            mTexture = Utils.MatToTexture2D(Buddy.Sensors.HDCamera.Frame);
            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);

            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTexture);

            mSlider = Buddy.GUI.Footer.CreateOnMiddle<FLabeledHorizontalSlider>();
            mSlider.SlidingValue = GuardianData.Instance.MovementDetectionThreshold;
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

            mMovementTracker = Buddy.Perception.MotionDetector;
            mMovementTracker.OnDetect.AddP(OnMovementDetected);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.HideTitle();
        }

        private void OnFrameCaptured(Mat iInput)
        {
            mMatSrc = iInput;
            Core.flip(mMatSrc, mMatSrc, 1);
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
            

            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatSrc, Utils.Center(lEntity.RectInFrame), 3, new Scalar(255, 0, 0), -1);
            }

            Mat lMat = new Mat();
            Core.flip(mMatSrc, lMat, 1);
            Utils.MatToTexture2D(lMat, Utils.ScaleTexture2DFromMat(lMat, mTexture));

            //if (iMotions.Length > 0)
            //    mHasDetectedMouv = true;
            return true;
        }
    }
}