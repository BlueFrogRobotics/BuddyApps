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
		public float distance = 1.5f;
		public float wheelSpeed = 200f;

		private AnimatorManager mAnimatorManager;
		private CollisionDetector mCollisionDetector;
		private float radius;
		private Vector3 mRobotPose;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();
			mCollisionDetector.InitBehaviour ();
			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			StartCoroutine (MoveForward (wheelSpeed));
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			distance = distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			yield return new WaitUntil (() => mCollisionDetector.enableToMove);
			while (true) {
				BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (lSpeed, lSpeed, distance, 0.01f);
				yield return new WaitForSeconds (0.5f);
				Debug.LogFormat ("Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, distance);
				if (Math.Abs (BYOS.Instance.Primitive.Motors.Wheels.Speed) > 0.01f)
					break;
			}
			yield return new WaitUntil (() => distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose) <= DISTANCE_THRESHOLD || !mCollisionDetector.enableToMove);

			if (!mCollisionDetector.enableToMove) {
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
			}
				
			if ((distance - CollisionDetector.Distance(BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose)) > DISTANCE_THRESHOLD) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
//			} else {
//				Debug.Log ("Run new MoveForward Coroutine");
//				distance = 1.5f;
//				StartCoroutine (MoveForward (wheelSpeed));
			}
		}

		public void StopBehaviour ()
		{
			mCollisionDetector.StopBehaviour ();
			Debug.LogWarning ("Stop Move Behaviour");
			StopAllCoroutines ();
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();
		}

	}
}