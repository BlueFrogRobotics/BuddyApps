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
			mCollisionDetector.InitBehaviour ();

			mTTS = BYOS.Instance.Interaction.TextToSpeech;
			radius = BYOS.Instance.Primitive.Motors.Wheels.Radius;
			mRobotPose = BYOS.Instance.Primitive.Motors.Wheels.Odometry;
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

			if (distance - Math.Abs (BYOS.Instance.Primitive.Motors.Wheels.Odometry.x - mRobotPose.x) > DISTANCE_THRESHOLD) {
				Debug.Log ("Restart MoveForward Coroutine");
				StartCoroutine (MoveForward (wheelSpeed));
			} else {
				mRobotMoving = false;
			}
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

			if (!mHttpManager.Connected)
				mHttpManager.Login();

			yield return new WaitUntil (() => mHttpManager.RetrieveDevices);
			mHttpManager.StoreDeploy (true);
			mHttpManager.LightOn (true);
			mHttpManager.SonosPlay (true);

            // Dance for 50 seconds
            DateTime lStartDance = DateTime.Now;
            while(true)
            {
                TimeSpan lElapsedTime = DateTime.Now - lStartDance;
                if (lElapsedTime.TotalSeconds > 50.0)
                {
                    mHttpManager.SonosPlay(false);
                    BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
                    break;
                }

                if (BYOS.Instance.Interaction.BMLManager.DonePlaying)
                    BYOS.Instance.Interaction.BMLManager.LaunchByName("Dance01");

                yield return new WaitUntil(() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
            }

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
			mCollisionDetector.StopBehaviour ();
			Debug.LogWarning ("Stop IOT Behaviour");
			StopAllCoroutines ();
			if (!mTTS.HasFinishedTalking)
				mTTS.Stop ();
			BYOS.Instance.Primitive.Motors.Wheels.Stop ();
		}
	}
}
