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
		private float mTimeRaise;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mActionManager.CurrentAction = BUDDY_ACTION.WANDER;

			if (CompanionData.Instance.mMovingDesire < 8)
				CompanionData.Instance.mMovingDesire = 8;


			mDetectionManager.mDetectedElement = Detected.NONE;
			mTimeThermal = 0F;
			mTimeRaise = 0F;
			mTimeLastThermal = 0F;
			mTrigged = false;
			Debug.Log("state:  Wander");
			Interaction.TextToSpeech.SayKey("startwander", true);

			Debug.Log("wander: " + CompanionData.Instance.mMovingDesire);


		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mState.text = "WANDER \n interactDesire: " + CompanionData.Instance.mInteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.mMovingDesire;

			// Hack to remove trigger when wander because of false positive
			if (mDetectionManager.IsDetectingTrigger != CompanionData.Instance.CanTriggerWander)
				if (CompanionData.Instance.CanTriggerWander)
					mDetectionManager.StartSphinxTrigger();
				else
					mDetectionManager.StopSphinxTrigger();


			///////// TODO: REWORK NEEDED
			// what mood to wander?
			// thermal follow: use follow action?
			mTimeRaise += Time.deltaTime;
			if (mTimeRaise > 30F) {
				mTimeRaise = 0F;
				if (UnityEngine.Random.Range(0, 2) == 0)
					OnRandomMinuteActivation();
			}


			if (!Interaction.BMLManager.DonePlaying)
				mState.text += "\n BML: " + Interaction.BMLManager.ActiveBML[0].Name;
			//else if (mActionManager.ThermalFollow) {
			//	mState.text += "\n thermal follow <3";
			//}

			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.ActiveAction() && CompanionData.Instance.CanMoveBody) {
				Debug.Log("CompanionWander start wandering");
				mActionManager.StartWander(mActionManager.WanderingMood);
			} else if (!CompanionData.Instance.CanMoveBody)
				Trigger("IDLE");


			// if we follow for a while, or lose the target for 5 seconds, go back to wander:
			//if (mActionManager.ThermalFollow && (Time.time - mTimeThermal > CompanionData.Instance.mInteractDesire
			//	|| (Time.time - mTimeLastThermal > 5.0F) && Interaction.BMLManager.DonePlaying && CompanionData.Instance.CanMoveBody))
			//	mActionManager.StartWander(mActionManager.WanderingMood);

			//////////////



			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state

			if (mDetectionManager.mDetectedElement != Detected.NONE) {

				string lTrigger = mActionManager.LaunchReaction(COMPANION_STATE.WANDER, mDetectionManager.mDetectedElement);
				if (!string.IsNullOrEmpty(lTrigger)) {
					Trigger(lTrigger);
					mTrigged = true;
				} else
					mTrigged = false;
			}

			//switch (mDetectionManager.mDetectedElement) {


			//	case Detected.TRIGGER:
			//		mTrigged = true;
			//		Trigger("VOCALTRIGGERED");
			//		break;

			//	case Detected.TOUCH:
			//		mTrigged = true;
			//		Trigger("ROBOTTOUCHED");
			//		break;

			//	case Detected.KIDNAPPING:
			//		mTrigged = true;
			//		Trigger("KIDNAPPING");
			//		break;

			//	case Detected.BATTERY:
			//		mTrigged = true;
			//		Trigger("CHARGE");
			//		break;

			//	// If thermal signature, activate thermal follow for some time
			//	case Detected.THERMAL:
			//		mTimeLastThermal = Time.time;
			//		if (CompanionData.Instance.InteractDesire > 80 && CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
			//			mTrigged = true;
			//			Trigger("INTERACT");
			//		} else if (CompanionData.Instance.InteractDesire > 30 && !mActionManager.ThermalFollow && Interaction.BMLManager.DonePlaying && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
			//			//Stop wandering and go to thermal follow
			//			Debug.Log("CompanionWander start following " + CompanionData.Instance.InteractDesire);
			//			mTimeThermal = Time.time;
			//			mDetectionManager.mDetectedElement = Detected.NONE;
			//			Interaction.Mood.Set(MoodType.HAPPY);
			//			mActionManager.StartThermalFollow(HumanFollowType.BODY);
			//		}
			//		break;

			//	//case Detected.HUMAN_RGB & Detected.THERMAL:
			//	//	// TODO: check false positive level
			//	//	mTrigged = true;
			//	//	if (CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
			//	//		mTrigged = true;
			//	//		Trigger("INTERACT");
			//	//	}
			//	//	break;

			//	default:
			//		mDetectionManager.mDetectedElement = Detected.NONE;
			//		break;
			//}

			if (!mTrigged) {
				// TODO maybe check the situation only every x seconds
				// if < 20
				if (CompanionData.Instance.mMovingDesire < 5) {
					string lTrigger = mActionManager.LaunchDesiredAction(COMPANION_STATE.WANDER);
					if (!string.IsNullOrEmpty(lTrigger)) {
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, -3, "moodwander", "WANDEREND", EmotionalEventType.FULFILLED_DESIRE, InternalMood.RELAXED));
						Trigger(lTrigger);
					}
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mActionManager.StopAllActions();
			mDetectionManager.StartSphinxTrigger();
			mDetectionManager.mDetectedElement = Detected.NONE;

			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}



		void OnRandomMinuteActivation()
		{
			// TODO REWORK THIS => avoid "random"
			if (CompanionData.Instance.CanMoveHead)
				mActionManager.RandomActionWander();
		}

	}
}