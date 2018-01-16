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
		public bool behaviourEnd;

		private AnimatorManager mAnimatorManager;
		private CollisionDetector mCollisionDetector;
		private float radius;
		private Vector3 mRobotPose;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();
			behaviourEnd = false;
			if (ExperienceCenterData.Instance.EnableMovement)
				mCollisionDetector.InitBehaviour ();

			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;

			if (ExperienceCenterData.Instance.EnableMovement)
				StartCoroutine (MoveForward (wheelSpeed));
			else
				behaviourEnd = true;
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			distance = distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			yield return new WaitUntil (() => mCollisionDetector.enableToMove);

			BYOS.Instance.Primitive.Motors.Wheels.SetWheelsSpeed (lSpeed);

			Debug.LogFormat ("Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, distance);

			yield return new WaitUntil (() => CheckSpeed () || CheckDistance () || !mCollisionDetector.enableToMove);

			Debug.LogFormat ("Distance left to travel : {0}", distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose));
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();

			if (!CheckDistance ()) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				behaviourEnd = true;
			}
		}

		public void StopBehaviour ()
		{
			mCollisionDetector.StopBehaviour ();
			Debug.LogWarning ("Stop Move Behaviour");
			StopAllCoroutines ();
			if (ExperienceCenterData.Instance.EnableMovement)
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
			behaviourEnd = true;
		}

		private bool CheckSpeed ()
		{
			return Math.Abs (BYOS.Instance.Primitive.Motors.Wheels.Speed) <= 0.1f;
		}

		private bool CheckDistance ()
		{
			return distance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose) <= DISTANCE_THRESHOLD;
		}

	}
}