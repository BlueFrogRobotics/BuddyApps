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
		public const float  DISTANCE_THRESHOLD = 0.05f;
		public float distance = 1.5f;
		public float vitesse = 0.5f;

		private AnimatorManager mAnimatorManager;
		private float radius;
		private Vector3 robotPose;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			StartCoroutine(MoveForward(distance,vitesse));
		}
			
		private IEnumerator MoveForward (float lDistance, float lSpeed)
		{
			Debug.Log("Distance = " +  Math.Abs(BYOS.Instance.Primitive.Motors.Wheels.Odometry.x - robotPose.x));
			robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
	        float destinationPoseX = BYOS.Instance.Primitive.Motors.Wheels.Odometry.x + lDistance;
			float lAngularSpeed = (float) (180 / Math.PI / radius * lSpeed);
			BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (lAngularSpeed, lAngularSpeed, lDistance, 0.01f);
			yield return new WaitUntil(() => Math.Abs(BYOS.Instance.Primitive.Motors.Wheels.Odometry.x - destinationPoseX) <= DISTANCE_THRESHOLD );
		}

		public void StopBehaviour ()
		{
			StopAllCoroutines ();
		}

	}
}