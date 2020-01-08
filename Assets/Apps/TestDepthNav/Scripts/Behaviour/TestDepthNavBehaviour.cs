using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVUnity;

namespace BuddyApp.TestDepthNav
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestDepthNavBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestDepthNavData mAppData;
        private int mDownSample;
        private bool mCanMove;
        private int mSign;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            TestDepthNavActivity.Init(null);

            Buddy.Actuators.Head.SetPosition(0F, -10F);

            Buddy.Sensors.DepthCamera.Open(DepthCameraMode.DEPTH_640X480_30FPS_1MM);
            Buddy.Sensors.DepthCamera.OnNewFrame.Add(OnNewFrame);
            Buddy.GUI.Toaster.Display<VideoStreamToast>().With(Buddy.Sensors.DepthCamera);
        }


        private void OnNewFrame(DepthCameraFrame iInput)
        {
            Debug.LogWarning("OnNewFrame " + mDownSample);
            if (mDownSample < 5)
                mDownSample++;
            else if (iInput != null && mCanMove) {
                Debug.LogWarning("OnNewFrame " + iInput.DepthFrameData.Length);
                mDownSample = 0;

                int lLeft = 0;
                int lMiddle = 0;
                int lRight = 0;
                // Assess position of obstacles
                for (int i = 0; i < iInput.DepthFrameData.Length; ++i) {
                    if (i < 10)
                        Debug.LogWarning("Depth cam 10 first values are: " + iInput.DepthFrameData[i]);
                    if (i % Buddy.Sensors.DepthCamera.Width < Buddy.Sensors.DepthCamera.Width / 3) {
                        lLeft += iInput.DepthFrameData[i];
                    } else if (i % Buddy.Sensors.DepthCamera.Width > Buddy.Sensors.DepthCamera.Width * 2 / 3) {
                        lRight += iInput.DepthFrameData[i];
                    } else {
                        lMiddle += iInput.DepthFrameData[i];
                    }
                }

                Debug.LogWarning("Final values left, middle, right " + lLeft + " " + lMiddle + " " + lRight);
                Debug.LogWarning("set velocity with " + 0.5F + " " + (1 - ((float)lRight / lLeft)) / 2);

                mSign = Math.Sign(1 - ((float)lRight / lLeft));

                float lCoeff = 1.0F;
                if ( (Buddy.Sensors.UltrasonicSensors.Left.FilteredValue < 1000F && Buddy.Sensors.UltrasonicSensors.Left.Error == 0) ||
                        (Buddy.Sensors.UltrasonicSensors.Right.FilteredValue < 1000F && Buddy.Sensors.UltrasonicSensors.Right.Error == 0) )
                    lCoeff = Math.Min(Buddy.Sensors.UltrasonicSensors.Left.FilteredValue, Buddy.Sensors.UltrasonicSensors.Right.FilteredValue) / 1000F;
             

                Buddy.Actuators.Wheels.SetVelocities(lCoeff, 45F * (1 - ((float)lRight / lLeft)) / 2);

            } else {
                Debug.LogWarning("mCanMove? " + mCanMove);
            }
        }

        void Update()
        {
            if (mCanMove && ObstacleDetected()) {
                Debug.LogWarning("there is an US obstacle " + Buddy.Sensors.UltrasonicSensors.Left.FilteredValue
                    + " " + Buddy.Sensors.UltrasonicSensors.Right.FilteredValue);

                Debug.LogWarning("set velocity with " + 0F + " " + mSign * 30F);
                Buddy.Actuators.Wheels.SetVelocities(0F, mSign * 30F);

                mCanMove = false;
            } else if (!mCanMove && NoObstacleDetected()) {
                Debug.LogWarning("there is no more US obstacle ");
                Buddy.Actuators.Wheels.Stop();
                mCanMove = true;
            }
        }

        /// <summary>
        /// Measure distance with sensors, and compute a score according to values.
        /// All value has been found empirically, to have stop distance of 0.5 meter to the target.
        /// </summary>
        /// <returns>The compute score, according to sensors measurement.</returns>
        private bool ObstacleDetected()
        {
            float lMeasure = 0F;

            // Don't consider very small values, (Sometimes sensors go to zero instead of 8 meters)

            lMeasure = Buddy.Sensors.UltrasonicSensors.Left.FilteredValue;

            if (lMeasure < 330F && Buddy.Sensors.UltrasonicSensors.Left.Error == 0)
                return true;
            lMeasure = Buddy.Sensors.UltrasonicSensors.Right.FilteredValue;

            if (lMeasure < 330F && Buddy.Sensors.UltrasonicSensors.Right.Error == 0)
                return true;


            return false;
        }

        private bool NoObstacleDetected()
        {
            float lMeasure = 0F;

            lMeasure = Buddy.Sensors.UltrasonicSensors.Left.FilteredValue;
            if (lMeasure < 360F /*|| lMeasure < lMinimumThreshold*/) {
                return false;
            }

            lMeasure = Buddy.Sensors.UltrasonicSensors.Right.FilteredValue;
            if (lMeasure < 360F /*|| lMeasure < lMinimumThreshold*/) {
                return false;
            }




            return true;
        }

    }
}
