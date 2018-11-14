using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.HumanCounter
{
    public sealed class HumanCounterState : AStateMachineBehaviour
    {

        private float COEFF_X;
        private float COEFF_Y;

        // The number of frame use to calcul the average of human detected.
        private const int AVERAGE_FRAME_NUMBER = 6;

        private int mHumanCounter;
        private int mCurrentHumanCount;

        private List<OpenCVUnity.Rect> mDetectedBox;
        private List<SkeletonJoint[]> mListJoint;
        private List<int> mSampleCount;
        private int mAverageMemory;

        private float mObservationTimeStamp;
        private float mDetectTimeStamp;
        private float mResetTimer;

        private bool mDefaultHeader;
        private bool mDisplayed;
        private bool mVideoMode;
        private Texture2D mCamView;

        /*
        *   Temporary variable, used to deal with some feature in WIP.
        *   On Windows crash of unity are possible, with removeP.
        *   WINDOWS is used to enable or disable removeP, quickly.
        *   mHumanDetectEnable & mFaceDetectEnable are use to enable or disable the code in each callback.
        */
        private const bool WINDOWS = false;
        private bool mHumanDetectEnable;
        private bool mFaceDetectEnable;
        private bool mSkeletonDetectEnable;
        private List<Scalar> mColor = new List<Scalar> { };


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            COEFF_X = 1.7F;
            COEFF_Y = 2.45F;
            mHumanDetectEnable = false;
            mFaceDetectEnable = false;
            mSkeletonDetectEnable = false;
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
            mDetectedBox = new List<OpenCVUnity.Rect> { };
            mListJoint = new List<SkeletonJoint[]> { };

            for (int j = 0; j < 20; ++j)
            {
                double r = UnityEngine.Random.Range(0, 256);
                double g = UnityEngine.Random.Range(0, 256);
                double b = UnityEngine.Random.Range(0, 256);
                mColor.Add(new Scalar(r, g, b));
            }

            // The mCurrentHumanCount is reset every 200ms.
            mResetTimer = 0.200F;
            // Set WINDOWS to true: initialize the callback juste once, false: at every OnStateEnter.
            if (HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
            {
                mHumanDetectEnable = true;
                OpenCVUnity.Rect lRoi = new OpenCVUnity.Rect(0, 0, 0, 0);
                HumanDetectorParameter lParameters = new HumanDetectorParameter();
                lParameters.RegionOfInterest = lRoi;
                lParameters.UseThermal = false;
                if ((Buddy.Perception.HumanDetector.OnDetect.Count == 0 || !WINDOWS))
                    Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect, lParameters);
            }
            else if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT)
            {
                mFaceDetectEnable = true;
                if ((Buddy.Perception.FaceDetector.OnDetect.Count == 0 || !WINDOWS))
                    Buddy.Perception.FaceDetector.OnDetect.AddP(OnFaceDetect);
            }
            else
            {
                mSkeletonDetectEnable = true;
                if ((Buddy.Perception.SkeletonDetector.OnDetect.Count == 0 || !WINDOWS))
                {
                    Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320x240_30FPS_RGB);
                    Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetect);

                }
            }
            // Initialize texture.
            mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // The matrix is send to OnNewFrame.
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            // Hide the default parameter button.
            Buddy.GUI.Header.DisplayParametersButton(false);
            // Custom Font (Not working because of a bug - wait for bug fix).
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0, 1F);
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
            if ((Time.time - mObservationTimeStamp) >= HumanCounterData.Instance.ObservationTime)
            {
                if (!Buddy.Behaviour.IsBusy)
                    Trigger("BackToSettings");
            }
            // Reset real time counter if OnHumanDetect is not call since mResetTimer.
            if ((Time.time - mDetectTimeStamp) >= mResetTimer)
            {
                // Clear all old box, from the last detection
                mDetectedBox.Clear();
                //mListJoint.Clear();
                mCurrentHumanCount = 0;
                // If nobody is detect, it's important to continue to take sample, to keep consistency of the average.
                if (mSampleCount.Count < AVERAGE_FRAME_NUMBER)
                    mSampleCount.Add(mCurrentHumanCount);
                // Refresh the header.
                if (!mDefaultHeader)
                {
                    string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
                    lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
                    Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
                    mDefaultHeader = true;
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Input.GetKey("a"))
            {
                COEFF_Y = COEFF_Y + 0.01F;
                Debug.Log("ADD Y : " + COEFF_Y);
            }
            else if (Input.GetKey("z"))
            {
                COEFF_Y = COEFF_Y - 0.01F;
                Debug.Log("REMOVE Y : " + COEFF_Y);
            }
            TimerHandler();
            // Calcul the average of human on a sample of frame.
            if (mSampleCount.Count == AVERAGE_FRAME_NUMBER)
            {
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
            if (mVideoMode && !mDisplayed)
            {
                Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCamView);
                mDisplayed = true;
            }

            // If the video mode is disable: Buddy's face is display.
            if (!mVideoMode && mDisplayed)
            {
                Buddy.GUI.Toaster.Hide();
                mDisplayed = false;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mColor.Clear();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            // The code in OnHumanDetect is disable but the callback is still running if WINDOWS is true.
            mHumanDetectEnable = false;
            mFaceDetectEnable = false;
            mSkeletonDetectEnable = false;
            // The removeP function is in work in progress - set WINDOWS to false to run on android.
            if (!WINDOWS)
            {
                if (HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
                    Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                else if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT)
                    Buddy.Perception.FaceDetector.OnDetect.RemoveP(OnFaceDetect);
                else if (HumanCounterData.Instance.DetectionOption == DetectionOption.SKELETON_DETECT)
                    Buddy.Perception.SkeletonDetector.OnDetect.RemoveP(OnSkeletonDetect);
            }
        }

        //  -----CALLBACK------  //

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        private void OnFrameCaptured(Mat iInput)
        {
            Mat lMatSrc;
            // Always clone the input matrix to avoid working with the matrix when the C++ part wants to modify it. It will crash.
            lMatSrc = iInput.clone();
            // Adding of each box on the frame
            if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT || HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
            {
                foreach (OpenCVUnity.Rect lBox in mDetectedBox)
                    Imgproc.rectangle(lMatSrc, lBox.tl(), lBox.br(), new Scalar(new Color(255, 0, 0)));
            }
            else
            {
                if(Buddy.Sensors.RGBCamera.IsOpen)
                {
                    //for (int k = 0; k < Math.Min(mListJoint.Count, 2); ++k)
                    foreach(var skeleton in mListJoint)
                    {
                        int lWidth = lMatSrc.cols();
                        int lHeight = lMatSrc.rows();
                        //for (int i = 0; i < mListJoint[k].Length; ++i)
                        foreach (var lJoint in skeleton)
                        {
                            //var lJoint = mListJoint[k][i];
                            Point lCenter = new Point(lWidth / 2, lHeight / 2);
                            Point lLocal = new Point(lJoint.WorldPosition.x / lJoint.WorldPosition.z, lJoint.WorldPosition.y / lJoint.WorldPosition.z);
                            lLocal.x *= COEFF_X * lWidth / 2;
                            lLocal.y *= COEFF_Y * lHeight / 2;

                            //for (int j = 0; j < mListJoint[k].Length; j++)
                            foreach(var lSecondJoint in skeleton)
                            {
                                //var lSecondJoint = mListJoint[k][j];
                                Point lSecondLocal = new Point(lSecondJoint.WorldPosition.x / lSecondJoint.WorldPosition.z, lSecondJoint.WorldPosition.y / lSecondJoint.WorldPosition.z);
                                lSecondLocal.x *= COEFF_X * lWidth / 2;
                                lSecondLocal.y *= COEFF_Y * lHeight / 2;
                                #region JOINTS_LINKED

                                //if (lJoint.Type != SkeletonJointType.UNKNOWN || lSecondJoint.Type != SkeletonJointType.UNKNOWN)
                                //{
                                //    if (lJoint.Type == SkeletonJointType.HEAD && lSecondJoint.Type == SkeletonJointType.SHOULDER_SPINE)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }

                                //    //RIGHT ARM
                                //    if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.RIGHT_SHOULDER)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.RIGHT_SHOULDER && lSecondJoint.Type == SkeletonJointType.RIGHT_ELBOW)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.RIGHT_ELBOW && lSecondJoint.Type == SkeletonJointType.RIGHT_WRIST)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.RIGHT_WRIST && lSecondJoint.Type == SkeletonJointType.RIGHT_HAND)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }

                                //    //LEFT ARM
                                //    if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.LEFT_SHOULDER)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.LEFT_SHOULDER && lSecondJoint.Type == SkeletonJointType.LEFT_ELBOW)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.LEFT_ELBOW && lSecondJoint.Type == SkeletonJointType.LEFT_WRIST)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.LEFT_WRIST && lSecondJoint.Type == SkeletonJointType.LEFT_HAND)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }



                                //    if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.MID_SPINE)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.MID_SPINE && lSecondJoint.Type == SkeletonJointType.BASE_SPINE)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }

                                //    //RIGHT LEG
                                //    if (lJoint.Type == SkeletonJointType.BASE_SPINE && lSecondJoint.Type == SkeletonJointType.RIGHT_HIP)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.RIGHT_HIP && lSecondJoint.Type == SkeletonJointType.RIGHT_KNEE)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.RIGHT_KNEE && lSecondJoint.Type == SkeletonJointType.RIGHT_FOOT)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }

                                //    //LEFT LEG
                                //    if (lJoint.Type == SkeletonJointType.BASE_SPINE && lSecondJoint.Type == SkeletonJointType.LEFT_HIP)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.LEFT_HIP && lSecondJoint.Type == SkeletonJointType.LEFT_KNEE)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //    if (lJoint.Type == SkeletonJointType.LEFT_KNEE && lSecondJoint.Type == SkeletonJointType.LEFT_FOOT)
                                //    {
                                //        Imgproc.line(lMatSrc, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                                //    }
                                //}
                                #endregion
                            }

                            Imgproc.circle(lMatSrc, lCenter - lLocal, (int)(10 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)), new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                        }
                    }
                }


            }
            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mCamView);

        }

        /*
        *   On a human detection this function is called.
        *   mHumanDetectEnable: Enable or disable the code when WINDOWS is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if ((!mHumanDetectEnable && WINDOWS) || mSampleCount.Count == AVERAGE_FRAME_NUMBER)
                return true;
            // Clear all old box, from the last detection
            mDetectedBox.Clear();
            // Refresh the header.
            string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
            lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
            Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
            mCurrentHumanCount = iHumans.Length;
            mDetectTimeStamp = Time.time;
            // Reset the display of the header if nobody is detect.
            mDefaultHeader = false;
            // We add each box to a list, to display them later in OnNewFrame
            foreach (HumanEntity lHuman in iHumans)
                mDetectedBox.Add(new OpenCVUnity.Rect(lHuman.BoundingBox.tl(), lHuman.BoundingBox.br()));
            // We add a measure to the list of sample
            mSampleCount.Add(mCurrentHumanCount);
            return true;
        }

        /*
        *   On a face detection this function is called.
        *   mFaceDetectEnable: Enable or disable the code when WINDOWS is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private bool OnFaceDetect(FaceEntity[] iFaces)
        {
            if ((!mFaceDetectEnable && WINDOWS) || mSampleCount.Count == AVERAGE_FRAME_NUMBER)
                return true;
            // Clear all old box, from the last detection
            mDetectedBox.Clear();
            // Refresh the header.
            string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
            lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
            Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
            mCurrentHumanCount = iFaces.Length;
            mDetectTimeStamp = Time.time;
            // Reset the display of the header if nobody is detect.
            mDefaultHeader = false;
            // We add each box to a list, to display them later in OnNewFrame
            foreach (FaceEntity lFace in iFaces)
                mDetectedBox.Add(new OpenCVUnity.Rect(lFace.BoundingBox.tl(), lFace.BoundingBox.br()));
            // We add a measure to the list of sample
            mSampleCount.Add(mCurrentHumanCount);
            return true;
        }

        /*
        *   On a skeleton detection this function is called.
        *   mSkeletonDetectEnable: Enable or disable the code when WINDOWS is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private bool OnSkeletonDetect(SkeletonEntity[] iSkeleton)
        {
            if ((!mSkeletonDetectEnable && WINDOWS) || mSampleCount.Count == AVERAGE_FRAME_NUMBER)
                return true;
            mListJoint.Clear();
            // Refresh the header.
            string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
            lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
            Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
            mCurrentHumanCount = iSkeleton.Length;
            mDetectTimeStamp = Time.time;
            // Reset the display of the header if nobody is detect.
            mDefaultHeader = false;
            // We add each box to a list, to display them later in OnNewFrame
            foreach (SkeletonEntity lSkeleton in iSkeleton)
            {

                mListJoint.Add(lSkeleton.Joints);
            }
            // We add a measure to the list of sample
            mSampleCount.Add(mCurrentHumanCount);
            return true;
        }
    }
}
