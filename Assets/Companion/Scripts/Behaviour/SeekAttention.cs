using Buddy;
using Buddy.Features.Stimuli;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class SeekAttention : AStateMachineBehaviour
	{
		private float mTimeState;
		private float mTimeHumanDetected;
		private bool mVocalTriggered;
		private bool mNeedCharge;
		private bool mReallyNeedCharge;
		private bool mKidnapping;
		private bool mGrumpy;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mSensorManager = GetComponent<StimuliManager>();
			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mState.text = "Seek Attention";
			Debug.Log("state: Seek Attention" + BYOS.Instance.Battery.EnergyLevel);

			mTimeState = 0F;
			mTimeHumanDetected = 0F;
			mVocalTriggered = false;
			mReallyNeedCharge = false;
			mNeedCharge = false;
			mGrumpy = false;

			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);


			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();




			mTTS.Say("Helloooooo", true);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeHumanDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			if (mVocalTriggered) {
				iAnimator.SetTrigger("VOCALTRIGGERED");

			} else if (mKidnapping) {
				mMood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (mReallyNeedCharge) {
				iAnimator.SetTrigger("CHARGE");

				// 1) If no more human detected for a while, go back to IDLE or go to sad buddy
			} else if (mTimeHumanDetected > 59F) {
				iAnimator.SetTrigger("SADBUDDY");


				// 2) after a while
			} else if (mTimeState > 45F && !mGrumpy ) {
				mMood.Set(MoodType.GRUMPY);
				mGrumpy = true;
				mTTS.Say("Au cas où tu ne l'aurais pas remarqué, je cherche à attirer ton attention!", true);

			} else if (mTimeState > 60F) {
				iAnimator.SetTrigger("SADBUDDY");

				// 3) Otherwise, do crazy stuff
			} else {
				// TODO -> move / dance / make noise
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();

		}

		void OnSphinxActivation()
		{
			mVocalTriggered = true;
		}

		void OnVeryLowBattery()
		{
			mMood.Set(MoodType.TIRED);
			Debug.Log("SeekAttention really need charge"  +BYOS.Instance.Battery.EnergyLevel);
			mReallyNeedCharge = true;
		}

		void OnHumanDetected()
		{
			mTimeHumanDetected = 0F;
		}

		void OnKidnapping()
		{
			mKidnapping = true;
		}


	}
}