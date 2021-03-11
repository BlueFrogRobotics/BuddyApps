using BlueQuark;

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BuddyApp.Korian
{
    public sealed class GoToUserState : AStateMachineBehaviour
    {
        private const float DIRECTION_OFFSET_LIMIT = 0.2F;
        private const float DIRECTION_THRESHOLD = 0.2F;
        private const float ANGULAR_SPEED = 80F;
        private const float LINEAR_SPEED = 0.3F;
        private const int MEASURE_NUMBER = 2;

        private const int TIME_USER_LOST = 3;
        private const int TIME_GIVEUP = 3;
        private const int ROT_SEARCH_USER = 40;

        private double mLastDirection;
        private bool mIsSearching;
        private float mObstacleCount;
        private int mMeasure;

        private float mTimeHumanDetected;
        private float mTimeHumanLost;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
                new HumanDetectorParameter { HumanDetectionMode = HumanDetectionMode.VISION });

            mMeasure = 0;
            mTimeHumanDetected = Time.time;
            mTimeHumanLost = -1;
            mIsSearching = false;
            mObstacleCount = 0;
            mLastDirection = 10F;

            Buddy.Actuators.Head.Yes.SetPosition(2);

            MoveToward(0F);
        }

        private void MoveToward(float iDirection)
        {
            float lAngularVelocity;

            // If the direction changes enough, update the robot direction
            if (Math.Abs(mLastDirection - iDirection) > DIRECTION_OFFSET_LIMIT)
            {
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
            if (mTimeHumanLost > 0 && (Time.time - mTimeHumanLost) > TIME_GIVEUP)
                Trigger("Scan");
            else if (Time.time - mTimeHumanDetected > TIME_USER_LOST && Buddy.Behaviour.Mood != Mood.THINKING && !Buddy.Behaviour.IsBusy)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.Say(Buddy.Resources.GetString("userlost"), false);
                Buddy.Behaviour.SetMood(Mood.THINKING, false);
                Buddy.Actuators.Wheels.Stop();
                mLastDirection = 10F;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-ROT_SEARCH_USER, 60, () =>
                // OnEndMove 
                {
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(ROT_SEARCH_USER * 2, 60, () => { mTimeHumanLost = Time.time; });
                });
            }

            // Add the average of sensor that found an obstacle
            mObstacleCount += UserInFrontScore();

            // When mMeasurNumber is reach, a new average is compute to reduce false positive
            if (mMeasure == MEASURE_NUMBER)
            {
                Debug.LogWarning("Obstacle:" + mObstacleCount / (float)MEASURE_NUMBER);
                // Finally if an obstacle is detected by several sensor, during several frame, we stop.
                if ((mObstacleCount / (float)MEASURE_NUMBER) > 0.4F && mLastDirection < 9F)
                    Trigger("Evaluation");
                mObstacleCount = 0;
                mMeasure = 0;
            }
        }

        // Average of Sensor - (Value ​​were chosen empirically)
        private float UserInFrontScore()
        {
            int lScore = 0;
            mMeasure++;
            if (Buddy.Sensors.UltrasonicSensors.Left.Value < 700)
                lScore++;
            if (Buddy.Sensors.UltrasonicSensors.Right.Value < 700)
                lScore++;

            if (Buddy.Sensors.TimeOfFlightSensors.Center.Value < 700)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Right.Value < 600)
                lScore++;
            if (Buddy.Sensors.TimeOfFlightSensors.Left.Value < 600)
                lScore++;

            if (Buddy.Sensors.TimeOfFlightSensors.Head.Value < 850)
                lScore++;
            //if (Buddy.Sensors.TimeOfFlightSensors.Chin.Value < 700)
            //    lScore++;
            return ((float)lScore / 7F);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanDetect);
            Buddy.Actuators.Wheels.Stop();
            if (Buddy.Navigation.IsBusy)
                Buddy.Navigation.Stop();
        }

        /*
        *   On a human detection this function is called.
        *   mHumanDetectEnable: Enable or disable the code when WINDOWS is true.
        *   Because the removeP function is in WIP on windows, we juste disable the code, for now.
        */
        private void OnHumanDetect(HumanEntity[] iHumans)
        {
            if (Buddy.Behaviour.Mood != Mood.HAPPY && !Buddy.Behaviour.IsBusy)
            {
                Buddy.Behaviour.SetMood(Mood.HAPPY, false);
                Buddy.Vocal.StopAndClear();
                mTimeHumanLost = -1;
                if (Buddy.Navigation.IsBusy)
                    Buddy.Navigation.Stop();
                Buddy.Vocal.Say("je te vois", false);
            }

            mTimeHumanDetected = Time.time;

            // The target is the biggest human detect. (The closest one)
            double lMaxArea = iHumans.Max(h => h.BoundingBox.area());
            // Get biggest box
            HumanEntity lBiggestHuman = iHumans.First(h => h.BoundingBox.area() == lMaxArea);

            double lHorizontalRatio;
            // Value between 0 & 1, that represent target's position, (abscissa) were 0.5 is the center on the image.
            lHorizontalRatio = lBiggestHuman.Center.x / Buddy.Sensors.RGBCamera.Width;
            // Now the value is between -0.5 & 0.5
            MoveToward((float)lHorizontalRatio - 0.5F);
        }
    }
}

