using Buddy;
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
			//Perception.Stimuli = BYOS.Instance.SensorManager;
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
			Interaction.TextToSpeech.Say("Je m'ennuye un peu, je vais me balader", true);

			Debug.Log("wander: " + CompanionData.Instance.MovingDesire);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnLowBattery);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);


			//Perception.Stimuli.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.LOW_BATTERY].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking && !mWandering) {
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
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnLowBattery);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			//Perception.Stimuli.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.LOW_BATTERY].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//Perception.Stimuli.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();


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