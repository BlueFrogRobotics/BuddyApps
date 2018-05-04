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
		private float mTimeIdle;

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
			mActionManager.CurrentAction = BUDDY_ACTION.EXPRESS_MOOD;
			mState.text = "EXPRESS MOOD";
			Debug.Log("state: EXRESSMOOD nrj:" + Interaction.InternalState.Energy + " positivity: " + Interaction.InternalState.Positivity);

			mTimeState = 0F;
			mTimeHumanDetected = 0F;
			mTimeIdle = 0F;

			mActionManager.LastMoodExpression = Time.time;

			
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeState += Time.deltaTime;


			if (BYOS.Instance.Interaction.BMLManager.DonePlaying) {

				mTimeIdle += Time.deltaTime;


				// Play BML after 4 seconds every 8 seconds or launch desired action
				if (((int)mTimeIdle) % 8 == 4) {
					mActionTrigger = mActionManager.DesiredAction(COMPANION_STATE.EXPRESS_MOOD);
					if (string.IsNullOrEmpty(mActionTrigger) && mTimeState > 15F) {
						//if no desired action, play BML
						Debug.Log("Play mood BML with category: " + Interaction.InternalState.InternalStateMood.ToString());
						BYOS.Instance.Interaction.BMLManager.LaunchRandom(Interaction.InternalState.InternalStateMood.ToString());
					} else {
						// Otherwise trigger to perform the action
						Trigger(mActionTrigger);
					}
				}

			}

			// Otherwise, react on all detectors
			if (string.IsNullOrEmpty(mActionTrigger))
				if (mDetectionManager.mDetectedElement != Detected.NONE)
					Trigger(mActionManager.LaunchReaction(COMPANION_STATE.EXPRESS_MOOD, mDetectionManager.mDetectedElement));
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}


	}
}