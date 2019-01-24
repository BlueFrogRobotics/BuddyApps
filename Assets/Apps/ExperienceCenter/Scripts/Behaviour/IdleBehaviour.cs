using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlueQuark;

namespace BuddyApp.ExperienceCenter
{

	public class IdleBehaviour : MonoBehaviour
	{
		public bool headPoseInit;
		public bool behaviourEnd;

		private const double IDLE_TIMEOUT = 20;

		private AnimatorManager mAnimatorManager;
		private AttitudeBehaviour mAttitudeBehaviour;
		private QuestionsBehaviour mQuestionBehaviour;

		public void InitBehaviour ()
		{
			mAnimatorManager = GameObject.Find ("AIBehaviour").GetComponent<AnimatorManager> ();
			mAttitudeBehaviour = GameObject.Find ("AIBehaviour").GetComponent<AttitudeBehaviour> ();
			mQuestionBehaviour = GameObject.Find ("AIBehaviour").GetComponent<QuestionsBehaviour> ();

			behaviourEnd = false;

			headPoseInit = false;

			if (!mAnimatorManager.emergencyStop && ExperienceCenterData.Instance.EnableHeadMovement) {
				StartCoroutine (Idle ());
			} else
				headPoseInit = true;
		}

		private IEnumerator InitHeadPosition ()
		{
            Buddy.Behaviour.Interpreter.StopAndClear();
            yield return new WaitForSeconds(1.0F);
            Buddy.Behaviour.Interpreter.Run("reset.xml");
            //BYOS.Instance.Interaction.BMLManager.LaunchByName ("Reset01");
            Debug.Log("INIT HEAD!!!!!!!!!!!!!");
            yield return new WaitUntil (() => !Buddy.Behaviour.Interpreter.IsBusy);
            Debug.Log("FINI INIT HEAD!!!!!!!!!!!!!");
            yield return null;
			headPoseInit = true;
		}

		public void StopBehaviour ()
		{
			Debug.LogWarning ("[EXCENTER] Stop Idle Behaviour");
			StopAllCoroutines ();
			if (ExperienceCenterData.Instance.EnableHeadMovement && !headPoseInit) {
				mAttitudeBehaviour.IsWaiting = false;
				//BYOS.Instance.Interaction.BMLManager.StopAllBehaviors ();
				StartCoroutine (InitHeadPosition ());
			}
			behaviourEnd = true;
		}

		private IEnumerator Idle ()
		{
			while (true) {
				if (/*ExperienceCenterData.Instance.VoiceTrigger*//*Buddy.Vocal.EnableTrigger && */!mAttitudeBehaviour.IsWaiting && !behaviourEnd) {
					mAttitudeBehaviour.StartWaiting ();
					headPoseInit = false;
				}

				yield return new WaitForSeconds (5.0f);
			}
		}

	}
}
