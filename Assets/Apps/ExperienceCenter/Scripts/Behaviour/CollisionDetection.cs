using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
	public class CollisionDetector : MonoBehaviour
	{
		//List of booleans for detecting collision
		private bool mObstacle;
		private bool mStoppingPhase;

		private float mStopDistance;
		private float mNoiseTime;
		private float mMaxDistance;
		private float mMinDistance;
		private float mMaxSpeed;
		private float mMinSpeed;
		private float mA1;
		private float mB1;
		private float mA2;
		private float mB2;

		private DateTime mDetectionTime;

		public bool behaviourInit;
		public bool enableToMove;
		public bool updateSpeed;

		public float leftSpeed;
		public float rightSpeed;
		public float middleSpeed;

		public void InitBehaviour ()
		{
			mObstacle = false;
			mStoppingPhase = false;
			behaviourInit = true;
			enableToMove = true;
			updateSpeed = false;
			leftSpeed = 0F;
			rightSpeed = 0F;
			middleSpeed = 0F;
			mStopDistance = ExperienceCenterData.Instance.StopDistance;
			mNoiseTime = ExperienceCenterData.Instance.NoiseTime;

		}

		//		private void CheckObstacle ()
		//		{
		//			float leftObs = Buddy.Sensors.UltrasonicSensors.Left.Value/1000;
		//			float rightObs = Buddy.Sensors.UltrasonicSensors.Right.Value/1000;
		//			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;
		//			if (mStopDistance != ExperienceCenterData.Instance.StopDistance) {
		//				mStopDistance = ExperienceCenterData.Instance.StopDistance;
		//				Debug.LogWarningFormat ("Stop Distance = {0}m ", mStopDistance);
		//			}
		//			if (mNoiseTime != ExperienceCenterData.Instance.NoiseTime) {
		//				mNoiseTime = ExperienceCenterData.Instance.NoiseTime;
		//				Debug.LogWarningFormat ("Noise Time = {0}s ", mNoiseTime);
		//			}

		//			if (leftObs <= 0.2 || rightObs <= 0.2 || middleObs <= 0.2) {
		//				enableToMove = false;
		//				mObstacle = true;
		//				Debug.LogError ("There is a collision: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
		//				return;
		//			}


		//			if (leftObs <= mStopDistance || rightObs <= mStopDistance || middleObs <= mStopDistance) {
		//			if (leftObs > 0.01 || rightObs > 0.01 || middleObs > 0.01) {
		//				if (!mObstacle) {
		//					Debug.LogWarning ("Stopping Buddy");
		//					mStoppingPhase = true;
		//					mObstacle = true;
		//					enableToMove = false;
		//				} else {
		//					if (Buddy.Actuators.Wheels.Speed <= 0.01f) {
		//						Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
		//						mStoppingPhase = false;
		//						enableToMove = false;
		//					} else {
		//						mStoppingPhase = true;
		//						enableToMove = false;
		//						Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
		//					}
		//				}
		//			} else {
		//				if (!mStoppingPhase) {
		//					Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
		//					mObstacle = false;
		//					enableToMove = true;
		//				} else {
		//					Debug.LogWarning ("Stopping slipping phase");
		//					mStoppingPhase = false;
		//				}
		//			}
		//		}


		private void CheckObstacleTimeFiltred (bool debug)
		{
			float leftObs = Buddy.Sensors.UltrasonicSensors.Left.Value/1000;
			float rightObs = Buddy.Sensors.UltrasonicSensors.Right.Value/1000;
            float middleObs = 10;// BYOS.Instance.Primitive.IRSensors.Middle.Distance;

			if (mStopDistance != ExperienceCenterData.Instance.StopDistance) {
				mStopDistance = ExperienceCenterData.Instance.StopDistance; 
				Debug.LogWarningFormat ("[EXCENTER] Stop Distance = {0}m ", mStopDistance);
			}
			if (debug)
				Debug.LogWarning ("[EXCENTER] Sensors : L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs);

			if (leftObs <= 0.3 || rightObs <= 0.3 || middleObs <= 0.3) {
				enableToMove = false;
				if (debug)
					Debug.LogWarning ("[EXCENTER] Critical distance : L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
			}

			if (leftObs <= mStopDistance || rightObs <= mStopDistance || middleObs <= mStopDistance) {
				//if (leftObs > 0.01 || rightObs > 0.01 || middleObs > 0.01) {
				if (!mObstacle) {
					if (debug)
						Debug.LogWarning ("[EXCENTER] Something detected: Obstacle or Noise ?");
					mObstacle = true;
					mDetectionTime = DateTime.Now;
				} else {
					TimeSpan lElapsedTime = DateTime.Now - mDetectionTime;
					if (lElapsedTime.TotalSeconds > mNoiseTime) {
						if (debug)
							Debug.LogWarningFormat ("[EXCENTER] Obstacle is detected at {0}", DateTime.Now.Date.ToString ());
						if (Buddy.Actuators.Wheels.Speed <= 0.01f) {
							if (debug)
								Debug.LogWarning ("[EXCENTER] Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
							mStoppingPhase = false;
							enableToMove = false;
						} else {
							mStoppingPhase = true;
							enableToMove = false;
							if (debug)
								Debug.LogWarning ("[EXCENTER] Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
						}
					} else {
						if (debug)
							Debug.LogWarningFormat ("[EXCENTER] Check Obstacle: {0}s", lElapsedTime.TotalSeconds);
						enableToMove = true;
					}
				}
			} else {
				if (!mStoppingPhase) {
					if (debug)
						Debug.LogWarning ("[EXCENTER] Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + Buddy.Actuators.Wheels.Speed);
					mObstacle = false;
					enableToMove = true;
				} else {
					if (debug)
						Debug.LogWarning ("[EXCENTER] Stopping slipping phase");
					mStoppingPhase = false;
				}
			}
		}


		static public float Distance (Vector3 v1, Vector3 v2)
		{
			float d = (float)(Math.Sqrt ((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y)));
			return d;
		}

		public bool CheckSpeed (float speedThreshold)
		{
			return Math.Abs (Buddy.Actuators.Wheels.Speed) <= speedThreshold;
		}

		public bool CheckDistance (float distance, Vector3 robotPose, float distanceThreshold)
		{
			return Mathf.Abs(distance - CollisionDetector.Distance (Buddy.Actuators.Wheels.Odometry, robotPose)) <= distanceThreshold;
		}

		void Update ()
		{
			if (mMaxDistance != ExperienceCenterData.Instance.MaxDistance) {
				mMaxDistance = ExperienceCenterData.Instance.MaxDistance; 
				UpdateCoefficients ();
				Debug.LogFormat ("[EXCENTER] Distance ({0} deg/s) = {1}", mMaxSpeed, mMaxDistance);
				Debug.LogWarningFormat ("[EXCENTER] Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMinDistance != ExperienceCenterData.Instance.MinDistance) {
				mMinDistance = ExperienceCenterData.Instance.MinDistance; 
				UpdateCoefficients ();
				Debug.LogFormat ("[EXCENTER] Distance ({0} deg/s) = {1}", mMinSpeed, mMaxDistance);
				Debug.LogFormat ("[EXCENTER] Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMaxSpeed != ExperienceCenterData.Instance.MaxSpeed) {
				mMaxSpeed = ExperienceCenterData.Instance.MaxSpeed; 
				UpdateCoefficients ();
				Debug.LogFormat ("[EXCENTER] Max Speed = {0} deg/s", mMaxSpeed);
				Debug.LogFormat ("[EXCENTER] Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMinSpeed != ExperienceCenterData.Instance.MinSpeed) {
				mMinSpeed = ExperienceCenterData.Instance.MinSpeed; 
				UpdateCoefficients ();
				Debug.LogFormat ("[EXCENTER] Min Speed = {0} deg/s", mMinSpeed);
				Debug.LogFormat ("[EXCENTER] Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (behaviourInit) {
				CheckObstacleTimeFiltred (ExperienceCenterData.Instance.CollisionDebug);
				updateSpeed = false;
				GetLeftSpeed ();
				GetRightSpeed ();
				GetMiddleSpeed ();
			}
		}

		private void UpdateCoefficients ()
		{
			mA1 = mMaxSpeed / (mMaxDistance - mMinDistance);
			mB1 = -mA1 * mMinDistance;
			mA2 = -mMinSpeed / mMinDistance;
			mB2 = mMinSpeed;
		}


		public float GetLeftSpeed ()
		{
			float leftObs = Buddy.Sensors.UltrasonicSensors.Left.Value/1000;
			float speed = 0F;

			if (leftObs >= mMaxDistance) {
				speed = 200; 
			} else if (leftObs >= mMinDistance) {
				speed = mA1 * leftObs + mB1;
			} else if (leftObs < mMinDistance) {
				speed = mA2 * leftObs + mB2;
			}

			if (speed != leftSpeed) {
				leftSpeed = speed;
				updateSpeed = true;
			}
			return speed;
		}

		public float GetRightSpeed ()
		{
			float rightObs = Buddy.Sensors.UltrasonicSensors.Right.Value/1000;
			float speed = 0F;

			if (rightObs >= mMaxDistance) {
				speed = 200;
			} else if (rightObs >= mMinDistance) {
				speed = mA1 * rightObs + mB1;
			} else if (rightObs < mMinDistance) {
				speed = mA2 * rightObs + mB2;
			}

			if (speed != rightSpeed) {
				rightSpeed = speed;
				updateSpeed = true;
			}
			return speed;
		}

		public float GetMiddleSpeed ()
		{
            float middleObs = 10;// BYOS.Instance.Primitive.IRSensors.Middle.Distance;
			float speed = 0F;

			if (middleObs >= mMaxDistance) {
				speed = 200;
			} else if (middleObs >= mMinDistance) {
				speed = mA1 * middleObs + mB1;
			} else if (middleObs < mMinDistance) {
				speed = mA2 * middleObs + mB2;
			}

			if (speed != rightSpeed) {
				middleSpeed = speed;
				updateSpeed = true;
			}
			return speed;
		}



		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Collision Detection");
			behaviourInit = false;
		}

	}
}