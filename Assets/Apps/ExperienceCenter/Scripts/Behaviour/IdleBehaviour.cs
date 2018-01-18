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
		public bool behaviourInit;

		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		private QuestionsBehaviour mQuestionBehaviour;
		private const double IDLE_TIMEOUT = 10;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find("AIBehaviour").GetComponent<AttitudeBehaviour>();
			mQuestionBehaviour = GameObject.Find("AIBehaviour").GetComponent<QuestionsBehaviour>();

			behaviourInit = false;

			if (!mAnimatorManager.emergencyStop && ExperienceCenterData.Instance.EnableHeadMovement)
			{
				StartCoroutine(InitHeadPosition());
				StartCoroutine(Idle());
			}
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

		private IEnumerator Idle()
		{
			TimeSpan lElapsedTimeSinceLastTTS;
			while(true)
			{
				lElapsedTimeSinceLastTTS = DateTime.Now - mQuestionBehaviour.LastSTTCallbackTime;

				if(!mAttitudeBehaviour.IsWaiting && lElapsedTimeSinceLastTTS.TotalSeconds>IDLE_TIMEOUT)
					mAttitudeBehaviour.StartWaiting();
			}
		}

	}
}
