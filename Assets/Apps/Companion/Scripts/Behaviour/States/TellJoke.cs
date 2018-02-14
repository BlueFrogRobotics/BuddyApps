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
	public class TellJoke : AStateMachineBehaviour
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
			mState.text = "Tell Joke";

			Debug.Log("Playing BML joke");

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.JOKE;

			// TODO: improve this to have multi turn jokes
			Interaction.BMLManager.LaunchRandom("joke");
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if ((Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying) || mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {

				mActionManager.StopAllBML();

				if (mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {
					// frustration
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 0, "moodjokestop", "JOKESTOP", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.SALTY));

				} else {
					// TODO: if not interupted, reduce interact desire?
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, 1, "moodjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.EXCITED));
					CompanionData.Instance.mInteractDesire -= 40;
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