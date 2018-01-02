using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{


	public class IOTBehaviour : MonoBehaviour
	{
		public const float DISTANCE_THRESHOLD = 0.05f;
		public float distance = 1.5f;
		public float vitesse = 0.5f;

		private AnimatorManager mAnimatorManager;
		private HTTPRequestManager mHttpManager;
		private TextToSpeech mTTS;
		private float radius;
		private Vector3 robotPose;
		private bool mRobotMoving = false;


		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mHttpManager = GameObject.Find ("AIBehaviour").GetComponent<HTTPRequestManager> ();
			mHttpManager.Login ();
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			StartCoroutine (MoveForward (distance, vitesse));
			StartCoroutine (Speaking ());
		}

		private IEnumerator MoveForward (float lDistance, float lSpeed)
		{
			mRobotMoving = true;
			robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			float destinationPoseX = BYOS.Instance.Primitive.Motors.Wheels.Odometry.x + lDistance;
			float lAngularSpeed = (float)(180 / Math.PI / radius * lSpeed);
			BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (lAngularSpeed, lAngularSpeed, lDistance, 0.01f);
			yield return new WaitUntil (() => Math.Abs (BYOS.Instance.Primitive.Motors.Wheels.Odometry.x - destinationPoseX) <= DISTANCE_THRESHOLD);
			mRobotMoving = false;
		}

		private IEnumerator Speaking ()
		{
			mTTS.SayKey ("iotboost", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotrouler", true);
			mTTS.Silence (2000, true);
			yield return new WaitWhile (() => mRobotMoving);

			mTTS.SayKey ("iotdemo", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotlance", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotparti", true);
			mTTS.Silence (500, true);

			yield return new WaitUntil (() => mHttpManager.RetrieveDevices);
			mHttpManager.StoreDeploy (true);
			mHttpManager.LightOn (true);
			mHttpManager.SonosPlay (true);
		

			mTTS.SayKey ("iotsomfy", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotassez", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotola", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotcontinuation", true);
			mTTS.Silence (500, true);
		}

		public void StopBehaviour ()
		{
			mTTS.Stop ();
			StopAllCoroutines ();
		}
	}
}