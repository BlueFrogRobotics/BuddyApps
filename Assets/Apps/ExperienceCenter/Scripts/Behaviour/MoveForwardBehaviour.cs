using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{
	public class MoveForwardBehaviour : MonoBehaviour
	{
		public const float DISTANCE_THRESHOLD = 0.05f;
		public float wheelSpeed = 200f;
		public bool behaviourEnd;

		private AnimatorManager mAnimatorManager;
		private CollisionDetector mCollisionDetector;
		private float radius;
		private Vector3 mRobotPose;
		private float mDistance;
		private float mMoveTimeOut;
		private bool mTimeOut;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();

			if (mMoveTimeOut != ExperienceCenterData.Instance.MoveTimeOut) {
				mMoveTimeOut = ExperienceCenterData.Instance.MoveTimeOut; 
				Debug.LogFormat ("MoveForward TimeOut = {0} s ", mMoveTimeOut);
			}

			behaviourEnd = false;
			mTimeOut = false;
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				mCollisionDetector.InitBehaviour ();

			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			mDistance = ExperienceCenterData.Instance.TableDistance; 
			if (ExperienceCenterData.Instance.EnableBaseMovement) {
				StartCoroutine (MoveForward (wheelSpeed));
				StartCoroutine (MoveTimeOut ());
			} else
				behaviourEnd = true;
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			
			mDistance = mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			Debug.LogWarningFormat ("Check condition : EnableMove = {0},  RecognitionFinished = {1}, TimeOut = {2}", mCollisionDetector.enableToMove, BYOS.Instance.Interaction.VocalManager.RecognitionFinished, mTimeOut);
			yield return new WaitUntil (() => (mCollisionDetector.enableToMove && BYOS.Instance.Interaction.VocalManager.RecognitionFinished) || mTimeOut);

			if (!mTimeOut) {
				BYOS.Instance.Primitive.Motors.Wheels.SetWheelsSpeed (lSpeed);

				Debug.LogFormat ("Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, mDistance);

				yield return new WaitUntil (() => mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)
				|| !mCollisionDetector.enableToMove
				|| mTimeOut);
			}
			Debug.LogFormat ("Check condition : Distance = {0}, Obstacle ={1}, TimeOut ={2}", mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD), !mCollisionDetector.enableToMove, mTimeOut);
			Debug.LogFormat ("Distance left to travel : {0}", mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose));
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();

			if (!mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD) && !mTimeOut) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				//Debug.Log ("New MoveForward Coroutine");
				//mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
				//mDistance = ExperienceCenterData.Instance.TableDistance;  
				//StartCoroutine (MoveForward (wheelSpeed));
				if (!mTimeOut)
					Debug.Log ("End MoveForward Coroutine");
				else
					Debug.Log ("End MoveForward Coroutine: (Time-out)");
				behaviourEnd = true;
				mCollisionDetector.StopBehaviour ();
			}
		}

		private IEnumerator MoveTimeOut ()
		{
			yield return new WaitUntil (() => !mTimeOut);
			Vector3 robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			DateTime lLastTime = DateTime.Now;
			while (!mTimeOut) {
				if (CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, robotPose) <= DISTANCE_THRESHOLD) {
					Debug.LogWarning ("Robot is stopped");
					TimeSpan lElapsedTime = DateTime.Now - lLastTime;
					if (lElapsedTime.TotalSeconds > mMoveTimeOut) {
						mTimeOut = true;
						break;
					}
				} else {
					Debug.LogWarning ("Robot is moving");
					robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
					lLastTime = DateTime.Now;
				}
				yield return new WaitForSeconds (1.0f);
			}
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Move Behaviour");
			StopAllCoroutines ();
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
			behaviourEnd = true;
		}
			
	}
}