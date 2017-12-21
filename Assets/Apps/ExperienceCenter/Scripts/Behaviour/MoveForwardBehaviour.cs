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

		private bool mBuddyMoving = true;
		public float distance = 1.5f;
		public float vitesse = 0.5f;

		public void InitBehaviour ()
		{
			Debug.Log ("Start Move Forward");
			StartCoroutine(MoveForward(distance,vitesse));
		}



		private IEnumerator MoveForward (float lDistance, float lSpeed)
		{
			float lRadius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			Vector3 pose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			Debug.Log ("Init Pose: " + pose);
			float lAngularSpeed = (float) (180 / Math.PI / lRadius * lSpeed);
			Debug.Log ("lAngularSpeed: " + lAngularSpeed);
			BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (lAngularSpeed, lAngularSpeed, lDistance, 0.01f);
			yield return new WaitWhile(() => BYOS.Instance.Primitive.Motors.Wheels.Status.ToString () == "MOVING" );
			pose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			Debug.Log ("Final Pose: " + pose);
		}

	}
}