﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Buddy;

namespace BuddyApp.ExperienceCenter
{

	public class IdleBehaviour : MonoBehaviour
	{
		public const float ANGLE_THRESHOLD = 0.05f;
		public bool behaviourInit;

		private AnimatorManager mAnimatorManager;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			behaviourInit = false;
			if (!mAnimatorManager.emergencyStop)
				StartCoroutine (InitHeadPosition (30));
			else
				behaviourInit = true;
		}

		private IEnumerator InitHeadPosition (float lSpeed)
		{
			BYOS.Instance.Primitive.Motors.NoHinge.SetPosition (0, lSpeed);
			BYOS.Instance.Primitive.Motors.YesHinge.SetPosition (0, lSpeed);
			yield return new WaitUntil (() => Math.Abs (BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition) < ANGLE_THRESHOLD);
			yield return new WaitUntil (() => Math.Abs (BYOS.Instance.Primitive.Motors.NoHinge.CurrentAnglePosition) < ANGLE_THRESHOLD);
			behaviourInit =  true;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Idle Behaviour");
			StopAllCoroutines ();
		}

	}
}