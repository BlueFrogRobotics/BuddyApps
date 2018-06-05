using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class LookingForSomeone : AStateMachineBehaviour
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
			mActionManager.CurrentAction = BUDDY_ACTION.LOOK_FOR_USER;
			mState.text = "Looking for someone";
			Debug.Log("state: Looking 4 someone");
			mLookForSomeone = false;
			mWander = false;
			mWandering = false;

			mLookingTime = 0F;


			if (mActionManager.EmergencyNotif(mDetectionManager.ActiveReminders) == null) {
				Interaction.TextToSpeech.SayKey("anyoneplay", true);
				Interaction.Mood.Set(MoodType.THINKING);
			} else {
				Interaction.Mood.Set(MoodType.SCARED);
				mActionManager.TellNotifPriority(mDetectionManager.ActiveReminders);

			}

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;

			if (Interaction.TextToSpeech.HasFinishedTalking &&  ( (int) (mLookingTime % 20) ) == 10) {
				Debug.Log("[COMPANION][Lookingfor] say something, time: " + mLookingTime );

				if (mDetectionManager.ActiveReminders.Count == 0) {
					Interaction.TextToSpeech.SayKey("anyoneplay", true);
				} else {
					// TODO: check if notif is an emergency
					Debug.Log("[COMPANION][Lookingfor] Tellnotif");
					Interaction.Mood.Set(MoodType.SCARED);
					mActionManager.TellNotif(mDetectionManager.ActiveReminders[0]);
				}
			}

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