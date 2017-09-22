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
		private bool mNeedCharge;
		private bool mHumanDetected;
		private bool mKidnapping;

		//private Reaction mReaction;
		private bool mWandering;
		//private HumanRecognition mHumanReco;
		private KidnappingDetection mKidnappingDetection;

		private const float KIDNAPPING_THRESHOLD = 4.5F;

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
			mNeedCharge = false;
			mHumanDetected = false;
			mKidnapping = false;
			Interaction.TextToSpeech.Say("Je m'ennuye un peu, je vais me balader", true);

			Debug.Log("wander: " + CompanionData.Instance.MovingDesire);

			Interaction.SphinxTrigger.LaunchRecognition();

			//mHumanReco = Perception.Human;
			//mHumanReco.OnDetect(OnHumanDetected, BodyPart.FULL_BODY | BodyPart.FACE | BodyPart.LOWER_BODY | BodyPart.UPPER_BODY);


			mKidnappingDetection = Perception.Kidnapping;
			mKidnappingDetection.OnDetect(OnKidnapping, KIDNAPPING_THRESHOLD);


			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;


		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking && !mWandering) {
				Debug.Log("CompanionWander start wandering");
				//mReaction.StartWandering();
				mWandering = true;
			}

			if (Primitive.Battery.EnergyLevel < 15) {
				mNeedCharge = true;
			}

			if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0))) {
				// Add to 1st part of if? 
				// && Input.GetTouch(0).phase == TouchPhase.Moved

				// Screen touched
				Debug.Log("ROBOTTOUCHED");
				Trigger("ROBOTTOUCHED");
			} else if (Interaction.SphinxTrigger.HasTriggered) {
				// If Buddy is vocally triggered
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
			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;


			Interaction.SphinxTrigger.StopRecognition();
			//mHumanReco.StopAllOnDetect();
			mKidnappingDetection.StopAllOnDetect();

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

		private void OnLowBattery()
		{
			mNeedCharge = true;
		}

		private bool OnKidnapping()
		{
			mKidnapping = true;
			return true;
		}


		private bool OnHumanDetected(HumanEntity[] obj)
		{

			mHumanDetected = true;
			return true;
		}

	}
}