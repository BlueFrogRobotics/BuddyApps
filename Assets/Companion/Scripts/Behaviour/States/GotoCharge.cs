using Buddy;
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
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            mState.text = "Goto charge: " +	BYOS.Instance.Primitive.Battery.EnergyLevel;
			Debug.Log("state: Goto charge");
			mSpeechTriggered = false;
			mVeryLowBattery = false;
			Interaction.Mood.Set(MoodType.TIRED);
            Interaction.TextToSpeech.Say("Je vais me recharger", true);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
			
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Enable();


		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if (mSpeechTriggered) {
				if (mVeryLowBattery) {
					Interaction.TextToSpeech.Say("Désolé, je suis trop fatigué, je dois aller me recharger", true);
					mSpeechTriggered = false;
                } else {
					iAnimator.SetTrigger("ASKCHARGE");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Disable();
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
