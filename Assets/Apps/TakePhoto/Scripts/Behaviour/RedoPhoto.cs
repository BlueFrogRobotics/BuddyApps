using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.TakePhoto
{
	public class RedoPhoto : AStateMachineBehaviour
	{
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
		private int mError;
		private bool mQuit;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("Ask new request");
			mError = 0;
			mSpeechReco = "";

			mQuit = false;
            Buddy.Vocal.SayKey(questionKey);

			Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add((iInput) => { OnSpeechReco(iInput.Utterance); });

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString(questionKey));
            },

              () => { PressedNo(); }, " no",

              () => {
                  PressedYes();
                  

                  Buddy.GUI.Toaster.Hide();

              }, "yes"

             );
        }


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (Buddy.Vocal.IsSpeaking || Buddy.Vocal.IsListening)
				return;
			else if (mQuit) {
				QuitApp();
            }

			if (string.IsNullOrEmpty(mSpeechReco)) {
				Buddy.Vocal.Listen();

				Buddy.Behaviour.Mood.Set(FacialExpression.LISTENING);
				return;
			}

			if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings(option2Key))) {
				Buddy.GUI.Toaster.Hide();
				Option2();
			} else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings(option1Key)) ) {
                Buddy.GUI.Toaster.Hide();
				Option1();
			} else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("quit"))) {
                Buddy.GUI.Toaster.Hide();
				Option1();
			} else {
				Buddy.Vocal.SayKey("notunderstandyesno", true);
				mError++;
                if (mError > 2) {
					QuitApp();
				} else {
					Buddy.Vocal.Say("[1000]", true);
					Buddy.Vocal.SayKey(questionKey, true);
				}

				mSpeechReco = "";
			}

		}

        

		private  void OnSpeechReco(string iVoiceInput)
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);

			mSpeechReco = iVoiceInput;
		}

		private void PressedYes()
		{
			Buddy.GUI.Toaster.Hide();
            Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
            Option2();
		}

		private void PressedNo()
		{
            Buddy.GUI.Toaster.Hide();
			Buddy.Actuators.Speakers.Media.Play(SoundSample.BEEP_1);
			Option1();
		}

		private void Option2()
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			Trigger(option2Trigger);
		}

		private void Option1()
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			if (Buddy.Sensors.RGBCamera.IsOpen) {
                Buddy.Sensors.RGBCamera.Close();
			}
			Buddy.Vocal.SayKey("bye");
			mQuit = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            Buddy.GUI.Toaster.Hide();

			Buddy.Vocal.Say("ok");
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			mSpeechReco = "";
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