using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class ExpressMood : AStateMachineBehaviour
	{
		private float mTimeState;
		private float mTimeHumanDetected;
		private bool mVocalTriggered;
		private bool mReallyNeedCharge;
		private bool mKidnapping;
		private bool mGrumpy;
		private float mHumanDetectionCounter;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.INFORM_MOOD;
			mState.text = "Seek Attention";
			Debug.Log("state: Seek Attention" + BYOS.Instance.Primitive.Battery.EnergyLevel);

			mTimeState = 0F;
			mTimeHumanDetected = 0F;
			mGrumpy = false;

			Interaction.TextToSpeech.Say("Helloooooo", true);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeState += Time.deltaTime;

			// 1) If no more human detected for a while, go back to IDLE or go to sad buddy
			if (mHumanDetectionCounter > 10) {
				iAnimator.SetTrigger("SADBUDDY");


				// 2) after a while
			} else if (mTimeState > 45F && !mGrumpy) {
				Interaction.Mood.Set(MoodType.GRUMPY);
				mGrumpy = true;
				Interaction.TextToSpeech.SayKey("catchattention", true);

			} else if (mTimeState > 120F) {
				iAnimator.SetTrigger("SADBUDDY");

				// 3) Otherwise, do crazy stuff
			} else {
				// TODO -> move / dance / make noise

				switch (mDetectionManager.mDetectedElement) {
					case Detected.TRIGGER:
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.TOUCH:
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.KIDNAPPING:
						Trigger("KIDNAPPING");
						break;

					case Detected.BATTERY:
						Trigger("CHARGE");
						break;
						
					case Detected.HUMAN_RGB & Detected.THERMAL:
						// TODO: check false positive level
						if (CompanionData.Instance.mInteractDesire > CompanionData.Instance.mMovingDesire) {
							if(Time.time - mTimeHumanDetected > 3F) {
								mHumanDetectionCounter++;
								mTimeHumanDetected = Time.time;
							}
						}
						break;

					default:
						break;
				}

			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}


	}
}