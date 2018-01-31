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

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();

			behaviourEnd = false;
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				mCollisionDetector.InitBehaviour ();

			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			mDistance = ExperienceCenterData.Instance.TableDistance; 
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				StartCoroutine (MoveForward (wheelSpeed));
			else
				behaviourEnd = true;
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			mDistance = mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			yield return new WaitUntil (() => mCollisionDetector.enableToMove);

			Debug.LogFormat ("Wheels lock: {0}", BYOS.Instance.Primitive.Motors.Wheels.Locked);
			BYOS.Instance.Primitive.Motors.Wheels.SetWheelsSpeed (lSpeed);

			Debug.LogFormat ("Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, mDistance);

			yield return new WaitUntil (() => mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)
			|| !mCollisionDetector.enableToMove);

			Debug.LogFormat ("Check condition : Distance = {0}, Obstacle ={1}", mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD), !mCollisionDetector.enableToMove);
			Debug.LogFormat ("Distance left to travel : {0}", mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose));
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();

			if (!mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				//Debug.Log ("New MoveForward Coroutine");
				//mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
				//mDistance = ExperienceCenterData.Instance.TableDistance;  
				//StartCoroutine (MoveForward (wheelSpeed));
				Debug.Log ("End MoveForward Coroutine");
				behaviourEnd = true;
				mCollisionDetector.StopBehaviour ();
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