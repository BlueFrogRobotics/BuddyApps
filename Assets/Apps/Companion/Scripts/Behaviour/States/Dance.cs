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
	public class Dance : AStateMachineBehaviour
	{

		// For now, just play BML
        public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Dance";

			Debug.Log("Dancing");

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.DANCE;

			// TODO: improve this?
			Interaction.BMLManager.LaunchRandom("dance");
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if ((Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying) || mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {
				mActionManager.StopAllBML();

				if (mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {
					// frustration
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 1, "mooddancestoped", "DANCESTOP", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.ANGRY));
					Trigger("VOCALCOMMAND");

				} else {
					// satisfaction
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, -1, "mooddance", "DANCE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.RELAXED));
					CompanionData.Instance.mMovingDesire -= 40;
					Trigger("IDLE");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

        
	}
}