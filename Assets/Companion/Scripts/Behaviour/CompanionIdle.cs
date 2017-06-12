using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using UnityEngine.UI;
using Buddy.Features.Stimuli;

namespace BuddyApp.Companion
{

	/// <summary>
	/// This class is used when the robot is in default mode
	/// It will then go wander, interact, look for someone or charge according to the stimuli
	/// </summary>
	public class CompanionIdle : AStateMachineBehaviour
	{
		private float mTimeIdle;
		private float mPreviousTime;
		private bool mVocalTrigger;
		private bool mBatteryLow;
		private bool mBatteryVeryLow;
		private bool mFaceDetected;
		private bool mHumanDetected;
		private bool mBuddyMotion;
		private bool mKidnapping;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mSensorManager = GetComponent<StimuliManager>();
			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE";
			Debug.Log("state: IDLE");

			mVocalTrigger = false;
			mBatteryLow = false;
			mBatteryVeryLow = false;
			mFaceDetected = false;
			mKidnapping = false;
			mBuddyMotion = false;
			mHumanDetected = false;

			CompanionData.Instance.Bored = 0;
			CompanionData.Instance.InteractDesire = 60;

			mMood.Set(MoodType.NEUTRAL);

			mTimeIdle = 0F;
			mPreviousTime = 0F;
			mSensorManager.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);

			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.POSITION_UPDATE].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();


			//BYOS.Instance.Speaker.Voice.Play(VoiceSound.YAWN);
		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE \n bored: " + CompanionData.Instance.Bored + "\n interactDesire: " + CompanionData.Instance.InteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.MovingDesire;
			mTimeIdle += Time.deltaTime;

			// Do the following every second
			if (mTimeIdle - mPreviousTime > 1F) {
				int lRand = UnityEngine.Random.Range(0, 100);

				if (lRand < (int)mTimeIdle / 10) {
					CompanionData.Instance.Bored += 1;
				}
				mPreviousTime = mTimeIdle;

			}



			if (mFaceDetected && (mTimeIdle > 5F)) {
				Debug.Log("IDLE facedetected + time 5");
				// If Buddy sees a face and doesn't want to interact
				if (CompanionData.Instance.InteractDesire < 50 && mBatteryLow) {
					//mTTS.Say("Salut mon ami! [500] Je suis fatigué, puis-je aller me reposer?", true);
					iAnimator.SetTrigger("ASKCHARGE");

					//If its battery is too low
				} else if (mBatteryVeryLow) {
					mTTS.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");

					//Otherwise
				} else {
					//mTTS.Say("Hey, voulez vous jouer avec moi?", true);
					//mMood.Set(MoodType.HAPPY);
					mMood.Set(MoodType.HAPPY);
					iAnimator.SetTrigger("INTERACT");
				}

			} else if (mHumanDetected && (mTimeIdle > 5F)) {
				Debug.Log("HUMANDETECTED");
				mMood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("INTERACT");

			} else if (mBuddyMotion) {
				//if (CompanionData.Instance.InteractDesire < 10) {
				//	mMood.Set(MoodType.ANGRY);
				//	mTTS.Say("Pourquoi me pousses-tu? Grrr", true);
				//} else if (CompanionData.Instance.InteractDesire < 50) {
				//	mMood.Set(MoodType.GRUMPY);
				//	mTTS.Say("Que fais-tu?", true);
				//} else if (CompanionData.Instance.InteractDesire > 70) {
				//	BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				//	mMood.Set(MoodType.HAPPY);
				//	mTTS.Say("Que fais-tu?", true);
				//}

				Debug.Log("BuddyMotion");

				//iAnimator.SetTrigger("ROBOTTOUCHED");

			} else if (mKidnapping) {
				Debug.Log("KIDNAPPING");
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
				mTTS.Say("Je commence à fatigué, je vais faire une petite sièste!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mBatteryVeryLow) {
				//TODO put in dictionary
				mTTS.Say("Je suis très fatigué, je vais me coucher! [800] Bonne nuit tout le monde!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if ((CompanionData.Instance.InteractDesire > 70) && (mTimeIdle > 5F)) {
				Debug.Log("LOOKINGFOR");
				iAnimator.SetTrigger("LOOKINGFOR");

			} else if ((CompanionData.Instance.MovingDesire > 70) && (mTimeIdle > 5F)) {
				Debug.Log("WANDER");
				iAnimator.SetTrigger("WANDER");
			}

		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeIdle = 0F;
			mSensorManager.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);
			mSensorManager.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);


			//mSensorManager.mStimuliControllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.POSITION_UPDATE].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();

		}
		


		void OnRandomMinuteActivation()
		{
			CompanionData.Instance.Bored += 5;
		}

		void OnMinuteActivation()
		{
			int lRand = UnityEngine.Random.Range(0, 100);

			if (lRand < CompanionData.Instance.Bored) {
				CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored / 10;
				CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored / 10;
				mFace.SetEvent(FaceEvent.YAWN);
			}
		}

		void OnKidnapping()
		{
			mKidnapping = true;
		}

		void OnPositionUpdate()
		{
			mBuddyMotion = true;
		}

		void OnHumanDetected()
		{
			mHumanDetected = true;
		}

		void OnFaceDetected()
		{
			Debug.Log("IDLE: user detected");
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