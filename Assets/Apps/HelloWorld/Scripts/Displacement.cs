using BlueQuark;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.HelloWorld
{
    public sealed class Displacement : AStateMachineBehaviour
    {
        private const int MEASURE_NUMBER = 2;

        private float mObstacleCount;
        private int mMeasure;

        private bool mMovingAsyncIsActive;
        private Vector2 mLastPosition;
        private float mDistTravelled;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            // Init
            mObstacleCount = 0;
            mMeasure = 0;
            mMovingAsyncIsActive = false;

            // STEP 1 - forward
            Buddy.Navigation.Run<DisplacementStrategy>().Move(1.5F, HelloWorldUtils.LINEAR_VELOCITY, () =>
            {
                StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
              {
                // STEP 2 - rotate 60 left
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(60F, HelloWorldUtils.ANGULAR_VELOCITY, () =>
                  {
                      StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                      {
                        // STEP 3 - backward
                        Buddy.Navigation.Run<DisplacementStrategy>().Move(-1.5F, HelloWorldUtils.LINEAR_VELOCITY, () =>
                          {
                              StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                              {
                                // STEP 4 - rotate 360
                                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(300F, HelloWorldUtils.ANGULAR_VELOCITY, () =>
                                  {
                                      StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                                      {
                                        // STEP 5 - rotate 60 right
                                        Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-60F, HelloWorldUtils.ANGULAR_VELOCITY, () =>
                                          {
                                              StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                                              {
                                                  Trigger("TouchAndCaress");
                                              }));
                                          });
                                      }));
                                  });
                              }));
                          });
                      }));
                  });

              }));
            });
        }

        //public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        //{
        //    if (!Buddy.Navigation.IsBusy && mMovingAsyncIsActive) {
        //        // Double check obstacle and void

        //        // Add the average of sensor that found an obstacle
        //        mObstacleCount += UserInFrontScore();
        //        // When mMeasurNumber is reach, a new average is compute to reduce false positive
        //        if (mMeasure == MEASURE_NUMBER) {
        //            Debug.LogWarning("Obstacle:" + mObstacleCount / (float)MEASURE_NUMBER);
        //            // Finally if an obstacle is detected by several sensor, during several frame, we stop.
        //            if ((mObstacleCount / (float)MEASURE_NUMBER) > 0.4F) {
        //                Buddy.Navigation.Stop();
        //            }
        //            mObstacleCount = 0;
        //            mMeasure = 0;
        //        }
        //    }
        //}

        //// Average of Sensor - (Value ​​were chosen empirically)
        //private float UserInFrontScore()
        //{
        //    int lScore = 0;
        //    mMeasure++;
        //    if (Buddy.Sensors.UltrasonicSensors.Left.Value < 700)
        //        lScore++;
        //    if (Buddy.Sensors.UltrasonicSensors.Right.Value < 700)
        //        lScore++;

        //    if (Buddy.Sensors.TimeOfFlightSensors.Front.Value < 700)
        //        lScore++;
        //    if (Buddy.Sensors.TimeOfFlightSensors.Right.Value < 600)
        //        lScore++;
        //    if (Buddy.Sensors.TimeOfFlightSensors.Left.Value < 600)
        //        lScore++;

        //    if (Buddy.Sensors.TimeOfFlightSensors.Forehead.Value < 850)
        //        lScore++;
        //    if (Buddy.Sensors.TimeOfFlightSensors.Chin.Value < 700)
        //        lScore++;
        //    return ((float)lScore / 7F);
        //}

        //private IEnumerator MoveAsync(float iDistance, float iVelocity, Action iOnEndMove = null)
        //{
        //    if (!Buddy.Actuators.Wheels.IsBusy && !mMovingAsyncIsActive) {
        //        mMovingAsyncIsActive = true;
        //        Buddy.Actuators.Wheels.SetVelocities(iVelocity, 0F);
        //    }

        //    // Update odometry info
        //    mDistTravelled += (Buddy.Actuators.Wheels.Odometry.Position() - mLastPosition).Norm();
        //    mLastPosition = Buddy.Actuators.Wheels.Odometry.Position();

        //    yield return new WaitUntil(() =>
        //    {
        //        if (mDistTravelled > iDistance)
        //            return true;
        //        return false;
        //    });

        //    Buddy.Actuators.Wheels.Stop();
        //    if (iOnEndMove != null)
        //        iOnEndMove();
        //    mMovingAsyncIsActive = false;
        //}
    }
}
