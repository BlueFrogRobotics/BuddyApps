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
	public class ListenJoke : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private bool mTrigg;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "LISTEN Joke";


			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.JOKE;

			mNeedListen = false;
			mTrigg = false;

			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);

			// TODO add log
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking)
				if (mTrigg)
					Trigger("VOCALCOMMAND");
				else if (mNeedListen) {
					Interaction.VocalManager.StartInstantReco();
					mNeedListen = false;
				}
		}

		private void OnSpeechRecognition(string iMsg)
		{
			if (ContainsOneOf(iMsg, "accept")) {
				Interaction.TextToSpeech.SayKey("ilisten");
				mNeedListen = true;
			} else if (ContainsOneOf(iMsg, "refuse")) {
				Interaction.TextToSpeech.SayKey("nopb");
				Trigger("TELLJOKE");
			} else if (ContainsOneOf(iMsg, "questionwords")) {
				Interaction.TextToSpeech.SayKey("idontknow");
				mNeedListen = true;
			} else {

				// if Buddy in good mood, laugh easily, otherwise, hardly
				if (UnityEngine.Random.Range(0, 10) > 1) {
					if (Interaction.InternalState.Positivity > 0) {
						mActionManager.SetMood(MoodType.HAPPY, 6);
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						Interaction.TextToSpeech.SayKey("jokefunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(2, 1, "ugoodjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
					} else {
						mActionManager.SetMood(MoodType.GRUMPY, 6);
						Interaction.TextToSpeech.SayKey("jokenotfunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(-2, 0, "ubadjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.SALTY));
					}
				} else {
					if (Interaction.InternalState.Positivity <= 0) {
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						Interaction.TextToSpeech.SayKey("jokefunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(2, 1, "ugoodjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
					} else {
						Interaction.TextToSpeech.SayKey("jokenotfunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(-2, 0, "ubadjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.SALTY));
					}
				}
				mTrigg = true;

			}
		}

		private void ErrorSTT(STTError iError)
		{
			mNeedListen = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;

			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
		}

		private bool ContainsOneOf(string iSpeech, string iKeySpeech)
		{
			string[] iListSpeech = BYOS.Instance.Dictionary.GetPhoneticStrings(iKeySpeech);


			for (int i = 0; i < iListSpeech.Length; ++i) {

				if (string.IsNullOrEmpty(iListSpeech[i]))
					continue;

				string[] words = iSpeech.Split(' ');
				if (words.Length < 2 && !string.IsNullOrEmpty(words[0])) {
					if (words[0].ToLower() == iListSpeech[i].ToLower()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;
		}

	}
}