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

		private float mLookingTime;
		private float mTimeThermal;

		//private Reaction mReaction;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "SadBuddy";
			Debug.Log("state: SadBuddy");

			mLookingTime = 0F;
			Interaction.TextToSpeech.SayKey("nooneplay", true);
			Interaction.Mood.Set(MoodType.SAD);
			mTimeThermal = 0F;
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;


			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering && (mActionManager.ThermalFollow && mTimeThermal - Time.time > 5F)) {
				Debug.Log("sad buddy start wandering");
				mActionManager.StartWander(MoodType.SAD);
			}

			//if (mLookingTime > 3000)
			//TODO: after sometime, do something? Activarte some BML...



			switch (mDetectionManager.mDetectedElement) {
				case Detected.TRIGGER & Detected.TOUCH:
					Interaction.Mood.Set(MoodType.HAPPY);
					Trigger("PROPOSEGAME");
					break;

				case Detected.KIDNAPPING:
					Interaction.Mood.Set(MoodType.HAPPY);
					Trigger("KIDNAPPING");
					break;

				case Detected.BATTERY:
					Trigger("CHARGE");
					break;

				case Detected.THERMAL:
					mDetectionManager.mDetectedElement = Detected.NONE;
					mTimeThermal = Time.time;
					Debug.Log("sad buddy start follow");
					mActionManager.StartThermalFollow();
					break;

				case Detected.HUMAN_RGB:
					mDetectionManager.mDetectedElement = Detected.NONE;
					break;


				default:
					break;
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			mDetectionManager.mDetectedElement = Detected.NONE;
		}


		void OnRandomMinuteActivation()
		{
			// Say something?
			Interaction.TextToSpeech.SayKey("nooneplay", true);
		}

	}
}