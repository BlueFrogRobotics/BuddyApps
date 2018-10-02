using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;

namespace BuddyApp.HumanCounter
{
    enum ViewMode : int
    {
        COUNTER_MODE,
        VIDEO_MODE,
    }

    // The number of frame use to calcul the average of human detected.
    enum AverageFrameNumber : int
    {
        TINY = 2,
        MEDIUM = 6,
    }

    public sealed class HumanCounterState : AStateMachineBehaviour
    {
        private int humanCounter;
        private int currentHumanCount;

        private List<int> sampleCount = new List<int> { };
        private int averageMemory;

        private float observationTimeStamp;
        private float detectTimeStamp;
        private float resetTimer = 0.200F;

        private bool displayed;
        private ViewMode viewMode;
        private Mat mMatSrc;
        private Mat matDetect;
        private Texture2D camView;
        private Color detectColor;


        // Enable or Disable the using of removeP function - /!\ on windows crash of unity are possible
        private bool windows = false;

        private bool humanDetectEnable;
        private bool isInit = false;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            viewMode = ViewMode.VIDEO_MODE;
            displayed = false;
            detectColor = new Color(255, 0, 0);
            observationTimeStamp = Time.time;
            detectTimeStamp = Time.time;
            humanCounter = 0;
            currentHumanCount = 0;
            averageMemory = 0;

            // Set windows to true: initialize the callback juste once, false: at every OnStateEnter
            if (!isInit || !windows)
            {
                Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect);
                isInit = true;
            }

            // Enable the code inside the callback
            humanDetectEnable = true;

            // Initialize texture
            camView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);

            // The matrix is send to OnNewFrame
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

            // Hide the default parameter button
            Buddy.GUI.Header.DisplayParametersButton(false);

            // Custom Font (Not working because of a bug - wait for bug fix)
            Font headerFont = Buddy.Resources.Get<Font>("os_awesome");
            headerFont.material.color = new Color(0F, 0F, 0F, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(headerFont);

            string field_counter = Buddy.Resources.GetString("realtimecount") + currentHumanCount + " ";
            field_counter += Buddy.Resources.GetString("totalhuman") + humanCounter;
            Buddy.GUI.Header.DisplayLightTitle(field_counter);
            
            // Create the top left button to switch between count mode and video mode
            FButton viewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            viewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            viewModeButton.OnClick.Add(() =>
            {
                if (viewMode == ViewMode.VIDEO_MODE)
                    viewMode = ViewMode.COUNTER_MODE;
                else
                    viewMode = ViewMode.VIDEO_MODE;
            });
        }

        private void timer_handler()
        {
            // If the observation time is reach, back to the settings states
            if ((Time.time - observationTimeStamp) >= HumanCounterData.Instance.observationTime)
            {
                if (!Buddy.Behaviour.IsBusy)
                    Trigger("BackToSettings");
            }

            // Reset real time counter if OnHumanDetect is not call since resetTimer
            if ((Time.time - detectTimeStamp) >= resetTimer)
            {
                currentHumanCount = 0;
                if (sampleCount.Count < (int)AverageFrameNumber.MEDIUM)
                    sampleCount.Add(currentHumanCount);

                // Refresh the header
                string field_counter = Buddy.Resources.GetString("realtimecount") + currentHumanCount + " ";
                field_counter += Buddy.Resources.GetString("totalhuman") + humanCounter;
                Buddy.GUI.Header.DisplayLightTitle(field_counter);
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer_handler();
            // Calcul the average of human on a sample of frame
            if (sampleCount.Count == (int)AverageFrameNumber.MEDIUM)
            {
                int currentAverage = 0;

                foreach (int numb in sampleCount)
                    currentAverage += numb;
                currentAverage /= (int)AverageFrameNumber.MEDIUM;
                if (currentAverage > averageMemory)
                    humanCounter += currentAverage - averageMemory;
                averageMemory = currentAverage;
                sampleCount.Clear();
            }

            // Reset mood to neutral when nobody is detect or surprised if someone is detect
            if (currentHumanCount == 0 && !Buddy.Behaviour.IsBusy)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL, true);
            if (currentHumanCount > 0 && !Buddy.Behaviour.IsBusy)
                Buddy.Behaviour.SetMood(Mood.SURPRISED, true);

            // Video Mode: Display the camera view with a visual of detection
            if (viewMode == ViewMode.VIDEO_MODE && !displayed)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(camView);
                displayed = true;
            }
            // Counter mode: Display Buddy's face
            if (viewMode == ViewMode.COUNTER_MODE && displayed)
            {
                Buddy.GUI.Toaster.Hide();
                displayed = false;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            // The code in OnHumanDetect is disable but the callback is still running if windows is true
            humanDetectEnable = false;
            // The removeP function is in work in progress - set windows to false to run on android
            if (!windows)
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
        }

        //  -----CALLBACK------  //

        private void OnFrameCaptured(Mat iInput)
        {
            //Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
            mMatSrc = iInput.clone();
            matDetect = iInput.clone();
            // Flip to avoid mirror effect
            Core.flip(mMatSrc, mMatSrc, 1);
            // Use matrice format, to scale the texture
            camView = Utils.ScaleTexture2DFromMat(mMatSrc, camView);
            // Use matrice to fill the texture
            Utils.MatToTexture2D(mMatSrc, camView);
        }

        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if ((!humanDetectEnable && windows) || sampleCount.Count == (int)AverageFrameNumber.MEDIUM)
                return true;

            // Refresh the header
            string field_counter = Buddy.Resources.GetString("realtimecount") + currentHumanCount + " ";
            field_counter += Buddy.Resources.GetString("totalhuman") + humanCounter;
            Buddy.GUI.Header.DisplayLightTitle(field_counter);

            currentHumanCount = iHumans.Length;
            detectTimeStamp = Time.time;
            foreach (HumanEntity human in iHumans) {
                Imgproc.rectangle(matDetect, human.BoundingBox.tl(), human.BoundingBox.br(), new Scalar(detectColor));
            }
            sampleCount.Add(currentHumanCount);
            Core.flip(matDetect, matDetect, 1);
            camView = Utils.ScaleTexture2DFromMat(matDetect, camView);
            Utils.MatToTexture2D(matDetect, camView);
            return true;
        }
    }
}
