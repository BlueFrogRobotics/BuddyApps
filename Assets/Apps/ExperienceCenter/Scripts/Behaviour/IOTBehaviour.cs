using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{


	public class IOTBehaviour : MonoBehaviour
	{

		private const float DISTANCE_THRESHOLD = 0.05f;

		private AnimatorManager mAnimatorManager;
		private HTTPRequestManager mHttpManager;
		private CollisionDetector mCollisionDetector;
		//private TextToSpeech mTTS;

		private Vector3 mRobotPose;

		private float mDistance;
		private float mMoveTimeOut;

		private bool mRobotMoving;
		private bool mTimeOut;

		public float wheelSpeed = 0.4F;

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
			//mTTS = BYOS.Instance.Interaction.TextToSpeech;
			mRobotPose = Buddy.Actuators.Wheels.Odometry;
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
			mDistance = mDistance - CollisionDetector.Distance (Buddy.Actuators.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = Buddy.Actuators.Wheels.Odometry;
			yield return new WaitUntil (() => ((mCollisionDetector.enableToMove && !Buddy.Vocal.IsListening) || mTimeOut));

			if (!mTimeOut) {
                //Buddy.Actuators.Wheels.SetVelocities(lSpeed, 0);
                Buddy.Navigation.Run<DisplacementStrategy>().Move(5, 0.4F);

                Debug.LogFormat ("[EXCENTER] Speed = {0}, Distance to travel = {1}", Buddy.Actuators.Wheels.Speed, mDistance);

				yield return new WaitUntil (() => mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)
				|| !mCollisionDetector.enableToMove
				|| mTimeOut);
			}
			Debug.LogFormat ("[EXCENTER] Check condition : Distance = {0}, Obstacle ={1}, , TimeOut ={2}", mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD), !mCollisionDetector.enableToMove, mTimeOut);
			Debug.LogFormat ("[EXCENTER] Distance left to travel : {0}", mDistance - CollisionDetector.Distance (Buddy.Actuators.Wheels.Odometry, mRobotPose));
			Buddy.Actuators.Wheels.Stop();

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
			Vector3 robotPose = Buddy.Actuators.Wheels.Odometry;
			DateTime lLastTime = DateTime.Now;
			while (!mTimeOut) {
				if (CollisionDetector.Distance (Buddy.Actuators.Wheels.Odometry, robotPose) <= DISTANCE_THRESHOLD) {
					Debug.LogWarning ("[EXCENTER] Robot is stopped");
					TimeSpan lElapsedTime = DateTime.Now - lLastTime;
					if (lElapsedTime.TotalSeconds > mMoveTimeOut) {
						mTimeOut = true;
						break;
					}
				} else {
					Debug.LogWarning ("[EXCENTER] Robot is moving");
					robotPose = Buddy.Actuators.Wheels.Odometry;
					lLastTime = DateTime.Now;
				}
				yield return new WaitForSeconds (1.0f);
			}
		}

		private IEnumerator Scenario ()
		{
            Debug.Log("debut scenario");
			AttitudeBehaviour lAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();

			if (ExperienceCenterData.Instance.EnableHeadMovement)
				lAttitudeBehaviour.MoveHeadWhileSpeaking (-10, 10);
            Debug.Log("iotbahaviour1");
			
			yield return new WaitForSeconds (1);
			Buddy.Vocal.SayKey ("iotboost", true);
            Buddy.Vocal.Say("[250]", true);
            Buddy.Vocal.SayKey ("iotrouler", true);
            Buddy.Vocal.Say("[1000]", true);

            yield return new WaitUntil (() => !mRobotMoving || !ExperienceCenterData.Instance.EnableBaseMovement);
            Debug.Log("iotbahaviour2");
			Buddy.Vocal.SayKey ("iotdemo", true);
            Buddy.Vocal.Say("[250]", true);
            Buddy.Vocal.SayKey ("iotlance", true);
            Buddy.Vocal.Say("[250]", true);
            Debug.Log("iotbahaviour3");

            yield return new WaitUntil (() => !Buddy.Vocal.IsSpeaking);
            Debug.Log("iotbahaviour4");

			Buddy.Vocal.SayKey ("iotparti", true);
            Buddy.Vocal.Say("[250]", true);

            yield return new WaitForSeconds (2f); 
			if (!mHttpManager.Connected)
				mHttpManager.Login ();
            Debug.Log("iotbahaviour5");

			yield return new WaitForSeconds (2f);
			if (mHttpManager.RetrieveDevices) {
				mHttpManager.StoreDeploy (true);
				yield return new WaitForSeconds (10f);
				mHttpManager.LightOn (true);
				yield return new WaitForSeconds (5f);
				mHttpManager.SonosPlay (true);
				yield return new WaitForSeconds (1f);
			} else
				Debug.LogError ("[EXCENTER] Could not retrieve device list from targeted Tahoma box");

            Debug.Log("iotbahaviour6");
			//yield return new WaitForSeconds (2f);
            //Buddy.Behaviour.Interpreter.Run(Buddy.Resources.GetRawFullPath("reset.xml"));
            // Dance for 30 seconds (default)
            DateTime lStartDance = DateTime.Now;
			while (true) {
				TimeSpan lElapsedTime = DateTime.Now - lStartDance;
				if (lElapsedTime.TotalSeconds > ExperienceCenterData.Instance.DanceDuration) {
					mHttpManager.SonosPlay (false);
                    Buddy.Behaviour.Interpreter.Stop();
                    //BYOS.Instance.Interaction.BMLManager.StopAllBehaviors ();
                    break;
				}
				if (ExperienceCenterData.Instance.EnableBaseMovement && !Buddy.Behaviour.Interpreter.IsBusy) { 
                    Buddy.Behaviour.Interpreter.Run("dance1.xml");
				//	BYOS.Instance.Interaction.BMLManager.LaunchByName ("dance");
				}
				yield return new WaitForSeconds(1f);
			}
            Debug.Log("iotbahaviour7");

			yield return new WaitForSeconds (2f);

			Buddy.Vocal.SayKey ("iotsomfy", true);
            Buddy.Vocal.Say("[250]", true);
            Buddy.Vocal.SayKey ("iotassez", true);
            Buddy.Vocal.Say("[250]", true);
            Buddy.Vocal.SayKey ("iotola", true);
            Buddy.Vocal.Say("[250]", true);
            Buddy.Vocal.SayKey ("iotcontinuation", true);

            Debug.Log("iotbahaviour8");
			yield return new WaitUntil (() => !Buddy.Vocal.IsSpeaking);

			if (!mHttpManager.Connected)
				mHttpManager.Login ();
			yield return new WaitForSeconds (1f);
			if (mHttpManager.RetrieveDevices)
			{
				mHttpManager.StoreDeploy (false);
				yield return new WaitForSeconds (2f);
				mHttpManager.LightOn (false);
			}

			mAnimatorManager.ActivateCmd ((byte)(Command.Stop));
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop IOT Behaviour");
			StopAllCoroutines ();
			if (Buddy.Vocal.IsSpeaking)
                Buddy.Vocal.StopListening ();
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				Buddy.Actuators.Wheels.Stop();
		}
			
	}
}
