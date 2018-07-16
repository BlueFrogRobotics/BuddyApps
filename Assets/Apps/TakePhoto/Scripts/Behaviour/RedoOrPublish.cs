using UnityEngine;
using BlueQuark;

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
		private bool mQuit;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("Ask new request");
			mListening = false;
			mSpeechReco = "";

			mQuit = false;
			mError = 0;
			Buddy.Vocal.SayKey("nicepic");
			Buddy.Vocal.SayKey(questionKey, true);
            
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add((iInput) => { OnSpeechReco(iInput.Utterance); });


			
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!Buddy.Vocal.IsSpeaking || mListening)
				return;
			else if (mQuit) {
				QuitApp();
			}

			if (string.IsNullOrEmpty(mSpeechReco)) {
                Buddy.Vocal.Listen();
				mListening = true;

				Buddy.Behaviour.Mood.Set(FacialExpression.LISTENING);
				return;
			}

			if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings(option2Key))) {
				Option2();
			} else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings(option1Key)) ) {
				Option1();
			}else if (ContainsOneOf(mSpeechReco, Buddy.Resources.GetPhoneticStrings("quit"))) {
				Quit();
			} else {
				Buddy.Vocal.SayKey("notunderstand", true);
				mError++;
				if (mError > 2) {
					QuitApp();
				} else {
                    Buddy.Vocal.Say("[1000]");
					Buddy.Vocal.SayKey(questionKey, true);
                }
				mSpeechReco = "";
			}

		}

		private void Quit()
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			//Trigger(quitTrigger);
			if (Buddy.Sensors.RGBCamera.IsOpen) {
                Buddy.Sensors.RGBCamera.Close();
			}
			Buddy.Vocal.SayKey("bye");
			mQuit = true;
		}

		private void OnSpeechReco(string iVoiceInput)
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);

			mSpeechReco = iVoiceInput;
			mListening = false;
		}

		private void Option2()
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			Trigger(option2Trigger);
		}

		private void Option1()
		{
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
			Trigger(option1Trigger);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Buddy.Vocal.Say("ok");
			Buddy.GUI.Toaster.Hide();
			Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
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