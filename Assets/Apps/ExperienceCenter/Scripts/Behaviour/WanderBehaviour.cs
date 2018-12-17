﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{
	public class WanderBehaviour : MonoBehaviour
	{
		private const float DISTANCE_THRESHOLD = 0.05f;

		private CollisionDetector mCollisionDetector;

		private Vector3 mRobotPose;

		private float mDistance;

		public float wheelSpeed = 200f;

		public bool behaviourEnd;

		public void InitBehaviour ()
		{
			mCollisionDetector = GameObject.Find ("AIBehaviour").GetComponent<CollisionDetector> ();

			behaviourEnd = false;
			if (ExperienceCenterData.Instance.EnableBaseMovement)
				mCollisionDetector.InitBehaviour ();

			mDistance = 5F; 
			if (ExperienceCenterData.Instance.EnableBaseMovement) {
				Debug.Log("[EXCENTER] Not Launching Random Walk");
				//BYOS.Instance.Navigation.RandomWalk.StartWander (MoodType.NEUTRAL);
			} else {
				behaviourEnd = true;
			}
		}

		private IEnumerator Walk ()
		{
			mDistance = mDistance - CollisionDetector.Distance (Buddy.Actuators.Wheels.Odometry, mRobotPose);
			//Save the robot Pose for future iteration if any
			mRobotPose = Buddy.Actuators.Wheels.Odometry;

			yield return new WaitUntil (() => /*BYOS.Instance.Interaction.SpeechToText.HasFinished*/!Buddy.Vocal.IsListening);
			Debug.LogFormat ("[EXCENTER] Middle Speed = {0}, Right Speed = {1}, Left Speed = {2}", mCollisionDetector.middleSpeed, mCollisionDetector.rightSpeed, mCollisionDetector.leftSpeed);
//			if (mCollisionDetector.middleSpeed <= 0) {
//				BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (mCollisionDetector.middleSpeed, mCollisionDetector.middleSpeed, mDistance, 0.01F);
//			} else 

			//if (mCollisionDetector.middleSpeed < mCollisionDetector.rightSpeed || mCollisionDetector.middleSpeed < mCollisionDetector.leftSpeed) {
			//	if (mCollisionDetector.rightSpeed < mCollisionDetector.leftSpeed) {
			//		BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (mCollisionDetector.middleSpeed, mCollisionDetector.leftSpeed, mDistance, 0.01F);
			//	} else {
			//		BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (mCollisionDetector.rightSpeed, mCollisionDetector.middleSpeed, mDistance, 0.01F);
			//	}
			//} else {
			//	BYOS.Instance.Primitive.Motors.Wheels.MoveDistance (mCollisionDetector.rightSpeed, mCollisionDetector.leftSpeed, mDistance, 0.01F);
			//}
			//yield return new WaitUntil (() => mCollisionDetector.updateSpeed || mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD));

			//if (mCollisionDetector.CheckDistance (mDistance, mRobotPose, DISTANCE_THRESHOLD)) {
			//	mDistance = 5F;
			//	BYOS.Instance.Interaction.BMLManager.LaunchRandom("Idle");
			//	yield return new WaitUntil(() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
			//}
			Debug.Log ("[EXCENTER] Restart Wander Coroutine");
			StartCoroutine (Walk ());
			
		}
			

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Wander Behaviour");
			mCollisionDetector.StopBehaviour ();
			StopAllCoroutines ();
			//if (ExperienceCenterData.Instance.EnableBaseMovement)
			//	BYOS.Instance.Navigation.RandomWalk.StopWander();
			behaviourEnd = true;
		}
			
	}
}