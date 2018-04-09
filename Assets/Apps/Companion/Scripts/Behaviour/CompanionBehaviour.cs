using UnityEngine.UI;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.Companion
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class CompanionBehaviour : MonoBehaviour
	{
		/*
         * Modified data from the UI interaction
         */
		[SerializeField]
		private Text text;

		/*
         * API of the robot
         */
		private TextToSpeech mTextToSpeech;

		/*
         * Data of the application. Save on disc when app quit happened
         */
		private CompanionData mAppData;
		/*
         * Init refs to API and your app data
         */
		void Start()
		{
			mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
			mAppData = CompanionData.Instance;
		}

		/*
         * A sample of use of data (here for basic display purpose)
         */
		void Update()
		{

			// Update speech rate / pitch with mood
			if (Math.Abs(BYOS.Instance.Interaction.InternalState.Positivity) < 10) {
				if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.Pitch, (1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity)) ) {

					Debug.Log("Pitchm: " + BYOS.Instance.Interaction.TextToSpeech.Pitch + " != " + (1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity));
					BYOS.Instance.Interaction.TextToSpeech.SetPitch(1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity);
					Debug.Log("Pitchm: " + BYOS.Instance.Interaction.TextToSpeech.Pitch + " == " + (1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity));
				}

				//If max value
			} else if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.Pitch, Math.Sign(BYOS.Instance.Interaction.InternalState.Positivity) * 1.3F) ) {

				Debug.Log("Pitch: " + BYOS.Instance.Interaction.TextToSpeech.Pitch + " != " + (Math.Sign(BYOS.Instance.Interaction.InternalState.Positivity) * 1.3F));
				BYOS.Instance.Interaction.TextToSpeech.SetPitch(Math.Sign(BYOS.Instance.Interaction.InternalState.Positivity) * 1.3F);
				Debug.Log("Pitch: " + BYOS.Instance.Interaction.TextToSpeech.Pitch + " == " + (1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity));
			}


			if (Math.Abs(BYOS.Instance.Interaction.InternalState.Energy) < 10) {
				if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.SpeechRate , 1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy) ) {

					Debug.Log("Rate: " + BYOS.Instance.Interaction.TextToSpeech.SpeechRate + " != " + (1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy));
					BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy);
					Debug.Log("Rate: " + BYOS.Instance.Interaction.TextToSpeech.SpeechRate + " == " + (1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy));
				}

				//If max value
			} else if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.SpeechRate, Math.Sign(BYOS.Instance.Interaction.InternalState.Energy) * 1.2F)) {

				Debug.Log("Ratem: " + BYOS.Instance.Interaction.TextToSpeech.SpeechRate + " != " + (1.2F * Math.Sign(BYOS.Instance.Interaction.InternalState.Energy)));
				BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(Math.Sign(BYOS.Instance.Interaction.InternalState.Energy) * 1.2F);
				Debug.Log("Ratem: " + BYOS.Instance.Interaction.TextToSpeech.SpeechRate + " == " + (1.2F * Math.Sign(BYOS.Instance.Interaction.InternalState.Energy)));

			}


			if (text.enabled != CompanionData.Instance.Debug) {
				text.enabled = CompanionData.Instance.Debug;
			}

			// ensure motors stay in same state as config
			if (BYOS.Instance.Primitive.Motors.Wheels.Locked == CompanionData.Instance.CanMoveBody) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked wheels");
				BYOS.Instance.Primitive.Motors.Wheels.Locked = !CompanionData.Instance.CanMoveBody;
			}

			if (BYOS.Instance.Primitive.Motors.YesHinge.Locked == CompanionData.Instance.CanMoveHead) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked head");
				BYOS.Instance.Primitive.Motors.YesHinge.Locked = !CompanionData.Instance.CanMoveHead;
				BYOS.Instance.Primitive.Motors.NoHinge.Locked = !CompanionData.Instance.CanMoveHead;
			}
		}

		private bool EquivalentFloat(float iA, float iB)
		{
			if (Math.Abs(iA - iB) < 0.001)
				return true;
			else
				return false;
		}

	}
}