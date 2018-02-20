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
		private float mTimeHumanLastDetected;
		private float mDurationDetected;

		public override void Start()
		{
			Utils.LogI(LogContext.APP, "Start UserD");
			CompanionData.Instance.mMovingDesire = 0;
			CompanionData.Instance.mInteractDesire = 0;
			CompanionData.Instance.ChargeAsked = false;
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			Utils.LogI(LogContext.APP, "Start UserD");

		}




		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			Utils.LogI(LogContext.APP, "Enter UserD");
			mTimeState = 0F;
			mTimeHumanLastDetected = 0F;
			mDurationDetected = 0F;

		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "User Detected move: " + !BYOS.Instance.Primitive.Motors.Wheels.Locked;

			// if human is not detected for 1.5 seconds
			if(mTimeHumanLastDetected > 1.5F) {
				mDurationDetected = 0F;
			} else {
				mDurationDetected += Time.deltaTime;
			}

			mTimeHumanLastDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			if (BYOS.Instance.Interaction.BMLManager.DonePlaying && !mActionManager.ThermalFollow && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				mActionManager.StartThermalFollow(HumanFollowType.ROTATION_AND_HEAD);
			}

			// Play BML after 5 seconds every 15 seconds or launch desired action
			if (((int)mTimeState) % 15 == 5 && BYOS.Instance.Interaction.BMLManager.DonePlaying && mTimeHumanLastDetected < 8F) {

				// We launch desired action for USER_DETECTED only if we are sure human is there
				if (mDurationDetected > 6F)
					mActionTrigger = mActionManager.DesiredAction(COMPANION_STATE.USER_DETECTED);

				// if not sure human there or no action desired, play BML
				if (mActionTrigger == "IDLE" || string.IsNullOrEmpty(mActionTrigger)) {
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

			// if no one there, do whatever Buddy wants
			else if (mTimeHumanLastDetected > 10F)
				Trigger(mActionManager.DesiredAction(COMPANION_STATE.IDLE));



			// 0) If trigger vocal or kidnapping or low battery... go to corresponding state
			if (string.IsNullOrEmpty(mActionTrigger)) {
				if (mDetectionManager.mDetectedElement == Detected.THERMAL || mDetectionManager.mDetectedElement == Detected.HUMAN_RGB)
					mTimeHumanLastDetected = 0;
				else if (mDetectionManager.mDetectedElement != Detected.NONE) {
					mActionTrigger = mActionManager.LaunchReaction(COMPANION_STATE.USER_DETECTED, mDetectionManager.mDetectedElement);
					if (!string.IsNullOrEmpty(mActionTrigger))
						Trigger(mActionTrigger);
				}
			}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("User detected exit");
			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}
	}
}