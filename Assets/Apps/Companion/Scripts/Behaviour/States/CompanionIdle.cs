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
		private bool mBatteryLow;
		private bool mBatteryVeryLow;
		private bool mFaceDetected;
		private bool mHumanDetected;
		private bool mBuddyMotion;
		private bool mKidnapping;
		private HumanRecognition mHumanReco;
		private KidnappingDetection mKidnappingDetection;

		private const float KIDNAPPING_THRESHOLD = 4.5F;

		//private EyesFollowThermal mEyesFollowThermal;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			//mEyesFollowThermal = GetComponent<EyesFollowThermal>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE";
			Debug.Log("state: IDLE");

			//mEyesFollowThermal.enabled = true;

			mBatteryLow = false;
			mBatteryVeryLow = false;
			mFaceDetected = false;
			mKidnapping = false;
			mBuddyMotion = false;
			mHumanDetected = false;

			CompanionData.Instance.Bored = 0;
			CompanionData.Instance.InteractDesire = 60;

			Interaction.Mood.Set(MoodType.NEUTRAL);


			mTimeIdle = 0F;
			mPreviousTime = 0F;


			Interaction.SphinxTrigger.LaunchRecognition();

			mHumanReco = Perception.Human;
			mHumanReco.OnDetect(OnHumanDetected, BodyPart.FULL_BODY | BodyPart.FACE | BodyPart.LOWER_BODY | BodyPart.UPPER_BODY);


			mKidnappingDetection = Perception.Kidnapping;
			mKidnappingDetection.OnDetect(OnKidnapping, KIDNAPPING_THRESHOLD);

			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = true;
			Perception.Stimuli.Controllers[StimulusEvent.POSITION_UPDATE].enabled = true;


			//BYOS.Instance.Speaker.Voice.Play(VoiceSound.YAWN);
		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE \n bored: " + CompanionData.Instance.Bored + "\n interactDesire: " + CompanionData.Instance.InteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.MovingDesire;
			mTimeIdle += Time.deltaTime;


			if (Primitive.Battery.EnergyLevel < 5) {
				mBatteryVeryLow = true;
			} else if (Primitive.Battery.EnergyLevel < 15) {
				mBatteryLow = true;
			}


			// Do the following every second
			if (mTimeIdle - mPreviousTime > 1F) {
				int lRand = UnityEngine.Random.Range(0, 100);

				if (lRand < (int)mTimeIdle / 10) {
					CompanionData.Instance.Bored += 1;
				}
				mPreviousTime = mTimeIdle;

			}

			if(mTimeIdle > 3F)
			if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0))) {
				// Add to 1st part of if? 
				// && Input.GetTouch(0).phase == TouchPhase.Moved

				// Screen touched
				Debug.Log("ROBOTTOUCHED");
				Trigger("ROBOTTOUCHED");
			} else if (mFaceDetected) {
				Debug.Log("IDLE facedetected + time 5");
				// If Buddy sees a face and doesn't want to interact
				if (CompanionData.Instance.InteractDesire < 50 && mBatteryLow) {
					//Interaction.TextToSpeech.Say("Salut mon ami! [500] Je suis fatigué, puis-je aller me reposer?", true);
					iAnimator.SetTrigger("ASKCHARGE");

					//If its battery is too low
				} else if (mBatteryVeryLow) {
					Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");

					//Otherwise
				} else {
					//Interaction.TextToSpeech.Say("Hey, voulez vous jouer avec moi?", true);
					//Interaction.Mood.Set(MoodType.HAPPY);
					Interaction.Mood.Set(MoodType.HAPPY);
					iAnimator.SetTrigger("INTERACT");
				}

			} else if (mHumanDetected) {
				Debug.Log("HUMANDETECTED");
				Interaction.Mood.Set(MoodType.HAPPY);
				iAnimator.SetTrigger("INTERACT");

			} else if (mBuddyMotion) {
				//if (CompanionData.Instance.InteractDesire < 10) {
				//	Interaction.Mood.Set(MoodType.ANGRY);
				//	Interaction.TextToSpeech.Say("Pourquoi me pousses-tu? Grrr", true);
				//} else if (CompanionData.Instance.InteractDesire < 50) {
				//	Interaction.Mood.Set(MoodType.GRUMPY);
				//	Interaction.TextToSpeech.Say("Que fais-tu?", true);
				//} else if (CompanionData.Instance.InteractDesire > 70) {
				//	BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				//	Interaction.Mood.Set(MoodType.HAPPY);
				//	Interaction.TextToSpeech.Say("Que fais-tu?", true);
				//}

				Debug.Log("BuddyMotion");

				//iAnimator.SetTrigger("ROBOTTOUCHED");

			} else if (mKidnapping) {
				Debug.Log("KIDNAPPING");
				iAnimator.SetTrigger("KIDNAPPING");

			} else if (Interaction.SphinxTrigger.HasTriggered) {
				// If Buddy is vocally triggered
				if (mBatteryVeryLow) {
					Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
					iAnimator.SetTrigger("CHARGE");
				} else {
					iAnimator.SetTrigger("VOCALTRIGGERED");
				}

			} else if (mBatteryLow) {
				//TODO put in dictionary
				Interaction.TextToSpeech.Say("Je commence à fatigué, je vais faire une petite sièste!", true);
				iAnimator.SetTrigger("CHARGE");

			} else if (mBatteryVeryLow) {
				//TODO put in dictionary
				Interaction.TextToSpeech.Say("Je suis très fatigué, je vais me coucher! [800] Bonne nuit tout le monde!", true);
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


			Interaction.SphinxTrigger.StopRecognition();
			mHumanReco.StopAllOnDetect();
			mKidnappingDetection.StopAllOnDetect();


			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);



			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = false;
			Perception.Stimuli.Controllers[StimulusEvent.POSITION_UPDATE].enabled = false;

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

		private bool OnKidnapping()
		{
			mKidnapping = true;
			return true;
		}

		private void OnPositionUpdate()
		{
			mBuddyMotion = true;
		}


		private bool OnHumanDetected(HumanEntity[] obj)
		{
			mHumanDetected = true;
			return true;
		}

	}
}