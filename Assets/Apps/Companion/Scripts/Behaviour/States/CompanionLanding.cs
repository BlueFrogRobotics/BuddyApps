using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class CompanionLanding : AStateMachineBehaviour
	{
		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDesireManager = GetComponent<DesireManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Landing";
			Debug.Log("state: Landing ");

			float lTimeInApp = Time.time - CompanionData.Instance.LastAppTime;


			//TODO: check when the robot was turn on for 1st wake up!
			// check time os on

			// Logs
			System.IO.File.AppendAllText(Resources.GetPathToRaw("appLog.txt"), CompanionData.Instance.LastApp + " " + lTimeInApp.ToString("hh:mm:ss"));
			Debug.Log("log file at " + Resources.GetPathToRaw("appLog.txt"));

			if (lTimeInApp < 0.5F) {
				Debug.Log("Warning, the app " + CompanionData.Instance.LastApp + " ran for less than 0.5 seconds. It may be an error??");
				BYOS.Instance.Interaction.Mood.Set(MoodType.SICK);
			}

			else if (lTimeInApp < 10F && CompanionData.Instance.LastApp != "Weather" && CompanionData.Instance.LastApp != "Timer" && CompanionData.Instance.LastApp != "Reminder") {
				BYOS.Instance.Interaction.TextToSpeech.SayKey("appcancelled");
				BYOS.Instance.Interaction.Mood.Set(MoodType.THINKING);
			}

			// When we arrive in companion, depending on the previous application, we propose another request or...
			switch (AppUtils.GetAppCategory(CompanionData.Instance.LastApp)) {

				//case AppCategory.EDUTAINMENT:
				//	if (lTimeInApp < 10F) {
				//		CompanionData.Instance.mTeachDesire += 15;
				//		// user changed his mind? go to propose edutainment?

				//	} else if (lTimeInApp < 100F) {
				//		CompanionData.Instance.mTeachDesire -= 50;
				//	} else {
				//		CompanionData.Instance.mTeachDesire = 0;
				//	}

				//	break;

				//case AppCategory.GAME:
				//	if (lTimeInApp < 10F) {
				//		CompanionData.Instance.mInteractDesire += 15;
				//		// user changed his mind? go to propose edutainment?

				//	} else if (lTimeInApp < 100F) {
				//		CompanionData.Instance.mInteractDesire -= 50;
				//	} else {
				//		CompanionData.Instance.mInteractDesire = 0;
				//	}
				//	break;

				default:
					if (lTimeInApp < 10F) {
						CompanionData.Instance.mHelpDesire += 15;
						// user changed his mind? go to propose edutainment?

					} else if (lTimeInApp < 100F) {
						CompanionData.Instance.mHelpDesire -= 50;
					} else {
						CompanionData.Instance.mHelpDesire = 0;
					}
					break;


			}

		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			if (Interaction.TextToSpeech.HasFinishedTalking)
				Trigger("INTERACT");
		}

	}
}