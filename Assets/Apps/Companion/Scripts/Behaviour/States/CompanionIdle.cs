using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using UnityEngine.UI;

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
		private float mLastBMLTime;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			CompanionData.Instance.InteractDesire = 60;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE";
			Debug.Log("state: IDLE");

			CompanionData.Instance.Bored = 0;

			Interaction.Mood.Set(MoodType.NEUTRAL);

			mLastBMLTime = 0F;
			mTimeIdle = 0F;
			mPreviousTime = 0F;

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = true;
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


			if (mTimeIdle > 10F && BYOS.Instance.Interaction.BMLManager.ActiveBML.Count == 0) {
				// Play BML from time to time. Play more when bored
				if (mTimeIdle - mLastBMLTime > UnityEngine.Random.Range(100F - CompanionData.Instance.Bored, 200F)) {
					//BYOS.Instance.Interaction.BMLManager.LaunchByID("happy1");
				}
			} else if (mTimeIdle > 10F) {
				mLastBMLTime = mTimeIdle;
			}


			if (mTimeIdle > 3F) {
				// if no event and no BML and high desires
				if (BYOS.Instance.Interaction.BMLManager.ActiveBML.Count == 0 && (mTimeIdle > 5F) && mDetectionManager.mDetectedElement == Detected.NONE) {
					if (CompanionData.Instance.InteractDesire > 70) {
						Debug.Log("LOOKINGFOR");
						Trigger("LOOKINGFOR");

					} else if (CompanionData.Instance.MovingDesire > 70) {
						Debug.Log("WANDER");
						Trigger("WANDER");
					}
				}

				// Otherwise, react on almost all detectors
				switch (mDetectionManager.mDetectedElement) {
					case Detected.TRIGGER:
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.TOUCH:
						Trigger("ROBOTTOUCHED");
						break;

					case Detected.KIDNAPPING:
						Trigger("KIDNAPPING");
						break;

					case Detected.BATTERY:
						Trigger("CHARGE");
						break;

					case Detected.HUMAN_RGB & Detected.THERMAL:
						Trigger("INTERACT");
						break;

					default:
						break;
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeIdle = 0F;
			
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);

			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = false;

			mDetectionManager.mDetectedElement = Detected.NONE;
		}

		//////// CALLBACKS

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
				Interaction.Face.SetEvent(FaceEvent.YAWN);
			}
		}

	}
}