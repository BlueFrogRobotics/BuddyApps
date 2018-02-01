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
		public bool headPoseInit;
		public bool behaviourEnd;

		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		private QuestionsBehaviour mQuestionBehaviour;
		private const double IDLE_TIMEOUT = 20;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			mQuestionBehaviour = GameObject.Find("AIBehaviour").GetComponent<QuestionsBehaviour>();
			behaviourEnd = false;

			headPoseInit = false;

			if (!mAnimatorManager.emergencyStop && ExperienceCenterData.Instance.EnableHeadMovement)
			{
				StartCoroutine(Idle());
			}
			else
				headPoseInit = true;

		}

		private IEnumerator InitHeadPosition ()
		{
			BYOS.Instance.Interaction.BMLManager.LaunchByName ("Reset01");
			yield return new WaitUntil (() => BYOS.Instance.Interaction.BMLManager.DonePlaying);
			headPoseInit = true;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("Stop Idle Behaviour");
			StopAllCoroutines ();
			if (ExperienceCenterData.Instance.EnableHeadMovement && !headPoseInit)
			{
				mAttitudeBehaviour.IsWaiting = false;
				BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
				StartCoroutine (InitHeadPosition ());
			}
			behaviourEnd = true;
		}

		private IEnumerator Idle()
		{
			TimeSpan lElapsedTimeSinceLastTTS;
			while(true)
			{
				lElapsedTimeSinceLastTTS = DateTime.Now - mQuestionBehaviour.LastSTTCallbackTime;

				if (!mAttitudeBehaviour.IsWaiting && lElapsedTimeSinceLastTTS.TotalSeconds > IDLE_TIMEOUT) {
					Debug.LogWarning ("Start Waiting BML");
					mAttitudeBehaviour.StartWaiting ();
					headPoseInit = false;
				}

				yield return new WaitForSeconds(0.5f);
			}
		}

	}
}
