using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.Tutorial
{
    /// <summary>
    /// In this state we display what buddy sees and we will detect motion
    /// </summary>
    public class MotionState : AStateMachineBehaviour
    {
        private Mat mMatSrc;
        private Mat mMatDest;
        private Texture2D mTextPhoto;
        private bool mDisplayed;
        private Color mColorOfDisplay;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            mDisplayed = false;
            mColorOfDisplay = new Color(255, 0, 0);
           //initialize your texture to avoid nullref and crashes
            mTextPhoto = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            Buddy.Vocal.SayKey("motionstateintro");

            //You get the frame from the RGBCamera with the OnNewFrame
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            //You change parameters for your OnDetect : regionOfInterest represents where you want to do your detection in the current frame
            //and sensibilityThreshold represents your threshold for the detection
            MotionDetectorParameter mMotionDetectorParameter = new MotionDetectorParameter();
            mMotionDetectorParameter.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 640, 480);
            mMotionDetectorParameter.SensibilityThreshold = 2.5F;
            //OnDetect open the camera itself so you don't have to do it and the reseolution is 640*480 by default
            Buddy.Perception.MotionDetector.OnDetect.AddP(OnMovementDetected, mMotionDetectorParameter);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.Vocal.IsBusy && !mDisplayed)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextPhoto, OnDisplayClicked);
                mDisplayed = true;
            }
            else if (mDisplayed)
            {
                Buddy.Vocal.SayKey("motionstatedisplayed");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        /// <summary>
        /// Callback for every frame updated
        /// </summary>
        /// <param name="iInput"></param>
        private void OnFrameCaptured(Mat iInput)
        {
            //Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
            Mat lTest = iInput.clone();
            mMatSrc = lTest.clone();
            Core.flip(mMatSrc, mMatSrc, 1);
            mTextPhoto = Utils.ScaleTexture2DFromMat(mMatSrc, mTextPhoto);
            Utils.MatToTexture2D(mMatSrc, mTextPhoto);
        }

        /// <summary>
        /// Callback when the is movement detected
        /// </summary>
        /// <param name="iMotions"></param>
        /// <returns></returns>
        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            Core.flip(mMatSrc, mMatSrc, 1);
            //Draw circle on every motions detected in the image.
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatSrc, Utils.Center(lEntity.RectInFrame), 3, new Scalar(mColorOfDisplay), 3);
            }
            Core.flip(mMatSrc, mMatSrc, 1);
            mTextPhoto = Utils.ScaleTexture2DFromMat(mMatSrc, mTextPhoto);
            Utils.MatToTexture2D(mMatSrc, mTextPhoto);

            return true;
        }

        private void OnDisplayClicked()
        {
            Buddy.GUI.Toaster.Hide();
            Trigger("MenuTrigger");
        }

    }
}

