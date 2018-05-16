using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class Notify : AStateMachineBehaviour
	{

		private float mLookingTime;

		private bool mLookForSomeone;
		private bool mWander;
		private bool mWandering;

		//private Reaction mReaction;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			mDetectionManager.mFacePartTouched = FaceTouch.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NOTIFY;
			mState.text = "Notify";
			Debug.Log("state: Notify ");
			mLookForSomeone = false;
			mWander = false;
			mWandering = false;

			mLookingTime = 0F;
			Interaction.TextToSpeech.SayKey("anyoneplay", true);
			Interaction.Mood.Set(MoodType.THINKING);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;


			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering && CompanionData.Instance.CanMoveBody) {
				Debug.Log("CompanionLooking4 start wandering");
				mActionManager.StartWander();
			} else if (!CompanionData.Instance.CanMoveBody) {
				Trigger("IDLE");
			}

			// TODO: define a strategy here, may be say something from time to time...

			//if (string.IsNullOrEmpty(mActionTrigger)) {
			if (mDetectionManager.mDetectedElement != Detected.NONE) {
				Trigger(mActionManager.LaunchReaction(COMPANION_STATE.LOOK_FOR_USER, mDetectionManager.mDetectedElement));
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

	}
}