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
		public const float DISTANCE_THRESHOLD = 0.1f;
		public float distance = 1.5f;
		public float wheelSpeed = 200f;

		private AnimatorManager mAnimatorManager;
		private HTTPRequestManager mHttpManager;
		private CollisionDetector mCollisionDetector;
		private TextToSpeech mTTS;
		private float radius;
		private Vector3 mRobotPose;
		private bool mRobotMoving = false;


		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mHttpManager = GameObject.Find ("AIBehaviour").GetComponent<HTTPRequestManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();
			if (ExperienceCenterData.Instance.EnableMovement)
				mCollisionDetector.InitBehaviour ();

			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			if (ExperienceCenterData.Instance.EnableMovement)
				StartCoroutine (MoveForward (wheelSpeed));
			StartCoroutine (Speaking ());
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			mRobotMoving = true;
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
				mRobotMoving = false;
			}
			Debug.LogFormat ("mRobotMoving = {0}", mRobotMoving);
		}

		private IEnumerator Speaking ()
		{
			AttitudeBehaviour lAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();

			lAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);
			mTTS.SayKey ("iotboost", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotrouler", true);
			mTTS.Silence (2000, true);
			yield return new WaitUntil (() => !mRobotMoving || !ExperienceCenterData.Instance.EnableMovement);

			lAttitudeBehaviour.MoveHeadWhileSpeaking(-10, 10);
			mTTS.SayKey ("iotdemo", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotlance", true);
			mTTS.Silence (500, true);

			yield return new WaitUntil (() => mTTS.HasFinishedTalking);

			mTTS.SayKey ("iotparti", true);
			mTTS.Silence (500, true);

//			if (!mHttpManager.Connected)
//				mHttpManager.Login();
//
//			yield return new WaitUntil (() => mHttpManager.RetrieveDevices);
//			mHttpManager.StoreDeploy (true);
//			yield return new WaitUntil(() => ExperienceCenterData.Instance.IsStoreDeployed);
//			mHttpManager.LightOn (true);
//			yield return new WaitUntil(() => ExperienceCenterData.Instance.IsLightOn);
//			mHttpManager.SonosPlay (true);
//			yield return new WaitUntil(() => ExperienceCenterData.Instance.IsMusicOn);

			// Dance for 50 seconds
			DateTime lStartDance = DateTime.Now;
			while (true) {
				TimeSpan lElapsedTime = DateTime.Now - lStartDance;
				if (lElapsedTime.TotalSeconds > 50.0) {
					//mHttpManager.SonosPlay (false);
					BYOS.Instance.Interaction.BMLManager.StopAllBehaviors ();
					break;
				}
				if (ExperienceCenterData.Instance.EnableMovement) { 
					if (BYOS.Instance.Interaction.BMLManager.DonePlaying)
						BYOS.Instance.Interaction.BMLManager.LaunchByName ("Dance01");

					yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
				} else {
					yield return new WaitForSeconds (50f);
					//mHttpManager.SonosPlay (false);
					break;
				}
			}

			mTTS.SayKey ("iotsomfy", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotassez", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotola", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotcontinuation", true);
			mTTS.Silence (500, true);

			yield return new WaitUntil(() => mTTS.HasFinishedTalking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		public void StopBehaviour ()
		{
			mCollisionDetector.StopBehaviour ();
			Debug.LogWarning ("Stop IOT Behaviour");
			StopAllCoroutines ();
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			if (ExperienceCenterData.Instance.EnableMovement)
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
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
