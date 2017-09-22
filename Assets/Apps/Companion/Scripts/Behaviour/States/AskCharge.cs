using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Companion
{
	public class AskCharge : AStateMachineBehaviour
	{
		private bool mListening;
		private string mSpeechReco;

		private List<string> mAcceptPhonetics;
		private List<string> mRefusePhonetics;
		private List<string> mQuitPhonetics;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mState.text = "Ask new request";
			Debug.Log("Ask new request");
			mListening = false;
			mSpeechReco = "";

			mAcceptPhonetics = new List<string>(Dictionary.GetPhoneticStrings("accept"));
			mRefusePhonetics = new List<string>(Dictionary.GetPhoneticStrings("refuse"));
			mQuitPhonetics = new List<string>(Dictionary.GetPhoneticStrings("quit"));

			Interaction.TextToSpeech.SayKey("askcharge");

			Interaction.SpeechToText.OnBestRecognition.Clear();
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechReco);


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

			if (mAcceptPhonetics.Contains(mSpeechReco)) {
				Toaster.Hide();
				RedoListen();
			} else if (mRefusePhonetics.Contains(mSpeechReco) || mQuitPhonetics.Contains(mSpeechReco)) {
				Toaster.Hide();
				Exit();
			} else {
				Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString("newrequest"), PressedYes, PressedNo);
				Interaction.TextToSpeech.SayKey("notunderstandyesno", true);
				Interaction.TextToSpeech.Silence(1000, true);
				Interaction.TextToSpeech.SayKey("newrequest", true);

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
		}

		private void Exit()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Interaction.TextToSpeech.Say("ok");
			Trigger("DISENGAGE");
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Toaster.Hide();
			Interaction.Mood.Set(MoodType.NEUTRAL);
			mSpeechReco = "";
			mListening = false;
		}

	}
}