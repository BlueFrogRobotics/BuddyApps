using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.HumanCounter
{
    public sealed class HumanCounterState : AStateMachineBehaviour
    {
        // The number of frame use to calcul the average of human detected.
        private const int AVERAGE_FRAME_NUMBER = 6;

        private int mHumanCounter;
        private int mCurrentHumanCount;

        private List<int> mSampleCount;
        private int mAverageMemory;

        private float mObservationTimeStamp;
        private float mDetectTimeStamp;
        private float mResetTimer;

        private bool mDefaultHeader;
        private bool mDisplayed;
        private bool mVideoMode;
        private Mat mMatDetect;
        private Texture2D mCamView;

        /*
        *   Temporary variable, used to deal with some feature in WIP.
        *   On Windows crash of unity are possible, with removeP.
        *   mWindows is used to enable or disable removeP, quickly.
        *   mHumanDetectEnable is use to enable or disable the code in the callback.
        *   mIsInit is used to initialize just once the callback, if mWindows is true.
        */
        private bool mWindows;
        private bool mHumanDetectEnable;
        private bool mIsInit;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Initialization - By default the app is open in video mode.
            mVideoMode = true;
            mDefaultHeader = true;
            mDisplayed = false;
            mObservationTimeStamp = Time.time;
            mDetectTimeStamp = Time.time;
            mHumanCounter = 0;
            mCurrentHumanCount = 0;
            mAverageMemory = 0;
            mSampleCount = new List<int> { };
            mIsInit = false;
            mHumanDetectEnable = true;
            mWindows = true;
            // The mCurrentHumanCount is reset every 200ms.
            mResetTimer = 0.200F;
            // Set mWindows to true: initialize the callback juste once, false: at every OnStateEnter.
            if (!mIsInit || !mWindows) {
                Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect);
                mIsInit = true;
            }
            // Initialize texture.
            mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // The matrix is send to OnNewFrame.
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            // Hide the default parameter button.
            Buddy.GUI.Header.DisplayParametersButton(false);
            // Custom Font (Not working because of a bug - wait for bug fix).
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0F, 0F, 0F, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
            lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
            Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
            // Create the top left button to switch between count mode and video mode.
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() => { mVideoMode = !mVideoMode; });
        }

        private void TimerHandler()
        {
            // If the observation time is reach, back to the settings states.
            if ((Time.time - mObservationTimeStamp) >= HumanCounterData.Instance.observationTime) {
                if (!Buddy.Behaviour.IsBusy)
                    Trigger("BackToSettings");
            }
            // Reset real time counter if OnHumanDetect is not call since mResetTimer.
            if ((Time.time - mDetectTimeStamp) >= mResetTimer) {
                mCurrentHumanCount = 0;
                // If nobody is detect, it's important to continue to take sample, to keep consistency of the average.
                if (mSampleCount.Count < AVERAGE_FRAME_NUMBER)
                    mSampleCount.Add(mCurrentHumanCount);
                // Refresh the header.
                if (!mDefaultHeader) {
                    string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
                    lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
                    Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
                    mDefaultHeader = true;
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TimerHandler();
            // Calcul the average of human on a sample of frame.
            if (mSampleCount.Count == AVERAGE_FRAME_NUMBER) {
                int lCurrentAverage = 0;
                foreach (int lNumb in mSampleCount)
                    lCurrentAverage += lNumb;
                lCurrentAverage /= AVERAGE_FRAME_NUMBER;
                if (lCurrentAverage > mAverageMemory)
                    mHumanCounter += lCurrentAverage - mAverageMemory;
                mAverageMemory = lCurrentAverage;
                mSampleCount.Clear();
            }
            // Reset mood to neutral when nobody is detect or surprised if someone is detect.
            if (Buddy.Behaviour.Mood != Mood.NEUTRAL && mCurrentHumanCount == 0 && !Buddy.Behaviour.IsBusy)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL, true);
            if (Buddy.Behaviour.Mood != Mood.SURPRISED && mCurrentHumanCount > 0 && !Buddy.Behaviour.IsBusy)
                Buddy.Behaviour.SetMood(Mood.SURPRISED, true);
            // Video Mode: Display the camera view with a visual of detection.
            if (mVideoMode && !mDisplayed) {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCamView);
                mDisplayed = true;
            }
            // If the video mode is disable: Buddy's face is display.
            if (!mVideoMode && mDisplayed) {
                Buddy.GUI.Toaster.Hide();
                mDisplayed = false;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            // The code in OnHumanDetect is disable but the callback is still running if mWindows is true.
            mHumanDetectEnable = false;
            // The removeP function is in work in progress - set mWindows to false to run on android.
            if (!mWindows)
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
        }

        //  -----CALLBACK------  //

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        private void OnFrameCaptured(Mat iInput)
        {
            Mat lMatSrc;
           //Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
            lMatSrc = iInput.clone();
            mMatDetect = iInput.clone();
            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mCamView);
        }

        /*
        *   On a human detection this function is called.
        *   mHumanDetectEnable: Enable or disable the code when mWindows is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if ((!mHumanDetectEnable && mWindows) || mSampleCount.Count == AVERAGE_FRAME_NUMBER)
                return true;
            // Refresh the header.
            string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
            lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
            Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
            mCurrentHumanCount = iHumans.Length;
            mDetectTimeStamp = Time.time;
            // Reset the display of the header if nobody is detect.
            mDefaultHeader = false;
            foreach (HumanEntity human in iHumans)
                Imgproc.rectangle(mMatDetect, human.BoundingBox.tl(), human.BoundingBox.br(), new Scalar(new Color(255, 0, 0)));
            mSampleCount.Add(mCurrentHumanCount);
            Core.flip(mMatDetect, mMatDetect, 1);
            mCamView = Utils.ScaleTexture2DFromMat(mMatDetect, mCamView);
            Utils.MatToTexture2D(mMatDetect, mCamView);
            return true;
        }
    }
}
