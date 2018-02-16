using Buddy;
using Buddy.UI;
using Buddy.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class Inform : AStateMachineBehaviour
	{



		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Inform";

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.INFORM;

			// TODO: list of what to tell:

			// 1 Robot State (battery, mood)
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("informbattery")
				.Replace("[batterylevel]", BYOS.Instance.Primitive.Battery.EnergyLevel.ToString()));

			// 2 external sensors (IOT, weather)

			// 3 General knowledge (fun facts)

			// 4 knowledge about other users

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if ((Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying) || mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {
				mActionManager.StopAllBML();

				if (mDetectionManager.mDetectedElement != Detected.MOUTH_TOUCH) {
					// satisfaction
					//BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, -1, "mooddance", "DANCE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.RELAXED));
					CompanionData.Instance.mTeachDesire -= 40;
					CompanionData.Instance.mHelpDesire -= 20;
				}
				Trigger("VOCALCOMMAND");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;
		}

	}
}