using Buddy;
using Buddy.Features.Stimuli;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{

	//[RequireComponent(typeof(Reaction))]
	public class CompanionWander : AStateMachineBehaviour
	{
		private bool mVocalTriggered;
		private bool mNeedCharge;
		private bool mHumanDetected;
		private bool mKidnapping;

		//private Reaction mReaction;
		private bool mWandering;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mSensorManager = GetComponent<StimuliManager>();
			mState = GetComponentInGameObject<Text>(0);
			//mReaction = GetComponent<Reaction>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Wander";
			Debug.Log("state: Wander");
			mVocalTriggered = false;
			mNeedCharge = false;
			mHumanDetected = false;
			mKidnapping = false;
			mTTS.Say("Je m'ennuye un peu, je vais me balader", true);

			Debug.Log("wander: " + CompanionData.Instance.MovingDesire);

			mSensorManager.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnLowBattery);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);


			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mTTS.HasFinishedTalking && !mWandering) {
				Debug.Log("CompanionWander start wandering");
				//mReaction.StartWandering();
				mWandering = true;
			}

			if (mVocalTriggered) {
				iAnimator.SetTrigger("VOCALTRIGGERED");
			} else if (mNeedCharge) {
				iAnimator.SetTrigger("CHARGE");
			} else if (mHumanDetected) {
				if (CompanionData.Instance.InteractDesire > 50 && CompanionData.Instance.MovingDesire < 70) {
					iAnimator.SetTrigger("INTERACT");
				}
			} else if (mKidnapping) {
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (CompanionData.Instance.MovingDesire < 5) {
				Debug.Log("wander -> IDLE: " + CompanionData.Instance.MovingDesire);
				iAnimator.SetTrigger("IDLE");
			} else if (CompanionData.Instance.InteractDesire > 80 && CompanionData.Instance.MovingDesire > 20) {
				iAnimator.SetTrigger("LOOKINGFOR");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSensorManager.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();


			//mReaction.StopWandering();
			mWandering = false;
		}



		void OnRandomMinuteActivation()
		{
			// Buddy wants more and more to interact
			CompanionData.Instance.InteractDesire += 1;

			// Buddy is moving around, so he wants less and less to move around
			CompanionData.Instance.MovingDesire -= 2;
		}

		void OnSphinxActivation()
		{
			mVocalTriggered = true;
		}

		void OnLowBattery()
		{
			mNeedCharge = true;
		}

		void OnHumanDetected()
		{
			mHumanDetected = true;
		}

		void OnKidnapping()
		{
			mKidnapping = true;
		}

	}
}