using Buddy;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class Follow : AStateMachineBehaviour
	{
		private float mTimeThermal;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mTimeThermal = Time.time;
			Debug.Log("state: follow");


			if (mDetectionManager.IsDetectingTrigger != CompanionData.Instance.CanTriggerWander)
				if (CompanionData.Instance.CanTriggerWander)
					mDetectionManager.StartSphinxTrigger();
				else
					mDetectionManager.StopSphinxTrigger();

		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mState.enabled)
				mState.text = "Follow last detect: " + (Time.time - mTimeThermal);

			if (mDetectionManager.IsDetectingTrigger != CompanionData.Instance.CanTriggerWander)
				if (CompanionData.Instance.CanTriggerWander)
					mDetectionManager.StartSphinxTrigger();
				else
					mDetectionManager.StopSphinxTrigger();

			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.ThermalFollow) {
				Debug.Log("Companion start follow");
				mActionManager.StartThermalFollow(HumanFollowType.BODY);
			}

			if (Time.time - mTimeThermal <= 5.0F) {
				if (!(Interaction.Mood.CurrentMood == MoodType.HAPPY || Interaction.Mood.CurrentMood == MoodType.NEUTRAL) && Interaction.Face.IsStable) {

					mActionManager.TimedMood(MoodType.HAPPY, 3F);
					if (Primitive.Speaker.Voice.Status != SoundChannelStatus.PLAYING)
						Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);

				}
			} else if (Time.time - mTimeThermal < 10.0F) {
				if (Interaction.Face.IsStable && Interaction.Mood.CurrentMood != MoodType.THINKING) {
					
					Interaction.Mood.Set(MoodType.THINKING);
					if (Primitive.Speaker.Voice.Status != SoundChannelStatus.PLAYING)
						Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);

				}
			} else if (Time.time - mTimeThermal < 15.0F) {
				if (Interaction.Face.IsStable && Interaction.Mood.CurrentMood != MoodType.SCARED) {
					//Buddy is alone now -> scared
					Interaction.Mood.Set(MoodType.SCARED);

				}
			} else if (Time.time - mTimeThermal <= 20.0F) {
				if (Interaction.Mood.CurrentMood != MoodType.SAD && Interaction.Face.IsStable) {
					//Buddy is alone sad
					Interaction.Mood.Set(MoodType.SAD);

				}
			} else if (Time.time - mTimeThermal > 20.0F) {
				//Buddy is alone sad
				mActionManager.StopThermalFollow();
				Trigger("IDLE");
				// TODO: Maybe Trigger("LOOKINGFOR");
			}

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
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
					mDetectionManager.mDetectedElement = Detected.NONE;
					Interaction.Mood.Set(MoodType.TIRED);
					break;

				// If thermal signature, nothing
				case Detected.THERMAL:
					mDetectionManager.mDetectedElement = Detected.NONE;
					mTimeThermal = Time.time;
					break;

				//case Detected.HUMAN_RGB & Detected.THERMAL:
				//	// TODO: check false positive level
				//	mTrigged = true;
				//	if (CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
				//		mTrigged = true;
				//		Trigger("INTERACT");
				//	}
				//	break;

				default:
					mDetectionManager.mDetectedElement = Detected.NONE;
					break;
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			Interaction.Mood.Set(MoodType.NEUTRAL);
			mDetectionManager.StartSphinxTrigger();
		}

	}
}