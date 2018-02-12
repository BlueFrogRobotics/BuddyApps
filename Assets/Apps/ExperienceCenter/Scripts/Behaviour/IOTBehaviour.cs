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
		public float wheelSpeed = 200f;

		private AnimatorManager mAnimatorManager;
		private HTTPRequestManager mHttpManager;
		private CollisionDetector mCollisionDetector;
		private TextToSpeech mTTS;
		private Vector3 mRobotPose;
		private float mDistance;
		private bool mRobotMoving;


		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mHttpManager = GameObject.Find ("AIBehaviour").GetComponent<HTTPRequestManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();

			if (ExperienceCenterData.Instance.EnableBaseMovement)
				mCollisionDetector.InitBehaviour ();
			
			mRobotMoving = false;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			mDistance = ExperienceCenterData.Instance.IOTDistance; 

			if (ExperienceCenterData.Instance.EnableBaseMovement)
				StartCoroutine (MoveForward (wheelSpeed));
			StartCoroutine (Scenario ());
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			yield return new WaitForSeconds (1); 
			mRobotMoving = true;
			mDistance = mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			yield return new WaitUntil (() => mCollisionDetector.enableToMove && BYOS.Instance.Interaction.VocalManager.RecognitionFinished);

			BYOS.Instance.Primitive.Motors.Wheels.SetWheelsSpeed (lSpeed);

			Debug.LogFormat ("Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, mDistance);

			yield return new WaitUntil (() => mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)
			|| !mCollisionDetector.enableToMove);

			Debug.LogFormat ("Check condition : Distance = {0}, Obstacle ={1}",  mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD), !mCollisionDetector.enableToMove );
			Debug.LogFormat ("Distance left to travel : {0}", mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose));
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();

			if (! mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				Debug.Log ("End MoveForward Coroutine");
				mRobotMoving = false;
				mCollisionDetector.StopBehaviour ();
			}
			Debug.LogFormat ("mRobotMoving = {0}", mRobotMoving);
		}

		private IEnumerator Scenario ()
		{
			AttitudeBehaviour lAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();

			if (ExperienceCenterData.Instance.EnableHeadMovement)
				lAttitudeBehaviour.MoveHeadWhileSpeaking (-10, 10);
			
			yield return new WaitForSeconds (1);
			mTTS.SayKey ("iotboost", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotrouler", true);
			mTTS.Silence (2000, true);

			yield return new WaitUntil (() => !mRobotMoving || !ExperienceCenterData.Instance.EnableBaseMovement);

			mTTS.SayKey ("iotdemo", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotlance", true);
			mTTS.Silence (500, true);

			yield return new WaitUntil (() => mTTS.HasFinishedTalking);

			mTTS.SayKey ("iotparti", true);
			mTTS.Silence (500, true);

			yield return new WaitForSeconds (2f); 
			if (!mHttpManager.Connected)
				mHttpManager.Login ();

			yield return new WaitForSeconds(2f);
			if (mHttpManager.RetrieveDevices)
			{
				mHttpManager.StoreDeploy(true);
				yield return new WaitForSeconds(10f);
				mHttpManager.LightOn(true);
				yield return new WaitForSeconds(5f);
				mHttpManager.SonosPlay(true);
				yield return new WaitForSeconds(2f);
			}
			else
				Debug.LogError("Could not retrieve device list from targeted Tahoma box");

			yield return new WaitForSeconds (2f);
			// Dance for 50 seconds
			DateTime lStartDance = DateTime.Now;
			while (true) {
				TimeSpan lElapsedTime = DateTime.Now - lStartDance;
				if (lElapsedTime.TotalSeconds > 50.0) {
					mHttpManager.SonosPlay (false);
					BYOS.Instance.Interaction.BMLManager.StopAllBehaviors ();
					break;
				}
				if (ExperienceCenterData.Instance.EnableBaseMovement) { 
					if (BYOS.Instance.Interaction.BMLManager.DonePlaying)
						BYOS.Instance.Interaction.BMLManager.LaunchByName ("dance");

					yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
				} else {
					yield return new WaitForSeconds (50f);
					mHttpManager.SonosPlay (false);
					break;
				}
			}
			yield return new WaitForSeconds (2);

			mTTS.SayKey ("iotsomfy", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotassez", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotola", true);
			mTTS.Silence (500, true);
			mTTS.SayKey ("iotcontinuation", true);
			mTTS.Silence (500, true);

			yield return new WaitUntil (() => mTTS.HasFinishedTalking);
			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop IOT Behaviour");
			StopAllCoroutines ();
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
		}
			
	}
}
