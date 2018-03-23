﻿using Buddy;
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

			int lRandom = UnityEngine.Random.Range(0, 4);
				



			// TODO: list of what to tell:



			// 1 Robot State (battery, mood)
			switch (lRandom) {

				case 0:
					// If desire to express mood, then express mood
					int lMaxInternalValue = Math.Max(Math.Abs(BYOS.Instance.Interaction.InternalState.Positivity), Math.Abs(BYOS.Instance.Interaction.InternalState.Energy));
					EmotionalEvent lEventMood = Interaction.InternalState.ExplainMood();
					if (lMaxInternalValue > 4 && lEventMood != null) {
						Debug.Log("[COMPANION][INFORM] key: " + Interaction.InternalState.ExplainMood().ExplanationKey + " dico value: " + Dictionary.GetRandomString(Interaction.InternalState.ExplainMood().ExplanationKey));
						mActionManager.ShowInternalMood();
						Interaction.TextToSpeech.Say(Dictionary.GetRandomString("ifeel") + " " + Dictionary.GetString(Interaction.InternalState.InternalStateMood.ToString().ToLower()) + " "
							+ Dictionary.GetRandomString("because") + " " + Dictionary.GetRandomString(Interaction.InternalState.ExplainMood().ExplanationKey), true);

					} else {
						if(BYOS.Instance.Primitive.Battery.EnergyLevel < 0) {
							mActionManager.TimedMood(MoodType.SCARED, 7F);
							Interaction.TextToSpeech.Say("Oh my God! I can't feel my battery anymore! [200] Please put it back!!");
						} else {
							Interaction.TextToSpeech.Say(Dictionary.GetRandomString("informbattery")
							.Replace("[batterylevel]", ( (int) BYOS.Instance.Primitive.Battery.EnergyLevel).ToString()));
						}
					}
					break;


				// 2 external sensors (IOT, weather)
				case 1:
					// TODO: add more random cities

					string lParam = Dictionary.GetString("whatweather") + " " + Dictionary.GetRandomString("inlocation") + " " + Dictionary.GetRandomString("citylist");

					Debug.Log("[COMPANION][INFORM] start app weather with param " + lParam);
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "Weather";
					new StartAppCmd("Weather", new int[] { }, new float[] { }, new string[] { lParam }).Execute();
					CompanionData.Instance.LandingTrigger = true;

					break;



				// 3 General knowledge (fun facts)
				case 2:
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introfunfact") + " " + Dictionary.GetRandomString("funfacts") );
					break;

				// 4 knowledge about other users
				// TODO
				case 3:
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introfunfact") + " " + Dictionary.GetRandomString("funfacts"));
					break;

				default:
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introfunfact") + " " + Dictionary.GetRandomString("funfacts"));
					break;

			}




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
				Trigger("INTERACT");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;
		}

	}
}