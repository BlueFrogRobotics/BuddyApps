using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class LookingForSomeone : AStateMachineBehaviour
	{

		private float mLookingTime;

		private bool mLookForSomeone;
		private bool mWander;
		private bool mVocalTrigger;
		private bool mBatteryLow;
		private bool mBatteryVeryLow;
		private bool mFaceDetected;
		private bool mHumanDetected;
		private bool mBuddyMotion;
		private bool mKidnapping;
		private bool mWandering;

		//private Reaction mReaction;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mState = GetComponentInGameObject<Text>(0);
			//mReaction = GetComponent<Reaction>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Looking for someone";
			Debug.Log("state: Looking 4 someone");
			mLookForSomeone = false;
			mWander = false;
			mVocalTrigger = false;
			mBatteryLow = false;
			mBatteryVeryLow = false;
			mFaceDetected = false;
			mKidnapping = false;
			mBuddyMotion = false;
			mHumanDetected = false;
			mWandering = false;

			mLookingTime = 0F;
			Interaction.TextToSpeech.Say("Je cherche quelqu'un pour jouer avec moi!", true);
            Interaction.Mood.Set(MoodType.THINKING);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;

			if (Interaction.TextToSpeech.HasFinishedTalking && !mWandering) {
				//mReaction.StartWandering();
				mWandering = true;
			}

			if (mFaceDetected) {

				// If Buddy sees a face and wants to interact
				if (mBatteryVeryLow) {
                    Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					//mTTS.Say("Hey, voulez vous jouer avec moi?", true);
					Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
					Interaction.Mood.Set(MoodType.HAPPY);
					iAnimator.SetTrigger("PROPOSEGAME");
				}
			} else if (mHumanDetected) {
				Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
                Interaction.Mood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("INTERACT");

			} else if (mKidnapping) {
				Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				Interaction.Mood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (mVocalTrigger) {
				// If Buddy is vocally triggered
				if (mBatteryVeryLow) {
					Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					iAnimator.SetTrigger("VOCALTRIGGERED");
				}

			} else if (mBatteryLow) {
				//TODO put in dictionary
				//mTTS.Say("Je commence à fatigué, je vais faire une petite sièste!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mBatteryVeryLow) {
				//TODO put in dictionary
				//mTTS.Say("Je suis très fatigué, je vais me coucher! [800] Bonne nuit tout le monde!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mLookingTime > 300) {
				// We didn't find anybody, buddy is sad and wants some attention :/
				if (CompanionData.Instance.InteractDesire > 80) {
					iAnimator.SetTrigger("IDLE");
				} else {
					iAnimator.SetTrigger("SADBUDDY");
					CompanionData.Instance.InteractDesire = 100;
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);

			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();

			//mReaction.StopWandering();
		}


		void OnRandomMinuteActivation()
		{
			// Say something?
		}

		void OnKidnapping()
		{
			mKidnapping = true;
		}

		void OnHumanDetected()
		{
			mHumanDetected = true;
		}

		void OnFaceDetected()
		{
			mFaceDetected = true;
		}

		void OnLowBattery()
		{
			Interaction.Mood.Set(MoodType.TIRED);
			mBatteryLow = true;
		}

		void OnVeryLowBattery()
		{
            Interaction.Mood.Set(MoodType.TIRED);
			mBatteryVeryLow = true;
		}

		void OnSphinxActivation()
		{
			mVocalTrigger = true;
		}
	}
}