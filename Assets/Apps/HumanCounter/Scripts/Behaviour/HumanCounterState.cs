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
        // Coefficient to adjust all skeleton point, on the image.
        private float COEFF_X;
        private float COEFF_Y;

        // The number of frame use to calcul the average of human detected.
        private const int AVERAGE_FRAME_NUMBER = 6;
        // The number max of skeleton detect by perception
        private const int MAX_SKELETON_DETECT = 2;
        // Real time counter reset frequency in second
        private const float RESET_COUNTER_TIMER = 0.200F;

        // Total of count
        private int mHumanCounter;
        // Real time count
        private int mCurrentHumanCount;

        // List of box were face or human are detect in
        private List<OpenCVUnity.Rect> mDetectedBox;
        // List of Skeleton detect
        private List<SkeletonJoint[]> mListJoint;
        // Object detect are count one time per frame, and, during several frame.
        private List<int> mSampleCount;
        // The calcul of the average of each count in mSampleCount.
        private int mAverageMemory;

        // Time memory to handle each timer
        private float mObservationTimeStamp;
        private float mDetectTimeStamp;

        // Header have to be refreshed at each detection
        private bool mHeaderRefreshed;
        // Variable to avoid multiple display
        private bool mDisplayed;
        // Variable to choose between video view & buddy's face view
        private bool mVideoMode;
        // This texture will be filled with the camera data
        private Texture2D mCamView;
        // Color array to use for each skeleton
        private Color[] mColor;

        /*
        *   Keep WINDOWS to false during the sprint y18w48.
        *   The solution to disable code on Windows have to be removed at the end of the sprint if no crash were encountered.
        *   
        *   Temporary variable, used to deal with some feature in WIP.
        *   On Windows crash of unity are possible, with removeP.
        *   WINDOWS is used to enable or disable removeP, quickly.
        *   mHumanDetectEnable, mFaceDetectEnable & mSkeletonDetectEnable are use to enable or disable the code in each callback.
        */
        private const bool WINDOWS = false;
        private bool mHumanDetectEnable;
        private bool mFaceDetectEnable;
        private bool mSkeletonDetectEnable;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            COEFF_X = 1.7F;
            COEFF_Y = 2.45F;

            mHumanDetectEnable = false;
            mFaceDetectEnable = false;
            mSkeletonDetectEnable = false;

            // Initialization - By default the app is open in video mode. - The other mode is Buddy's face with the counter view
            mVideoMode = true;
            mHeaderRefreshed = true;
            mDisplayed = false;

            mObservationTimeStamp = Time.time;
            mDetectTimeStamp = Time.time;

            mHumanCounter = 0;
            mCurrentHumanCount = 0;
            mAverageMemory = 0;
            mSampleCount = new List<int> { };

            mDetectedBox = new List<OpenCVUnity.Rect> { };
            mListJoint = new List<SkeletonJoint[]> { };
            mColor = new Color[MAX_SKELETON_DETECT];

            // Fill color array with random color
            for (int j = 0; j < mColor.Length; ++j)
            {
                float r = UnityEngine.Random.Range(0, 256);
                float g = UnityEngine.Random.Range(0, 256);
                float b = UnityEngine.Random.Range(0, 256);
                mColor[j] = new Color(r, g, b);
            }

            // Setting of the detection, depending of what object the user want to detect
            if (HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
            {
                mHumanDetectEnable = true;
                // Creation & Settings of parameters that will be used in detection
                HumanDetectorParameter lParameters = new HumanDetectorParameter();
                lParameters.SensorMode = SensorMode.VISION;
                lParameters.YOLO = new YOLOParameter();
                // Region of Interest for the tracking, 0 rectangle will use all the field of view.
                lParameters.YOLO.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 0, 0); ;
                // Also use thermal camera or not.
                lParameters.YOLO.UseThermal = false;
                // Callback to use on detection - remove the if statement when WINDOWS will be removed.
                if ((Buddy.Perception.HumanDetector.OnDetect.Count == 0 || !WINDOWS))
                    Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect, lParameters);
            }
            else if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT)
            {
                mFaceDetectEnable = true;
                // Callback to use on detection - remove the if statement when WINDOWS will be removed.
                if ((Buddy.Perception.FaceDetector.OnDetect.Count == 0 || !WINDOWS))
                    Buddy.Perception.FaceDetector.OnDetect.AddP(OnFaceDetect);
            }
            else
            {
                mSkeletonDetectEnable = true;
                // Callback to use on detection - remove the if statement when WINDOWS will be removed.
                if ((Buddy.Perception.SkeletonDetector.OnDetect.Count == 0 || !WINDOWS))
                {
                    // Skeleton detection doesn't open the camera by default
                    Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_30FPS_RGB);
                    Buddy.Perception.SkeletonDetector.OnDetect.AddP(OnSkeletonDetect);

                }
            }
            // Initialize texture.
            mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));

            // Hide the default parameter button.
            Buddy.GUI.Header.DisplayParametersButton(false);
            // Set Title with a custom font
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = Color.black;
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            // Display header title
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
            // Reset real time counter if OnHumanDetect is not call since RESET_COUNTER_TIMER seconds.
            if ((Time.time - mDetectTimeStamp) >= RESET_COUNTER_TIMER)
            {
                // Clear all old box, from the last detection
                mDetectedBox.Clear();
                //mListJoint.Clear();
                mCurrentHumanCount = 0;
                // If nobody is detect, it's important to continue to take sample, to keep consistency of the average.
                if (mSampleCount.Count < AVERAGE_FRAME_NUMBER)
                    mSampleCount.Add(mCurrentHumanCount);
                // Refresh the header if needed.
                if (!mHeaderRefreshed)
                {
                    string lFieldCounter = Buddy.Resources.GetString("realtimecount") + mCurrentHumanCount + " ";
                    lFieldCounter += Buddy.Resources.GetString("totalhuman") + mHumanCounter;
                    Buddy.GUI.Header.DisplayLightTitle(lFieldCounter);
                    mHeaderRefreshed = true;
                }
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TimerHandler();
            if (mSampleCount.Count == AVERAGE_FRAME_NUMBER)
            {
                int lCurrentAverage = 0;
                // Calcul the average of human on a sample of frame.
                foreach (int lNumb in mSampleCount)
                    lCurrentAverage += lNumb;
                lCurrentAverage /= AVERAGE_FRAME_NUMBER;
                // If the current average is higher than the last average - we consider that new object are detect, and we count them.
                if (lCurrentAverage > mAverageMemory)
                    mHumanCounter += lCurrentAverage - mAverageMemory;
                // Save the current average to compare it later
                mAverageMemory = lCurrentAverage;
                // Clear the sample
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
            mListJoint.Clear();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            // All following line have to be removed if no crash occured during sprint y18w48
            // The code in OnHumanDetect is disable but the callback is still running if WINDOWS is true.
            mHumanDetectEnable = false;
            mFaceDetectEnable = false;
            mSkeletonDetectEnable = false;
            Buddy.Sensors.RGBCamera.Close();
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
        private void OnFrameCaptured(RGBCameraFrame iInput)
        {
            // Always clone the input matrix, this avoid to working with the original matrix, when the C++ part wants to modify it.
            Mat lMatSrc = iInput.Mat.clone();

            // Drawing each box were detect something, on the frame.
            if (HumanCounterData.Instance.DetectionOption == DetectionOption.FACE_DETECT || HumanCounterData.Instance.DetectionOption == DetectionOption.HUMAN_DETECT)
            {
                foreach (OpenCVUnity.Rect lBox in mDetectedBox)
                    Imgproc.rectangle(lMatSrc, lBox.tl(), lBox.br(), new Scalar(new Color(255, 0, 0)));
            }
            // Drawing each links between skeleton joints.
            else
                DrawSkeletonLinks(lMatSrc);

            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mCamView);
        }

        // This function use all joints of a skeleton and draw a line between them
        private void DrawSkeletonLinks(Mat iMat)
        {
            int lSkeletonCount = 0;
            if (Buddy.Sensors.RGBCamera.IsOpen)
            {
                // We browse the skeleton list
                foreach (var skeleton in mListJoint)
                {
                    // Number max of skeleton to use for drawing
                    if (lSkeletonCount > MAX_SKELETON_DETECT)
                        break;
                    lSkeletonCount++;
                    int lWidth = iMat.cols();
                    int lHeight = iMat.rows();
                    // We browse all joints of the current skeleton
                    foreach (var lJoint in skeleton)
                    {
                        // Calcul the center of the img
                        Point lCenter = new Point(lWidth / 2, lHeight / 2);
                        // Calcul the local position of the joint
                        Point lLocal = new Point(lJoint.WorldPosition.x / lJoint.WorldPosition.z, lJoint.WorldPosition.y / lJoint.WorldPosition.z);
                        // Conversion of the local position, in the img
                        lLocal.x *= COEFF_X * lWidth / 2;
                        lLocal.y *= COEFF_Y * lHeight / 2;

                        // We browse all joints of the current skeleton a second time, to compare all joints between them
                        foreach (var lSecondJoint in skeleton)
                        {
                            // Calcul the local position of the joint
                            Point lSecondLocal = new Point(lSecondJoint.WorldPosition.x / lSecondJoint.WorldPosition.z, lSecondJoint.WorldPosition.y / lSecondJoint.WorldPosition.z);
                            // Conversion of the local position, in the img
                            lSecondLocal.x *= COEFF_X * lWidth / 2;
                            lSecondLocal.y *= COEFF_Y * lHeight / 2;
                            // Compare joints to know if a links have to be draw or not
                            #region JOINTS_LINKED

                            if (lJoint.Type != SkeletonJointType.UNKNOWN || lSecondJoint.Type != SkeletonJointType.UNKNOWN)
                            {
                                if (lJoint.Type == SkeletonJointType.HEAD && lSecondJoint.Type == SkeletonJointType.SHOULDER_SPINE)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);

                                //RIGHT ARM
                                if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.RIGHT_SHOULDER)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.RIGHT_SHOULDER && lSecondJoint.Type == SkeletonJointType.RIGHT_ELBOW)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.RIGHT_ELBOW && lSecondJoint.Type == SkeletonJointType.RIGHT_WRIST)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.RIGHT_WRIST && lSecondJoint.Type == SkeletonJointType.RIGHT_HAND)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);

                                //LEFT ARM
                                if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.LEFT_SHOULDER)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.LEFT_SHOULDER && lSecondJoint.Type == SkeletonJointType.LEFT_ELBOW)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.LEFT_ELBOW && lSecondJoint.Type == SkeletonJointType.LEFT_WRIST)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.LEFT_WRIST && lSecondJoint.Type == SkeletonJointType.LEFT_HAND)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);

                                if (lJoint.Type == SkeletonJointType.SHOULDER_SPINE && lSecondJoint.Type == SkeletonJointType.MID_SPINE)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.MID_SPINE && lSecondJoint.Type == SkeletonJointType.BASE_SPINE)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);

                                //RIGHT LEG
                                if (lJoint.Type == SkeletonJointType.BASE_SPINE && lSecondJoint.Type == SkeletonJointType.RIGHT_HIP)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.RIGHT_HIP && lSecondJoint.Type == SkeletonJointType.RIGHT_KNEE)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.RIGHT_KNEE && lSecondJoint.Type == SkeletonJointType.RIGHT_FOOT)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);

                                //LEFT LEG
                                if (lJoint.Type == SkeletonJointType.BASE_SPINE && lSecondJoint.Type == SkeletonJointType.LEFT_HIP)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.LEFT_HIP && lSecondJoint.Type == SkeletonJointType.LEFT_KNEE)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                                if (lJoint.Type == SkeletonJointType.LEFT_KNEE && lSecondJoint.Type == SkeletonJointType.LEFT_FOOT)
                                    DrawLine(iMat, lCenter - lLocal, lCenter - lSecondLocal, new Scalar(100, 0, 0), lJoint);
                            }
                            #endregion
                        }
                        // Draw a circle with the joint point as center
                        // 10 is a constant, choose after some test, to get a base for the size.
                        // Divide the z coordinate by 1000 to get value in millimeter
                        // The result of the pow operation is used to divide a constant, so adding 0.1 avoid a zero division.
                        // The pow operation purpose, is to increase the influence of the depth
                        Imgproc.circle(iMat, lCenter - lLocal, (int)(10 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)), new Scalar(100, 0, 0), (int)(8 / Math.Pow(lJoint.WorldPosition.z / 1000F + 0.1, 2)));
                    }
                }
            }
        }

        // This function just draw a line between two Point.
        // The thickness of the line, is compute with the depth position of the skeleton joint.
        private void DrawLine(Mat iMat, Point iFirstPoint, Point iSecondPoint, Scalar iColor, SkeletonJoint iJoint)
        {
            // 8 is a constant, choose after some test, to get a base for the size.
            // Divide the z coordinate by 1000 to get value in millimeter
            // The result of the pow operation is used to divide a constant, so adding 0.1 avoid a zero division.
            // The pow operation purpose, is to increase the influence of the depth
            Imgproc.line(iMat, iFirstPoint, iSecondPoint, iColor, (int)(8 / Math.Pow(iJoint.WorldPosition.z / 1000F + 0.1, 2)).Clamp(0.5, 32));
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
            mHeaderRefreshed = false;
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
            mHeaderRefreshed = false;
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
            mHeaderRefreshed = false;
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
