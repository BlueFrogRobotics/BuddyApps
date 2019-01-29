using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using BlueQuark;
using OpenCVUnity;
using System;

namespace BuddyApp.Korian
{
    public sealed class GoToUserState : AStateMachineBehaviour
    {
        private const float DIRECTION_OFFSET_LIMIT = 0.2F;
        private const float DIRECTION_THRESHOLD = 0.2F;
        private const float ANGULAR_SPEED = 80F;
        private const float LINEAR_SPEED = 0.3F;

        private double mLastDirection;

        //private double mAngle;

        //private List<OpenCVUnity.Rect> mDetectedBox;

        private Point mDetectedCenter;

        // This texture will be filled with the camera data
        private Texture2D mCamView;
        private float mTimeHumanDetected;
        private int mObstacleCount;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
                new HumanDetectorParameter {
                    SensorMode = SensorMode.VISION,
                    //YOLO = new YOLOParameter {
                    //    UseThermal = false,
                    //    DetectFallenHuman = false,
                    //    //DownSample = 10
                    //}
                }
                );

            mTimeHumanDetected = Time.time;

            mObstacleCount = 0;
            //no start
            //mAngle = 0;
            mLastDirection = 10F;
            // --- TMP Cam view to debug ---
            //mDetectedBox = new List<OpenCVUnity.Rect> { };
            mDetectedCenter = new Point();
            //// Initialize texture.
            //mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            //// Setting of the callback to use camera data
            //Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
            //Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCamView);

            Buddy.Actuators.Head.Yes.SetPosition(2);

            MoveToward(0F);
        }

        private void MoveToward(float iDirection)
        {
            float lAngularVelocity;

            // If the direction changes enough, update the robot direction
            if (Math.Abs(mLastDirection - iDirection) > DIRECTION_OFFSET_LIMIT) {
                mLastDirection = iDirection;
                lAngularVelocity = -iDirection * ANGULAR_SPEED;
                // If the direction is included in the middle range, stop to rotate.
                if (Math.Abs(iDirection) < DIRECTION_THRESHOLD)
                    lAngularVelocity = 0F;
                Buddy.Actuators.Wheels.SetVelocities(LINEAR_SPEED, lAngularVelocity);
            }
        }


        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Time.time - mTimeHumanDetected > 2.5F && Buddy.Behaviour.Mood != Mood.THINKING && !Buddy.Behaviour.IsBusy) {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say("où es-tu?", false);
                Buddy.Behaviour.SetMood(Mood.THINKING, false);
                //Buddy.Actuators.Speakers.Effects.Play(SoundSample.BEEP_2);
                Buddy.Navigation.Stop();
                mLastDirection = 10F;
            }

            mObstacleCount += UserInFrontScore();
            if (mObstacleCount > 4 && mLastDirection < 9F) {
                Buddy.Navigation.Stop();
                mLastDirection = 10F;
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say("Salut toi!", false);
                mObstacleCount = 0;
            }
        }

        private int UserInFrontScore()
        {
           int lScore = 0;
            if (Buddy.Sensors.UltrasonicSensors.Left.Value < 500)
                lScore++;
            if (Buddy.Sensors.UltrasonicSensors.Left.Value < 500)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Front.Value < 500)
                lScore++;
            if(Buddy.Sensors.TimeOfFlightSensors.Right.Value < 500)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Left.Value < 500)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Forehead.Value < 500)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Chin.Value < 500)
                lScore++;

            if (lScore == 0)
                lScore = -1;
            return lScore;
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanDetect);
            //mDetectedBox.Clear();
            if (Buddy.Navigation.IsBusy)
                Buddy.Navigation.Stop();
        }


        // On each frame captured by the camera this function is called, with the matrix of pixel.
        //private void OnFrameCaptured(RGBCameraFrame iInput)
        //{
        //    // Always clone the input matrix, this avoid to working with the original matrix, when the C++ part wants to modify it.
        //    Mat lMatSrc = iInput.Mat.clone();


        //    Imgproc.circle(lMatSrc, mDetectedCenter, 5, new Scalar(new Color(255, 0, 0)), 10);

        //    // Flip to avoid mirror effect.
        //    Core.flip(lMatSrc, lMatSrc, 1);
        //    // Use matrice format, to scale the texture.
        //    mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
        //    // Use matrice to fill the texture.
        //    Utils.MatToTexture2D(lMatSrc, mCamView);
        //}

        /*
        *   On a human detection this function is called.
        *   mHumanDetectEnable: Enable or disable the code when WINDOWS is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private void OnHumanDetect(HumanEntity[] iHumans)
        {
            if (Buddy.Behaviour.Mood != Mood.HAPPY && !Buddy.Behaviour.IsBusy) {
                Buddy.Behaviour.SetMood(Mood.HAPPY, false);
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say("je te vois", false);
            }

            Debug.Log("On Human detect pre biggest human");
            mTimeHumanDetected = Time.time;

            double lMaxArea = iHumans.Max(h => h.BoundingBox.area());
            HumanEntity lBiggestHuman = iHumans.First(h => h.BoundingBox.area() == lMaxArea); // get biggest box

            double lHorizontalRatio;

            Debug.Log("On Human detect post biggest human");

            mDetectedCenter.x = lBiggestHuman.Center.x;
            mDetectedCenter.y = lBiggestHuman.Center.y;

            Debug.Log("On Human detect pre camera");
            lHorizontalRatio = lBiggestHuman.Center.x / Buddy.Sensors.RGBCamera.Width; // 0 to 1 value
            MoveToward((float)lHorizontalRatio - 0.5F);
            Debug.Log("On Human detect post camera");
        }


    }
}

