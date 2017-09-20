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
		//private bool mVocalTriggered;
		private bool mNeedCharge;
		private bool mReallyNeedCharge;
		private bool mKidnapping;
		private const float KIDNAPPING_THRESHOLD = 4.5F;
		private HumanRecognition mHumanReco;
		private KidnappingDetection mKidnappingDetection;

		//private EyesFollowThermal mEyesFollowThermal;

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
			//mEyesFollowThermal = GetComponent<EyesFollowThermal>();
			Utils.LogI(LogContext.APP, "Start UserD");
		}




		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Utils.LogI(LogContext.APP, "Enter UserD 0");
			mState.text = "User Detected" + Primitive.Battery.EnergyLevel;

			//mEyesFollowThermal.enabled = true;


			mTimeState = 0F;
			mTimeHumanDetected = 0F;
			//mVocalTriggered = false;
			mReallyNeedCharge = false;
			mNeedCharge = false;


			Interaction.SphinxTrigger.LaunchRecognition();

			mHumanReco = Perception.Human;
			mHumanReco.OnDetect(OnHumanDetected, BodyPart.FULL_BODY | BodyPart.FACE | BodyPart.LOWER_BODY | BodyPart.UPPER_BODY);


			mKidnappingDetection = Perception.Kidnapping;
			mKidnappingDetection.OnDetect(OnKidnapping, KIDNAPPING_THRESHOLD);



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

			Utils.LogI(LogContext.APP, "Enter UserD 0");

		}

		

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "User Detected nrj " + BYOS.Instance.Primitive.Battery.EnergyLevel + " needcharge: " + mNeedCharge + " " + mReallyNeedCharge;

			if(Primitive.Battery.EnergyLevel < 5) {
				mReallyNeedCharge = true;
            } else if (Primitive.Battery.EnergyLevel < 15) {
				mNeedCharge = true;
            }

			mTimeHumanDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			if (Interaction.SphinxTrigger.HasTriggered) {
				Debug.Log("VOCAL TRIGGERED");
				Trigger("VOCALTRIGGERED");
			}else if (Input.touchCount > 0  || Input.GetMouseButtonDown(0)) {
				// Add to 1st part of if? 
				// && Input.GetTouch(0).phase == TouchPhase.Moved

				// Screen touched
				Debug.Log("ROBOTTOUCHED");
				Trigger("ROBOTTOUCHED");
			} else if (mKidnapping) {
				Trigger("KIDNAPPING");

			} else if ((mNeedCharge && CompanionData.Instance.InteractDesire < 30) || mReallyNeedCharge) {
				Debug.Log("User Detected needcharge " + mNeedCharge + " really need charge " + mReallyNeedCharge);
				Trigger("CHARGE");

				// 1) If no more human detected for a while, go back to IDLE or go to sad buddy
			} else if (mTimeHumanDetected > 20F) {
				if (CompanionData.Instance.InteractDesire > 80)
					Trigger("SADBUDDY");
				else
					Trigger("IDLE");


				// 2) If human detected for a while and want to interact but no interaction, go to Crazy Buddy
			} else if (mTimeState > 45F && CompanionData.Instance.InteractDesire > 50) {
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
				Interaction.Face.SetEvent(FaceEvent.SMILE);
				Trigger("SEEKATTENTION");

				// 3) Otherwise, follow human head / body with head, eye or body
			} else if (mTimeState > 500F && CompanionData.Instance.MovingDesire > 30) {
                Interaction.Mood.Set(MoodType.SURPRISED);
				Trigger("WANDER");
			}
		}



		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Interaction.SphinxTrigger.StopRecognition();
			mHumanReco.StopAllOnDetect();
			mKidnappingDetection.StopAllOnDetect();


		}
		
		//////// CALLBACKS

		private bool OnHumanDetected(HumanEntity[] obj)
		{
			//Debug.Log("Human is detected");
			mTimeHumanDetected = 0F;
			return true;
		}

		private bool OnKidnapping()
		{
			mKidnapping = true;
			return true;
		}
	}
}