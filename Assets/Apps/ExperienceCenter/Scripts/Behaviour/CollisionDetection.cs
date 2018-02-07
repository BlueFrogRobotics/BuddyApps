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
		//			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
		//			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
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
		//				Debug.LogError ("There is a collision: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
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
		//					if (BYOS.Instance.Primitive.Motors.Wheels.Speed <= 0.01f) {
		//						Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
		//						mStoppingPhase = false;
		//						enableToMove = false;
		//					} else {
		//						mStoppingPhase = true;
		//						enableToMove = false;
		//						Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
		//					}
		//				}
		//			} else {
		//				if (!mStoppingPhase) {
		//					Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
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
			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;

			if (mStopDistance != ExperienceCenterData.Instance.StopDistance) {
				mStopDistance = ExperienceCenterData.Instance.StopDistance; 
				Debug.LogWarningFormat ("Stop Distance = {0}m ", mStopDistance);
			}
			if (debug)
				Debug.LogWarning ("Sensors : L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs);

			if (leftObs <= 0.3 || rightObs <= 0.3 || middleObs <= 0.3) {
				enableToMove = false;
				if (debug)
					Debug.LogWarning ("Critical distance : L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
			}

			if (leftObs <= mStopDistance || rightObs <= mStopDistance || middleObs <= mStopDistance) {
				//if (leftObs > 0.01 || rightObs > 0.01 || middleObs > 0.01) {
				if (!mObstacle) {
					if (debug)
						Debug.LogWarning ("Something detected: Obstacle or Noise ?");
					mObstacle = true;
					mDetectionTime = DateTime.Now;
				} else {
					TimeSpan lElapsedTime = DateTime.Now - mDetectionTime;
					if (lElapsedTime.TotalSeconds > mNoiseTime) {
						if (debug)
							Debug.LogWarningFormat ("Obstacle is detected at {0}", DateTime.Now.Date.ToString ());
						if (BYOS.Instance.Primitive.Motors.Wheels.Speed <= 0.01f) {
							if (debug)
								Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
							mStoppingPhase = false;
							enableToMove = false;
						} else {
							mStoppingPhase = true;
							enableToMove = false;
							if (debug)
								Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
						}
					} else {
						if (debug)
							Debug.LogWarningFormat ("Check Obstacle: {0}s", lElapsedTime.TotalSeconds);
						enableToMove = true;
					}
				}
			} else {
				if (!mStoppingPhase) {
					if (debug)
						Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					mObstacle = false;
					enableToMove = true;
				} else {
					if (debug)
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

		public bool CheckSpeed (float speedThreshold)
		{
			return Math.Abs (BYOS.Instance.Primitive.Motors.Wheels.Speed) <= speedThreshold;
		}

		public bool CheckDistance (float distance, Vector3 robotPose, float distanceThreshold)
		{
			return distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, robotPose) <= distanceThreshold;
		}

		void Update ()
		{
			if (mMaxDistance != ExperienceCenterData.Instance.MaxDistance) {
				mMaxDistance = ExperienceCenterData.Instance.MaxDistance; 
				UpdateCoefficients ();
				Debug.LogWarningFormat ("Distance ({0} deg/s) = {1}", mMaxSpeed, mMaxDistance);
				Debug.LogWarningFormat ("Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMinDistance != ExperienceCenterData.Instance.MinDistance) {
				mMinDistance = ExperienceCenterData.Instance.MinDistance; 
				UpdateCoefficients ();
				Debug.LogWarningFormat ("Distance ({0} deg/s) = {1}", mMinSpeed, mMaxDistance);
				Debug.LogWarningFormat ("Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMaxSpeed != ExperienceCenterData.Instance.MaxSpeed) {
				mMaxSpeed = ExperienceCenterData.Instance.MaxSpeed; 
				UpdateCoefficients ();
				Debug.LogWarningFormat ("Max Speed = {0} deg/s", mMaxSpeed);
				Debug.LogWarningFormat ("Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
			}

			if (mMinSpeed != ExperienceCenterData.Instance.MinSpeed) {
				mMinSpeed = ExperienceCenterData.Instance.MinSpeed; 
				UpdateCoefficients ();
				Debug.LogWarningFormat ("Min Speed = {0} deg/s", mMinSpeed);
				Debug.LogWarningFormat ("Coefficients A1={0}(deg/s/m), B1={1}(deg/s), A2={2}(deg/s/m), B2={3}(deg/s)", mA1, mB1, mA2, mB2);
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
			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
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
			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
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
			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;
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
			Debug.Log ("Stop Collision Detection");
			behaviourInit = false;
		}

	}
}