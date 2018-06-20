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

			mActionManager.CurrentAction = BUDDY_ACTION.FOLLOW;
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

			mActionTrigger = mActionManager.NeededAction(COMPANION_STATE.IDLE);
			if (!string.IsNullOrEmpty(mActionTrigger))
				Trigger(mActionTrigger);
			else {

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
					if (!mActionManager.CurrentActionHumanOrder) {
						//Follow was not an order -> do what Buddy desires
						mActionManager.StopThermalFollow();

						mActionTrigger = mActionManager.DesiredAction(COMPANION_STATE.FOLLOW);
						if (mActionTrigger != "FOLLOW")
							Trigger(mActionTrigger);
					}
				}

				// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
				if (mDetectionManager.mDetectedElement == Detected.THERMAL) {
					mDetectionManager.mDetectedElement = Detected.NONE;
					mTimeThermal = Time.time;
				} else if (mActionManager.CurrentActionHumanOrder && mDetectionManager.mDetectedElement == Detected.BATTERY && Interaction.Mood.CurrentMood != MoodType.TIRED) {
					mDetectionManager.mDetectedElement = Detected.NONE;
					Interaction.Mood.Set(MoodType.TIRED);
				} else if (string.IsNullOrEmpty(mActionTrigger) || mActionTrigger != "FOLLOW") {

					// We react only if Buddy didn't chose an action.
					// This may not be the best way but this has very low chances (almost none) to happen at the same time (frame) ...

					//Debug.Log("No action chosen, react?");

					mActionTrigger = mActionManager.LaunchReaction(COMPANION_STATE.FOLLOW, mDetectionManager.mDetectedElement);
					if (!string.IsNullOrEmpty(mActionTrigger)) {
						Trigger(mActionTrigger);
						Debug.Log("Follow reaction chosen " + mActionTrigger);
					}
				} else
					Debug.Log("Follow action chosen " + mActionTrigger);

			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			Interaction.Mood.Set(MoodType.NEUTRAL);
			mActionManager.StopAllActions();
			mDetectionManager.StartSphinxTrigger();
		}

	}
}