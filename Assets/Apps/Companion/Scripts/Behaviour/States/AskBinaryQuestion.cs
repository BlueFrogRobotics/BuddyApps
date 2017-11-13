using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Companion
{
	public class AskBinaryQuestion: AStateMachineBehaviour
	{
		private bool mListening;
		private string mSpeechReco;

		[SerializeField]
		private string option1Key;

		[SerializeField]
		private string option1Trigger;

		[SerializeField]
		private string option2Key;

		[SerializeField]
		private string option2Trigger;

		[SerializeField]
		private string questionKey;

		[SerializeField]
		private string QuitTrigger;
		private bool mRedo;

		//[SerializeField]
		//private string option2;


		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Trigger("VOCALTRIGGERED");
			mState.text = "Ask question";
			Debug.Log("Ask new request");
			mListening = false;
			mSpeechReco = "";
			mRedo = false;
			

			/*Interaction.TextToSpeech.SayKey(questionKey);

			Interaction.SpeechToText.OnBestRecognition.Clear();
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);
			*/

			
		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!Interaction.TextToSpeech.HasFinishedTalking || mListening)
				return;

			if (string.IsNullOrEmpty(mSpeechReco)) {
				Interaction.SpeechToText.Request();
				mListening = true;

				Interaction.Mood.Set(MoodType.LISTENING);
				return;
			}

			if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option2Key)) ) {
				Toaster.Hide();
				RedoListen();
			} else if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option1Key)) || ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings("quit"))) {
				Toaster.Hide();
				Exit();
			} else {
				Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString(questionKey), PressedYes, PressedNo);
				Interaction.TextToSpeech.SayKey("notunderstandyesno", true);
				Interaction.TextToSpeech.Silence(1000, true);
				Interaction.TextToSpeech.SayKey(questionKey, true);

				mSpeechReco = "";
			}

		}


		private void OnSpeechReco(string iVoiceInput)
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);

			mSpeechReco = iVoiceInput;
			mListening = false;
		}

		private void PressedYes()
		{
			Toaster.Hide();
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			RedoListen();
		}

		private void PressedNo()
		{
			Toaster.Hide();
			Primitive.Speaker.FX.Play(FXSound.BEEP_1);
			Exit();
		}

		private void RedoListen()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Trigger("VOCALTRIGGERED");
			mRedo = true;
		}

		private void Exit()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Trigger("DISENGAGE");
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Toaster.Hide();

			//Interaction.TextToSpeech.Say("ok");
			if (mRedo) {
				Interaction.TextToSpeech.SayKey("ilisten");
			}
			Interaction.Mood.Set(MoodType.NEUTRAL);
			mSpeechReco = "";
			mListening = false;
		}

		private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
		{
			iSpeech = iSpeech.ToLower();
			for (int i = 0; i < iListSpeech.Length; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2) {
					words = iSpeech.Split(' ');
					foreach (string word in words) {
						if (word == iListSpeech[i].ToLower()) {
							return true;
						}
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;
		}

	}
}