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
			mSensorManager = GetComponent<StimuliManager>();
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
			mTTS.Say("Je cherche quelqu'un pour jouer avec moi!", true);
			mMood.Set(MoodType.THINKING);


			mSensorManager.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

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

			if (mTTS.HasFinishedTalking && !mWandering) {
				//mReaction.StartWandering();
				mWandering = true;
			}

			if (mFaceDetected) {

				// If Buddy sees a face and wants to interact
				if (mBatteryVeryLow) {
					mTTS.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					//mTTS.Say("Hey, voulez vous jouer avec moi?", true);
					BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
					mMood.Set(MoodType.HAPPY);
					iAnimator.SetTrigger("PROPOSEGAME");
				}
			} else if (mHumanDetected) {
				BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				mMood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("INTERACT");

			} else if (mKidnapping) {
				BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				mMood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (mVocalTrigger) {
				// If Buddy is vocally triggered
				if (mBatteryVeryLow) {
					mTTS.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
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
			mSensorManager.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);


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
			mMood.Set(MoodType.TIRED);
			mBatteryLow = true;
		}

		void OnVeryLowBattery()
		{
			mMood.Set(MoodType.TIRED);
			mBatteryVeryLow = true;
		}

		void OnSphinxActivation()
		{
			mVocalTrigger = true;
		}


	}
}