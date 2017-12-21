using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class CompanionWander : AStateMachineBehaviour
	{
		private bool mTrigged;
		private float mTimeThermal;
		private float mTimeLastThermal;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mTimeThermal = 0F;
			mTimeLastThermal = 0F;
            mState.text = "Wander";
			mTrigged = false;
			Debug.Log("state: Wander");
			Interaction.TextToSpeech.SayKey("startwander", true);

			Debug.Log("wander: " + CompanionData.Instance.MovingDesire);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;


		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering) {
				Debug.Log("CompanionWander start wandering");
				mActionManager.StartWander(MoodType.NEUTRAL);
			}

			// if we foolow for a while, or lose the target for 5 seconds, go back to wander:
			if (mActionManager.ThermalFollow && (Time.time - mTimeThermal > CompanionData.Instance.InteractDesire
				|| (Time.time - mTimeLastThermal > 5.0F)))
				mActionManager.StartWander(MoodType.NEUTRAL);

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			switch (mDetectionManager.mDetectedElement) {
				case Detected.TRIGGER:
					mTrigged = true;
					Trigger("VOCALTRIGGERED");
					break;

				case Detected.TOUCH:
					mTrigged = true;
					Trigger("ROBOTTOUCHED");
					break;

				case Detected.KIDNAPPING:
					mTrigged = true;
					Trigger("KIDNAPPING");
					break;

				case Detected.BATTERY:
					mTrigged = true;
					Trigger("CHARGE");
					break;

				// If thermal signature, activate thermal follow for some time
				case Detected.THERMAL:
					mTimeLastThermal = Time.time;
					if (CompanionData.Instance.InteractDesire > 80 && CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
						mTrigged = true;
						Trigger("INTERACT");
					} else if (CompanionData.Instance.InteractDesire > 30 && !mActionManager.ThermalFollow) {
						//Stop wandering and go to thermal follow
						Debug.Log("CompanionWander start following " + CompanionData.Instance.InteractDesire);
						mTimeThermal = Time.time;
						mDetectionManager.mDetectedElement = Detected.NONE;
						Interaction.Mood.Set(MoodType.HAPPY);
						mActionManager.StartThermalFollow(HumanFollowType.BODY);
					}
					break;

				//case Detected.HUMAN_RGB & Detected.THERMAL:
				//	// TODO: check false positive level
				//	mTrigged = true;
				//	if (CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
				//		mTrigged = true;
				//		Trigger("INTERACT");
				//	}
				//	break;

				default:
					break;
			}

			if (!mTrigged) {
				if (CompanionData.Instance.MovingDesire < 5) {
					Debug.Log("wander -> IDLE: " + CompanionData.Instance.MovingDesire);
					Trigger("IDLE");
				} else if (CompanionData.Instance.InteractDesire > 80 && CompanionData.Instance.MovingDesire < 20) {
					Trigger("LOOKINGFOR");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			mActionManager.StopWander();
			mActionManager.StopThermalFollow();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}



		void OnRandomMinuteActivation()
		{
			// Buddy wants more and more to interact
			CompanionData.Instance.InteractDesire += 10;

			// Buddy is moving around, so he wants less and less to move around
			CompanionData.Instance.MovingDesire -= 10;
		}

	}
}