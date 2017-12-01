using UnityEngine;
using Buddy;

namespace BuddyApp.TakePhoto
{
	public class RedoOrPublish : AStateMachineBehaviour
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
		private int mError;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("Ask new request");
			mListening = false;
			mSpeechReco = "";

			mError = 0;
			Interaction.TextToSpeech.SayKey("nicepic");
			Interaction.TextToSpeech.SayKey(questionKey, true);

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

			if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option2Key))) {
				Option2();
			} else if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings(option1Key)) ) {
				Option1();
			}else if (ContainsOneOf(mSpeechReco, Dictionary.GetPhoneticStrings("quit"))) {
				Quit();
			} else {
				Interaction.TextToSpeech.SayKey("notunderstand", true);
				mError++;
				if (mError > 2) {
					QuitApp();
				} else {
					Interaction.TextToSpeech.Silence(1000, true);
					Interaction.TextToSpeech.SayKey(questionKey, true);
                }

				mSpeechReco = "";
			}

		}

		private void Quit()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			//Trigger(quitTrigger);
			if (Primitive.RGBCam.IsOpen) {
				Primitive.RGBCam.Close();
			}
			QuitApp();
		}

		private void OnSpeechReco(string iVoiceInput)
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);

			mSpeechReco = iVoiceInput;
			mListening = false;
		}

		private void Option2()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Trigger(option2Trigger);
		}

		private void Option1()
		{
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Trigger(option1Trigger);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Interaction.TextToSpeech.Say("ok");
			Toaster.Hide();
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