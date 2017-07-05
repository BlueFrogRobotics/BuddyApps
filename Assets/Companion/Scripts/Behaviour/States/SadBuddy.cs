using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class SadBuddy : AStateMachineBehaviour
	{

		private float mSadTime;

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

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Sad Buddy";
			Debug.Log("state: Sad Buddy");

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

			mSadTime = 0F;
			Interaction.TextToSpeech.Say("Personne ne veut jouer avec moi!", true);
            Interaction.Mood.Set(MoodType.SAD);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.LOW_BATTERY].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Enable();
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSadTime = Time.deltaTime;
			if (mVocalTrigger) {
				// If Buddy is vocally triggered
				if (mBatteryVeryLow) {
					Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					iAnimator.SetTrigger("VOCALTRIGGERED");
				}

			} else if (mHumanDetected) {
				Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				Interaction.Mood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("PROPOSEGAME");

			} else if (mKidnapping) {
				Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				Interaction.Mood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (mFaceDetected) {

				// If Buddy sees a face and wants to interact
				if (mBatteryVeryLow) {
                    Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					//mTTS.Say("Hey, voulez vous jouer avec moi?", true);
					Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
                    Interaction.Mood.Set(MoodType.HAPPY);
					iAnimator.SetTrigger("INTERACT");
				}

			} else if (mBatteryLow) {
				//TODO put in dictionary
				//mTTS.Say("Je commence à fatigué, je vais faire une petite sièste!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mBatteryVeryLow) {
				//TODO put in dictionary
				//mTTS.Say("Je suis très fatigué, je vais me coucher! [800] Bonne nuit tout le monde!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mSadTime > 300F) {
				iAnimator.SetTrigger("SEEKATTENTION");
			} else {
				// Do sad stuff: move around slowly
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


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.LOW_BATTERY].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Disable();
		}

		void OnRandomMinuteActivation()
		{
			Interaction.TextToSpeech.Say("Quelqu'un veut faire un jeu?", true);
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
            Interaction.Mood.Set(MoodType.HAPPY);
			mVocalTrigger = true;
		}


	}
}