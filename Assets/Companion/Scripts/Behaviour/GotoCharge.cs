using Buddy;
using Buddy.Features.Stimuli;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{

	public class GotoCharge : AStateMachineBehaviour
	{
		private bool mSpeechTriggered;
		private bool mVeryLowBattery;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			//mSensorManager = BYOS.Instance.SensorManager;
			mSensorManager = GetComponent<StimuliManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mState.text = "Goto charge: " +	BYOS.Instance.Battery.EnergyLevel;
			Debug.Log("state: Goto charge");
			mSpeechTriggered = false;
			mVeryLowBattery = false;
			mMood.Set(MoodType.TIRED);
			mTTS.Say("Je vais me recharger", true);

			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();


		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if (mSpeechTriggered) {
				if (mVeryLowBattery) {
					mTTS.Say("Désolé, je suis trop fatigué, je dois aller me recharger", true);
					mSpeechTriggered = false;
                } else {
					iAnimator.SetTrigger("ASKCHARGE");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();
		}

		void OnSphinxActivation()
		{
			mSpeechTriggered = true;
		}

		void OnVeryLowBattery()
		{
			mVeryLowBattery = true;
		}
	}
}
