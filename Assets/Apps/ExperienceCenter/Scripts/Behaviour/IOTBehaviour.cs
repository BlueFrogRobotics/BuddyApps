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

		private const float DISTANCE_THRESHOLD = 0.05f;

		private AnimatorManager mAnimatorManager;
		private HTTPRequestManager mHttpManager;
		private CollisionDetector mCollisionDetector;
		private TextToSpeech mTTS;

		private Vector3 mRobotPose;

		private float mDistance;
		private float mMoveTimeOut;

		private bool mRobotMoving;
		private bool mTimeOut;

		public float wheelSpeed = 200f;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mHttpManager = GameObject.Find ("AIBehaviour").GetComponent<HTTPRequestManager> ();
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();

			if (mMoveTimeOut != ExperienceCenterData.Instance.MoveTimeOut) {
				mMoveTimeOut = ExperienceCenterData.Instance.MoveTimeOut; 
				Debug.LogFormat ("[EXCENTER] IOT TimeOut = {0} s ", mMoveTimeOut);
			}

			if (ExperienceCenterData.Instance.EnableBaseMovement)
				mCollisionDetector.InitBehaviour ();
			
			mRobotMoving = false;
			mTimeOut = false;
			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			mDistance = ExperienceCenterData.Instance.IOTDistance; 

			if (ExperienceCenterData.Instance.EnableBaseMovement) {
				StartCoroutine (MoveForward (wheelSpeed));
				StartCoroutine (MoveTimeOut ());
			}
			StartCoroutine (Scenario ());
		}

		private IEnumerator MoveForward (float lSpeed)
		{
			yield return new WaitForSeconds (1); 
			mRobotMoving = true;
			mDistance = mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
			yield return new WaitUntil (() => ((mCollisionDetector.enableToMove && BYOS.Instance.Interaction.VocalManager.RecognitionFinished) || mTimeOut));

			if (!mTimeOut) {
				BYOS.Instance.Primitive.Motors.Wheels.SetWheelsSpeed (lSpeed);

				Debug.LogFormat ("[EXCENTER] Speed = {0}, Distance to travel = {1}", BYOS.Instance.Primitive.Motors.Wheels.Speed, mDistance);

				yield return new WaitUntil (() => mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)
				|| !mCollisionDetector.enableToMove
				|| mTimeOut);
			}
			Debug.LogFormat ("[EXCENTER] Check condition : Distance = {0}, Obstacle ={1}, , TimeOut ={2}", mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD), !mCollisionDetector.enableToMove, mTimeOut);
			Debug.LogFormat ("[EXCENTER] Distance left to travel : {0}", mDistance - CollisionDetector.Distance (BYOS.Instance.Primitive.Motors.Wheels.Odometry, mRobotPose));
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();

			if (!mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD) && !mTimeOut) {
				Debug.Log ("[EXCENTER] Restart Move IOT Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				if (!mTimeOut)
					Debug.Log ("[EXCENTER] End Move IOT Coroutine");
				else
					Debug.Log ("[EXCENTER] End Move IOT Coroutine: (Time-out)");
				
				mRobotMoving = false;
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
					Debug.LogWarning ("[EXCENTER] Robot is stopped");
					TimeSpan lElapsedTime = DateTime.Now - lLastTime;
					if (lElapsedTime.TotalSeconds > mMoveTimeOut) {
						mTimeOut = true;
						break;
					}
				} else {
					Debug.LogWarning ("[EXCENTER] Robot is moving");
					robotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
					lLastTime = DateTime.Now;
				}
				yield return new WaitForSeconds (1.0f);
			}
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

			yield return new WaitForSeconds (2f);
			if (mHttpManager.RetrieveDevices) {
				mHttpManager.StoreDeploy (true);
				yield return new WaitForSeconds (10f);
				mHttpManager.LightOn (true);
				yield return new WaitForSeconds (5f);
				mHttpManager.SonosPlay (true);
				yield return new WaitForSeconds (2f);
			} else
				Debug.LogError ("[EXCENTER] Could not retrieve device list from targeted Tahoma box");

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
			Debug.LogWarning ("[EXCENTER] Stop IOT Behaviour");
			StopAllCoroutines ();
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				BYOS.Instance.Primitive.Motors.Wheels.Stop ();
		}
			
	}
}
