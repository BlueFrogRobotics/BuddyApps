using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class UserDetected : AStateMachineBehaviour
	{

		private float mTimeState;
		private float mTimeHumanDetected;
		private string mActionTrigger;

		public override void Start()
		{
			Utils.LogI(LogContext.APP, "Start UserD");
			CompanionData.Instance.Bored = 0;
			CompanionData.Instance.MovingDesire = 0;
			CompanionData.Instance.InteractDesire = 0;
			CompanionData.Instance.ChargeAsked = false;
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			Utils.LogI(LogContext.APP, "Start UserD");

		}




		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			Utils.LogI(LogContext.APP, "Enter UserD 0");
			mState.text = "User Detected";
			mActionTrigger = "";
			mTimeState = 0F;
			mTimeHumanDetected = 0F;


			Utils.LogI(LogContext.APP, "Enter UserD 0");

		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "User Detected move: " + !BYOS.Instance.Primitive.Motors.Wheels.Locked;



			mTimeHumanDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			if (BYOS.Instance.Interaction.BMLManager.DonePlaying && !mActionManager.ThermalFollow && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				mActionManager.StartThermalFollow(HumanFollowType.ROTATION_AND_HEAD);
			}

			// Play BML after 5 seconds every 15 seconds or launch desired action
			if (((int)mTimeHumanDetected) % 15 == 5 && BYOS.Instance.Interaction.BMLManager.DonePlaying) {
				mActionTrigger = mActionManager.LaunchDesiredAction("USERDETECTED");
				if (string.IsNullOrEmpty(mActionTrigger)) {
					//if no desired action, play BML
					Debug.Log("Play neutral BML user detected");
					//TODO: play other BML?
					mActionManager.StopAllActions();
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");
				} else {
					// Otherwise trigger to perform the action
					Trigger(mActionTrigger);
				}
			}

			// If human not there anymore
			//if (mTimeHumanDetected > 8F) {
			//	if (CompanionData.Instance.InteractDesire > 80)
			//		Trigger("SADBUDDY");
			//	else if (CompanionData.Instance.InteractDesire > 50 && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
			//		Trigger("LOOKINGFOR");
			//	else if (CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
			//		if (CompanionData.Instance.MovingDesire < 30)
			//			CompanionData.Instance.MovingDesire += 30;
			//		Trigger("WANDER");
			//	} else
			//		Trigger("IDLE");


			// 2) If human detected for a while and want to interact but no interaction, go to Crazy Buddy
			//if (mTimeState > 15F && mDetectionManager.TimeLastTouch > 5F && CompanionData.Instance.InteractDesire > 50) {
			//	BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			//	Interaction.Face.SetEvent(FaceEvent.SMILE);
			//	Trigger("SEEKATTENTION");

			//	// 3) Otherwise, go wander
			//} else if (mTimeState > 20F && mDetectionManager.TimeLastTouch > 5F && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
			//	if (CompanionData.Instance.MovingDesire < 30)
			//		CompanionData.Instance.MovingDesire += 30;
			//	Interaction.Mood.Set(MoodType.SURPRISED);
			//	Trigger("WANDER");
			//} else {



			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			if (string.IsNullOrEmpty(mActionTrigger)) {
				if (mDetectionManager.mDetectedElement == Detected.TRIGGER || mDetectionManager.mDetectedElement == Detected.TOUCH || mDetectionManager.mDetectedElement == Detected.THERMAL ||
					mDetectionManager.mDetectedElement == Detected.KIDNAPPING || mDetectionManager.mDetectedElement == Detected.BATTERY || mDetectionManager.mDetectedElement == Detected.HUMAN_RGB) {

					Trigger(mActionManager.LaunchReaction("IDLE", mDetectionManager.mDetectedElement));

				}
			}

			//switch (mDetectionManager.mDetectedElement) {
			//	case Detected.TRIGGER:
			//		Trigger("VOCALTRIGGERED");
			//		break;

			//	case Detected.TOUCH:
			//		Debug.Log("User Detected robot touched");
			//		Trigger("ROBOTTOUCHED");
			//		break;

			//	case Detected.KIDNAPPING:
			//		Trigger("KIDNAPPING");
			//		break;

			//	case Detected.BATTERY:
			//		Trigger("CHARGE");
			//		break;

			//	case Detected.THERMAL:
			//		mTimeHumanDetected = 0F;
			//		mDetectionManager.mDetectedElement = Detected.NONE;
			//		break;

			//	default:
			//		mDetectionManager.mDetectedElement = Detected.NONE;
			//		break;
			//}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("User detected exit");
			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}
	}
}