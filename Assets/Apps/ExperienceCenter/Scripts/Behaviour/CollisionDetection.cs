using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{


	public class CollisionDetector : MonoBehaviour
	{

		//List of booleans for detecting collision
		private bool mObstacle;
		private bool mStoppingPhase;
		private bool mBehaviourInit;
		public bool enableToMove;

		public void InitBehaviour ()
		{
			mObstacle = false;
			mStoppingPhase = false;
			mBehaviourInit = true;
			enableToMove = true;
		}
			
		private void CheckObstacle ()
		{
			float leftObs = BYOS.Instance.Primitive.IRSensors.Left.Distance;
			float rightObs = BYOS.Instance.Primitive.IRSensors.Right.Distance;
			float middleObs = BYOS.Instance.Primitive.IRSensors.Middle.Distance;
			if (leftObs >= 0.01f || rightObs >= 0.01f || middleObs >= 0.01f) {
				if (!mObstacle) {
					Debug.LogWarning ("Stopping Buddy");
					mStoppingPhase = true;
					mObstacle = true;
					enableToMove = false;
				} else {
					if (BYOS.Instance.Primitive.Motors.Wheels.Speed <= 0.01f) {
						Debug.LogWarning ("Buddy Stopped: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
						mStoppingPhase = false;
						enableToMove = false;
					} else {
						mStoppingPhase = true;
						enableToMove = false;
						Debug.LogWarning ("Buddy is Slipping: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					}

				}
			} else {
				if (!mStoppingPhase) {
					//Debug.LogWarning ("Safe Evironment: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
					mObstacle = false;
					enableToMove = true;
				} else {
					enableToMove = false;
					mStoppingPhase = true;
					Debug.LogError ("There is a collision: L= " + leftObs + ", M= " + middleObs + ", R= " + rightObs + ", V= " + BYOS.Instance.Primitive.Motors.Wheels.Speed);
				}
			}
		}

		static public float Distance (Vector3 v1, Vector3 v2)
		{
			float d = (float)(Math.Sqrt ((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z)));
			return d;
		}

		void Update ()
		{
			if(mBehaviourInit)
				CheckObstacle ();
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Collision Detection");
			mBehaviourInit = false;
		}

	}
}