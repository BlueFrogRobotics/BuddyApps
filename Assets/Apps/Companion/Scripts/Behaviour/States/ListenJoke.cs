using Buddy;
using Buddy.UI;
using Buddy.Command;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class ListenJoke : AStateMachineBehaviour
	{
		private bool mNeedListen;
		private bool mTrigg;
		private int mKnockKnock;
		private float mTime;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "LISTEN Joke";

			Debug.Log("listen joke");

			mTime = 0F;
			mKnockKnock = 0;

			Debug.Log("listen joke lastans " + Interaction.SpeechToText.LastAnswer);
			if (!string.IsNullOrEmpty(Interaction.SpeechToText.LastAnswer))
				if (ContainsOneOf(Interaction.SpeechToText.LastAnswer, "knockknock"))
					mKnockKnock = 1;


			Debug.Log("listen joke knockknock " + mKnockKnock);
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.JOKE;

			mNeedListen = true;
			mTrigg = false;

			mDetectionManager.StopSphinxTrigger();
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);

			// TODO add log
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (Interaction.TextToSpeech.HasFinishedTalking && Interaction.SpeechToText.HasFinished)
				if (mTrigg)
					Trigger("VOCALCOMMAND");
				else if (mNeedListen) {
					Interaction.VocalManager.StartInstantReco();
					mNeedListen = false;
				}


			if (mTime > 20F ) {
				iAnimator.SetTrigger("INTERACT");
				CompanionData.Instance.mLearnDesire -= 30;
			}
		}

		private void OnSpeechRecognition(string iMsg)
		{

			if (mKnockKnock == 1) {
				Interaction.TextToSpeech.Say(iMsg + " " + Dictionary.GetString("who"));
				mKnockKnock++;
				mNeedListen = true;
			} else if (ContainsOneOf(iMsg, "accept")) {
				Interaction.TextToSpeech.SayKey("ilisten");
				mNeedListen = true;
			} else if (ContainsOneOf(iMsg, "refuse")) {
				Interaction.TextToSpeech.SayKey("nopb");
				Trigger("TELLJOKE");
			} else if (ContainsOneOf(iMsg, "questionwords")) {
				Interaction.TextToSpeech.SayKey("idontknow");
				mNeedListen = true;
			} else if (ContainsOneOf(Interaction.SpeechToText.LastAnswer, "knockknock")) {
				mKnockKnock = 1;
				Interaction.TextToSpeech.SayKey("whoisthere");
				mNeedListen = true;

				// end of joke
			} else {

				// if Buddy in good mood, laugh easily, otherwise, hardly
				if (UnityEngine.Random.Range(0, 10) > 1) {
					if (Interaction.InternalState.Positivity > 0) {
						mActionManager.SetMood(MoodType.HAPPY, 6);
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						Interaction.TextToSpeech.SayKey("jokefunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(4, 1, "ugoodjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
					} else {
						mActionManager.SetMood(MoodType.GRUMPY, 6);
						Interaction.TextToSpeech.SayKey("jokenotfunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(-2, 0, "ubadjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.BITTER));
					}
				} else {
					if (Interaction.InternalState.Positivity <= 0) {
						mActionManager.SetMood(MoodType.HAPPY, 6);
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						Interaction.TextToSpeech.SayKey("jokefunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(4, 1, "ugoodjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));
					} else {
						mActionManager.SetMood(MoodType.GRUMPY, 6);
						Interaction.TextToSpeech.SayKey("jokenotfunny");
						BYOS.Instance.Interaction.InternalState.AddCumulative(
							new EmotionalEvent(-2, 0, "ubadjoke", "JOKE", EmotionalEventType.INTERACTION, InternalMood.BITTER));
					}
				}
				mTrigg = true;

			}
		}

		private void ErrorSTT(STTError iError)
		{

			mState.text = "LISTEN Joke " + iError;
			mNeedListen = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;
			CompanionData.Instance.mLearnDesire -= 50;
			CompanionData.Instance.mInteractDesire -= 50;

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
					if (words[0].ToLower() == iListSpeech[i].ToLower().Trim()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower().Trim()))
					return true;
			}
			return false;
		}

	}
}