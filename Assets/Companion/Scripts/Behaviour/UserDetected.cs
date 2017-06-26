using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class UserDetected : AStateMachineBehaviour
	{
		private float mTimeState;
		private float mTimeHumanDetected;
		private bool mVocalTriggered;
		private bool mNeedCharge;
		private bool mReallyNeedCharge;
		private bool mKidnapping;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			Utils.LogI(LogContext.APP, "Start UserD");
			CompanionData.Instance.Bored = 0;
			//CommonIntegers["mood"] = (int)MoodType.NEUTRAL;
			CompanionData.Instance.MovingDesire = 0;
			CompanionData.Instance.InteractDesire = 0;
			CompanionData.Instance.ChargeAsked = false;
			mState = GetComponentInGameObject<Text>(0);
			Utils.LogI(LogContext.APP, "Start UserD");
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Utils.LogI(LogContext.APP, "Enter UserD 0");
			mState.text = "User Detected";

			Debug.Log("User Detected battery: " + BYOS.Instance.Primitive.Battery.EnergyLevel);
			mTimeState = 0F;
			mTimeHumanDetected = 0F;
			mVocalTriggered = false;
			mReallyNeedCharge = false;
			mNeedCharge = false;

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StartListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StartListenning();



			if (CompanionData.Instance.InteractDesire < 30) {
				// Todo: we don't want to interact but we will still show the human we noticed him:
				// => gaze toward position / react to screen touch...
			} else if (CompanionData.Instance.InteractDesire < 70) {
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
				Interaction.Mood.Set(MoodType.HAPPY);
				//mTTS.Say("Salut, salut!", true);
			} else {
				//TODO: propose game only if we are pretty sure someone is present
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
                Interaction.Mood.Set(MoodType.HAPPY);
                Interaction.TextToSpeech.Say("[200] Salut, salut!", true);
                Interaction.TextToSpeech.Say("J'ai très envie de jouer avec toi, on fait un petit jeu?", true);

			}

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeHumanDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			if (mVocalTriggered) {
				Debug.Log("VOCAL TRIGGERED");
				iAnimator.SetTrigger("VOCALTRIGGERED");

			} else if (mKidnapping) {
				iAnimator.SetTrigger("KIDNAPPING");

			} else if ((mNeedCharge && CompanionData.Instance.InteractDesire < 30) || mReallyNeedCharge) {
				Debug.Log("User Detected needcharge " + mNeedCharge + " really need charge " + mReallyNeedCharge);
				iAnimator.SetTrigger("CHARGE");

				// 1) If no more human detected for a while, go back to IDLE or go to sad buddy
			} else if (mTimeHumanDetected > 20F) {
				if (CompanionData.Instance.InteractDesire > 80)
					iAnimator.SetTrigger("SADBUDDY");
				else
					iAnimator.SetTrigger("IDLE");


				// 2) If human detected for a while and want to interact but no interaction, go to Crazy Buddy
			} else if (mTimeState > 45F && CompanionData.Instance.InteractDesire > 50) {
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
				Interaction.Face.SetEvent(FaceEvent.SMILE);
				iAnimator.SetTrigger("SEEKATTENTION");

				// 3) Otherwise, follow human head / body with head, eye or body
			} else if (mTimeState > 500F && CompanionData.Instance.MovingDesire > 30) {
                Interaction.Mood.Set(MoodType.SURPRISED);
				iAnimator.SetTrigger("WANDER");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.VERY_LOW_BATTERY].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.HUMAN_DETECTED].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.KIDNAPPING].StopListenning();
			//mSensorManager.mStimuliControllers[StimulusEvent.FACE_DETECTED].StopListenning();

		}

		void OnSphinxActivation()
		{
			mVocalTriggered = true;
		}

		void OnLowBattery()
		{
			Interaction.Mood.Set(MoodType.TIRED);
			mNeedCharge = true;

			Debug.Log("User Detected low battery: " + BYOS.Instance.Primitive.Battery.EnergyLevel);
		}

		void OnVeryLowBattery()
		{
            Interaction.Mood.Set(MoodType.TIRED);
			mReallyNeedCharge = true;
			
			Debug.Log("User Detected very low battery: " + BYOS.Instance.Primitive.Battery.EnergyLevel);
		}

		void OnHumanDetected()
		{
			//Debug.Log("Human is detected");
			mTimeHumanDetected = 0F;
		}

		void OnKidnapping()
		{
			mKidnapping = true;
		}
	}
}