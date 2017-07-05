using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class BuddyInArms : AStateMachineBehaviour
	{
		private float mTimeInArmns;
		private bool mVocalTriggered;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			//mSensorManager = BYOS.Instance.SensorManager;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Buddy in Arms";
			Debug.Log("state: Buddy in Arms");
			mVocalTriggered = false;
			mTimeInArmns = 0F;
			if (CompanionData.Instance.InteractDesire > 40) {
				Interaction.TextToSpeech.Say("J'aime être dans tes bras!", true);
				CompanionData.Instance.InteractDesire -= 10;
                Interaction.Mood.Set(MoodType.HAPPY);
			} else {
                Interaction.TextToSpeech.Say("Laisse moi rouler!!!", true);
				CompanionData.Instance.InteractDesire -= 20;
                Interaction.Mood.Set(MoodType.GRUMPY);
			}


			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);

			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Enable();


		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeInArmns += Time.deltaTime;

			if (mVocalTriggered)
                Interaction.TextToSpeech.Say("Que puis-je pour toi?", true);
			iAnimator.SetTrigger("VOCALTRIGGERED");
			if (mTimeInArmns > 5F)
				iAnimator.SetTrigger("INTERACT");
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Disable();
		}

		void OnSphinxActivation()
		{
			mVocalTriggered = true;
		}
	}
}