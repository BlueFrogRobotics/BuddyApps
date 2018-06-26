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


			if (mDetectionManager.ActiveUrgentReminder) {
				Interaction.Mood.Set(MoodType.SCARED);
				Debug.Log("[COMPANION][Lookingfor] Set mood to scared for notif! " + Interaction.Mood.CurrentMood);
				mActionManager.InformNotifPriority();
			} else {
				Interaction.TextToSpeech.SayKey("anyoneplay", true);
				Interaction.Mood.Set(MoodType.THINKING);
				Debug.Log("[COMPANION][Lookingfor] Set mood to Thinking for notif! " + Interaction.Mood.CurrentMood);
			}

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime += Time.deltaTime;

			if (Interaction.TextToSpeech.HasFinishedTalking && (mLookingTime > 20F)) {
				Debug.Log("[COMPANION][Lookingfor] say something, time: " + mLookingTime);
				Debug.Log("[COMPANION][Lookingfor] current mood: " + Interaction.Mood.CurrentMood);

				if (!mDetectionManager.ActiveReminder) {
					Debug.Log("[COMPANION][Lookingfor] anyone  play?");
					Interaction.TextToSpeech.SayKey("anyoneplay", true);
					if (Interaction.Mood.CurrentMood != MoodType.THINKING && Interaction.Face.IsStable)
						Interaction.Mood.Set(MoodType.THINKING);
				} else {
					// TODO: check if notif is an emergency
					Debug.Log("[COMPANION][Lookingfor] Tellnotif");
					if (Interaction.Mood.CurrentMood != MoodType.SCARED && Interaction.Face.IsStable)
						Interaction.Mood.Set(MoodType.SCARED);
					mActionManager.InformNotifPriority();
				}
				mLookingTime = 0F;
			}

			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering && CompanionData.Instance.CanMoveBody) {
				Debug.Log("CompanionLooking4 start wandering");
				mActionManager.StartWander(Interaction.Mood.CurrentMood);
			} else if (!CompanionData.Instance.CanMoveBody && !mDetectionManager.ActiveReminder) {
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
			Debug.Log("[COMPANION][Lookingfor] quit");
		}

	}
}