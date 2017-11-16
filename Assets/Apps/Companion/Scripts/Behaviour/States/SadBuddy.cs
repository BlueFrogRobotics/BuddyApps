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

		private bool mLookForSomeone;
		private bool mWander;
		private bool mWandering;

		//private Reaction mReaction;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "SadBuddy";
			Debug.Log("state: SadBuddy");
			mLookForSomeone = false;
			mWander = false;
			mWandering = false;

			mLookingTime = 0F;
			Interaction.TextToSpeech.Say("nooneplay", true);
			Interaction.Mood.Set(MoodType.SAD);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mLookingTime = Time.deltaTime;


			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.Wandering) {
				Debug.Log("CompanionLooking4 start wandering");
				mActionManager.StartWander();
			}

			if (mLookingTime > 3000)
				//TODO: after wometime, do something? Activarte some BML...



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

					case Detected.HUMAN_RGB & Detected.THERMAL:
						Interaction.Mood.Set(MoodType.HAPPY);
						Trigger("INTERACT");
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
			Interaction.TextToSpeech.Say("nooneplay", true);
		}

	}
}