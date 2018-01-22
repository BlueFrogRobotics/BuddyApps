using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class CollisionDetector : MonoBehaviour
	{
		//List of booleans for detecting collision
		private bool mObstacle;
		private bool mStoppingPhase;
		private bool mBehaviourInit;
		private float mStopDistance;
		private float mNoiseTime;
		private DateTime mDetectionTime;
		public bool enableToMove;

		//private float mdist;
		//private Vector3 mRobotPose;

		public void InitBehaviour ()
		{
			mObstacle = false;
			mStoppingPhase = false;
			mBehaviourInit = true;
			enableToMove = true;
			mStopDistance = ExperienceCenterData.Instance.StopDistance;
			mNoiseTime = ExperienceCenterData.Instance.NoiseTime;

			//mdist = 0.0f;
			//mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
		}

		private void CheckObstacle ()
		{
			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;
			if (mStopDistance != ExperienceCenterData.Instance.StopDistance) {
				mStopDistance = ExperienceCenterData.Instance.StopDistance; 
				Debug.LogWarningFormat ("Stop Distance = {0}m ", mStopDistance);
			}
			if (mNoiseTime != ExperienceCenterData.Instance.NoiseTime) {
				mNoiseTime = ExperienceCenterData.Instance.NoiseTime; 
				Debug.LogWarningFormat ("Noise Time = {0}s ", mNoiseTime);
			}

			if (leftObs <= 0.2 || rightObs <= 0.2 || middleObs <= 0.2) {
				enableToMove = false;
				mObstacle = true;
				Debug.LogError ("There is a collision: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
				return;
			}

			if (leftObs <= mStopDistance || rightObs <= mStopDistance || middleObs <= mStopDistance) {
				if (!mObstacle) {
					Debug.LogWarning ("Stopping Buddy");
					mStoppingPhase = true;
					mObstacle = true;
					enableToMove = false;
				} else {
					if (BYOS.Instance.Primitive.Motors.Wheels.Speed <= 0.01f) {
						Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
						mStoppingPhase = false;
						enableToMove = false;
					} else {
						mStoppingPhase = true;
						enableToMove = false;
						Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					}
				}
			} else {
				if (!mStoppingPhase) {
					Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					mObstacle = false;
					enableToMove = true;
				} else {
					Debug.LogWarning ("Stopping slipping phase");
					mStoppingPhase = false;
				}
			}
		}
			

		private void CheckObstacleTimeFiltred ()
		{
			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;

			if (mStopDistance != ExperienceCenterData.Instance.StopDistance) {
				mStopDistance = ExperienceCenterData.Instance.StopDistance; 
				Debug.LogWarningFormat ("Stop Distance = {0}m ", mStopDistance);
			}

			if (middleObs <= 0.3) {
				enableToMove = false;
				Debug.LogError ("There is a collision: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
				return;
			}

			if (middleObs <= mStopDistance) {
				if (!mObstacle) {
					Debug.LogWarning ("Something detected: Obstacle or Noise ?");
					mObstacle = true;
					mDetectionTime = DateTime.Now;
				} else {
					TimeSpan lElapsedTime = DateTime.Now - mDetectionTime;
					if (lElapsedTime.TotalSeconds > mNoiseTime) {
						Debug.LogWarningFormat ("Obstacle is detected at {0}", DateTime.Now.Date.ToString ());
						if (BYOS.Instance.Primitive.Motors.Wheels.Speed <= 0.01f) {
							Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
							mStoppingPhase = false;
							enableToMove = false;
						} else {
							mStoppingPhase = true;
							enableToMove = false;
							Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
						}
					} else {
						Debug.LogWarningFormat ("Check Obstacle: {0}s", lElapsedTime.TotalSeconds);
						//mObstacle = false;
					}
				}
			} else {
				if (!mStoppingPhase) {
					Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					mObstacle = false;
					enableToMove = false;
				} else {
					Debug.LogWarning ("Stopping slipping phase");
					mStoppingPhase = false;
				}
			}
		}


		static public float Distance (Vector3 v1, Vector3 v2)
		{
			float d = (float)(Math.Sqrt ((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y)));
			return d;
		}

		void Update ()
		{
			if (mBehaviourInit)
				CheckObstacleTimeFiltred ();
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Collision Detection");
			mBehaviourInit = false;
		}

	}
}