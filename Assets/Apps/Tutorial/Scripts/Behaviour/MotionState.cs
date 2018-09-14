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
        private MotionDetector mMotion;
        private Mat mMatSrc;
        private Mat mMatCopy;
        private Mat mMatDest;
        private Texture2D mTextPhoto;
        private bool mDisplayed;
        private Color mColorOfDisplay;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            mDisplayed = false;
            mColorOfDisplay = new Color(255,0,0);
            if (Buddy.Sensors.HDCamera.IsBusy)
                Buddy.Sensors.HDCamera.Close();
            if (!Buddy.Sensors.HDCamera.IsOpen)
            {
                Buddy.Sensors.HDCamera.Open(HDCameraMode.COLOR_640x480_30FPS_RGB);
            }
            Buddy.Vocal.SayKey("motionstateintro");
            
            mMatSrc = Buddy.Sensors.HDCamera.Frame;
            Core.flip(mMatSrc, mMatSrc, 1);
            mTextPhoto = Utils.MatToTexture2D(Buddy.Sensors.HDCamera.Frame);
            Buddy.Sensors.HDCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            Buddy.Perception.MotionDetector.OnDetect.AddP(OnMovementDetected, new MotionDetectorParameter()
            {
                RegionOfInterest = new OpenCVUnity.Rect(0, 0, Buddy.Sensors.HDCamera.Width, Buddy.Sensors.HDCamera.Height),
                SensibilityThreshold = 3.0F
            });
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!Buddy.Vocal.IsBusy && !mDisplayed /*mMotionStep == MotionStep.DISPLAY_CAMERA*/)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mTextPhoto);
                mDisplayed = true;
            }
            else if (mDisplayed)
            {
                
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void OnFrameCaptured(Mat iInput)
        {
            mMatSrc = iInput;
            Core.flip(mMatSrc, mMatSrc, 1);
            Utils.MatToTexture2D(mMatSrc, mTextPhoto);
        }

        private bool OnMovementDetected(MotionEntity[] iMotions)
        {
            Debug.Log("Movement detected");
            foreach (MotionEntity lEntity in iMotions)
            {
                Imgproc.circle(mMatSrc, Utils.Center(lEntity.RectInFrame), 3, new Scalar(mColorOfDisplay), 3);
            }
            Utils.MatToTexture2D(mMatDest, mTextPhoto);

            return true;
        }
    }
}

