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

		public override void Start()
		{
			Utils.LogI(LogContext.APP, "Start UserD");
			CompanionData.Instance.Bored = 0;
			CompanionData.Instance.MovingDesire = 0;
			CompanionData.Instance.InteractDesire = 0;
			CompanionData.Instance.ChargeAsked = false;
			mState = GetComponentInGameObject<Text>(0);
			//Interaction.BMLManager.LoadAppBML();
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			Utils.LogI(LogContext.APP, "Start UserD");

		}




		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			Utils.LogI(LogContext.APP, "Enter UserD 0");
			mState.text = "User Detected";

			mTimeState = 0F;
			mTimeHumanDetected = 0F;



			if (CompanionData.Instance.InteractDesire < 30) {
				// Todo: we don't want to interact but we will still show the human we noticed him:
				// => gaze toward position ...
			} else if (CompanionData.Instance.InteractDesire < 70) {
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
				Interaction.Mood.Set(MoodType.HAPPY);
				//mTTS.Say("Salut, salut!", true);
			} else {
				//TODO: propose activity only if we are pretty sure someone is present
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				Interaction.Mood.Set(MoodType.HAPPY);
			}

			Utils.LogI(LogContext.APP, "Enter UserD 0");

		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "User Detected move: " + !BYOS.Instance.Primitive.Motors.Wheels.Locked;

			if (BYOS.Instance.Interaction.BMLManager.DonePlaying && !mActionManager.ThermalFollow && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				mActionManager.StartThermalFollow(HumanFollowType.ROTATION_AND_HEAD);
			}

			mTimeHumanDetected += Time.deltaTime;
			mTimeState += Time.deltaTime;

			// If human not there anymore
			if (mTimeHumanDetected > 8F) {
				if (CompanionData.Instance.InteractDesire > 80)
					Trigger("SADBUDDY");
				else if (CompanionData.Instance.InteractDesire > 50 && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
					Trigger("LOOKINGFOR");
				else if (CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
					if (CompanionData.Instance.MovingDesire < 30)
						CompanionData.Instance.MovingDesire += 30;
					Trigger("WANDER");
				} else
					Trigger("IDLE");


				// 2) If human detected for a while and want to interact but no interaction, go to Crazy Buddy
			} else if (mTimeState > 15F && mDetectionManager.TimeLastTouch > 5F && CompanionData.Instance.InteractDesire > 50) {
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
				Interaction.Face.SetEvent(FaceEvent.SMILE);
				Trigger("SEEKATTENTION");

				// 3) Otherwise, go wander
			} else if (mTimeState > 20F && mDetectionManager.TimeLastTouch > 5F &&  CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				if (CompanionData.Instance.MovingDesire < 30)
					CompanionData.Instance.MovingDesire += 30;
				Interaction.Mood.Set(MoodType.SURPRISED);
				Trigger("WANDER");
			} else {



				// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
				switch (mDetectionManager.mDetectedElement) {
					case Detected.TRIGGER:
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.TOUCH:
						Debug.Log("User Detected robot touched");
						Trigger("ROBOTTOUCHED");
						break;

					case Detected.KIDNAPPING:
						Trigger("KIDNAPPING");
						break;

					case Detected.BATTERY:
						Trigger("CHARGE");
						break;

					case Detected.THERMAL:
						mTimeHumanDetected = 0F;
						mDetectionManager.mDetectedElement = Detected.NONE;
						break;

					default:
						mDetectionManager.mDetectedElement = Detected.NONE;
						break;
				}
			}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("User detected exit");
			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}
	}
}