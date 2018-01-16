using System;
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

			if (!mAnimatorManager.emergencyStop && ExperienceCenterData.Instance.EnableMovement)
				StartCoroutine (InitHeadPosition ());
			else
				behaviourInit = true;

		}

		private IEnumerator InitHeadPosition ()
		{
			BYOS.Instance.Interaction.BMLManager.LaunchByName ("Reset01");
			yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
			behaviourInit = true;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Idle Behaviour");
			StopAllCoroutines ();
		}

	}
}