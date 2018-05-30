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
		private bool mTrigger;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDesireManager = GetComponent<DesireManager>();
			mActionManager = GetComponent<ActionManager>();
			mCompanion = GetComponent<CompanionBehaviour>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Landing";
			Debug.Log("state: Landing ");

			mTrigger = false;

			if (mCompanion.mCurrentUser == null)
				Debug.Log("landing CurrentUser Null");
			else
				Debug.Log("landing CurrentUser not Null");

			TimeSpan lTimeInApp = DateTime.Now - CompanionData.Instance.LastAppTime;
			TimeSpan lTimeSinceStart = DateTime.Now - BYOS.Instance.StartTime;

			Debug.Log("Starting companion, time in os: " + lTimeSinceStart.TotalSeconds + " s or " + lTimeSinceStart.Hours + ":" + lTimeSinceStart.Minutes + ":" + lTimeSinceStart.Seconds);


			// Buddy just turned on?
			if (lTimeSinceStart.TotalSeconds < 75F) {

				mActionManager.TimedMood(MoodType.HAPPY, 5F);
				BYOS.Instance.Interaction.TextToSpeech.SayKey("buddyturnedon");
				Debug.Log("Buddy turned on");
				Trigger("INTERACT");
				mTrigger = true;


			} else {
				// Logs
				System.IO.File.AppendAllText(Resources.GetPathToRaw("appLog.txt"), CompanionData.Instance.LastApp + " " + lTimeInApp.ToString());
				Debug.Log("log file at " + Resources.GetPathToRaw("appLog.txt"));

				if (lTimeInApp.TotalSeconds < 0.5) {
					Debug.Log("Warning, the app " + CompanionData.Instance.LastApp + " ran for less than 0.5 seconds. It may be an error??");
					// TODO: add sentences said by buddy
					BYOS.Instance.Interaction.Mood.Set(MoodType.SICK);
				} else if (lTimeInApp.TotalSeconds < 10F && CompanionData.Instance.LastApp != "Weather" && CompanionData.Instance.LastApp != "Timer" && CompanionData.Instance.LastApp != "Reminder") {
					BYOS.Instance.Interaction.TextToSpeech.SayKey("appcancelled");
					BYOS.Instance.Interaction.Mood.Set(MoodType.THINKING);
				}

				// When we arrive in companion, depending on the previous application, we propose another request or...
				switch (AppUtils.GetAppCategory(CompanionData.Instance.LastApp)) {

					case AppCategory.EDUTAINMENT:
						if (lTimeInApp.TotalSeconds < 10F) {
							CompanionData.Instance.mTeachDesire += 15;
							// user changed his mind? go to propose edutainment?

						} else if (lTimeInApp.TotalSeconds < 100F) {
							CompanionData.Instance.mTeachDesire -= 50;
						} else {
							CompanionData.Instance.mTeachDesire = 0;
						}

						break;

					case AppCategory.GAME:
						if (lTimeInApp.TotalSeconds < 10F) {
							CompanionData.Instance.mInteractDesire += 15;
							// user changed his mind? go to propose edutainment?

						} else if (lTimeInApp.TotalSeconds < 100F) {
							CompanionData.Instance.mInteractDesire -= 50;
						} else {
							CompanionData.Instance.mInteractDesire = 0;
						}
						break;

					default:
						if (lTimeInApp.TotalSeconds < 10F) {
							CompanionData.Instance.mHelpDesire += 15;
							// user changed his mind? go to propose edutainment?

						} else if (lTimeInApp.TotalSeconds < 100F) {
							CompanionData.Instance.mHelpDesire -= 50;
						} else {
							CompanionData.Instance.mHelpDesire = 0;
						}
						break;
				}
			}
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking && !mTrigger) {
				if (CompanionData.Instance.LandingTrigger)
					Trigger("INTERACT");
				else
					Trigger("VOCALCOMMAND");
				mTrigger = true;

			}
		}

	}
}