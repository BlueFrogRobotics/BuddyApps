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
			mState.text = "Looking for someone";
			Debug.Log("state: Looking 4 someone");
			mLookForSomeone = false;
			mWander = false;
			mWandering = false;

			mLookingTime = 0F;
			Interaction.TextToSpeech.SayKey("anyoneplay", true);
			Interaction.Mood.Set(MoodType.THINKING);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;


			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				Debug.Log("CompanionLooking4 start wandering");
				mActionManager.StartWander(MoodType.NEUTRAL);
			}

			if (mLookingTime > 300) {
				// We didn't find anybody, buddy is sad and wants some attention :/
				if (CompanionData.Instance.InteractDesire < 80) {
					Trigger("IDLE");
				} else {
					Trigger("SADBUDDY");
					// Frustration => raise interact desire
					CompanionData.Instance.InteractDesire = 100;
				}
			} else {

				if (mDetectionManager.mFacePartTouched != FaceTouch.NONE) {
					mDetectionManager.mFacePartTouched = FaceTouch.NONE;
					Trigger("PROPOSEGAME");

				} else {

					switch (mDetectionManager.mDetectedElement) {
						case Detected.TRIGGER:
							Interaction.Mood.Set(MoodType.HAPPY);
							Trigger("PROPOSEGAME");
							break;

						case Detected.TOUCH:
							Interaction.Mood.Set(MoodType.HAPPY);
							Trigger("PROPOSEGAME");
							break;

						case Detected.KIDNAPPING:
							Interaction.Mood.Set(MoodType.HAPPY);
							Trigger("KIDNAPPING");
							break;

						case Detected.BATTERY:
							Trigger("CHARGE");
							break;

						case Detected.THERMAL:
							if (CompanionData.Instance.InteractDesire > 40)
								BYOS.Instance.Interaction.BMLManager.LaunchRandom("joy");
							else
								BYOS.Instance.Interaction.BMLManager.LaunchRandom("surprised");

							Trigger("INTERACT");
							break;

						default:
							break;
					}
				}
			}

		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			mDetectionManager.mDetectedElement = Detected.NONE;
		}


		void OnRandomMinuteActivation()
		{
			// Say something?
			Interaction.TextToSpeech.SayKey("anyoneplay", true);
		}

	}
}