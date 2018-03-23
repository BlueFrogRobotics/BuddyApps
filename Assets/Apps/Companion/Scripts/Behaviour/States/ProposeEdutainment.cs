using Buddy;
using Buddy.Command;
using Buddy.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class ProposeEdutainment : AStateMachineBehaviour
	{
		private bool mNoGame;

		private float mTime;

		private List<string> mKeyOptions;

		private string mProposal;

		private bool mNeedListen;

		public override void Start()
		{
			mProposal = "";
			mKeyOptions = new List<string>();
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			mKeyOptions = new List<string>();
			mKeyOptions.Add("memory");
			mKeyOptions.Add("playmath");

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Propose  edutain";
			mNeedListen = true;
			mProposal = mKeyOptions[UnityEngine.Random.Range(0, mKeyOptions.Count)];

			mDetectionManager.StopSphinxTrigger();

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.GAME;
			mTime = 0F;
			mNoGame = false;
			Interaction.Mood.Set(MoodType.HAPPY);

			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("attention") + " " + Dictionary.GetRandomString("propose" + mProposal));


			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);

			Interaction.VocalManager.EnableDefaultErrorHandling = false;

			Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString("propose" + mProposal), YesAnswer, NoAnswer);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (mTime > 60F || mNoGame) {
				iAnimator.SetTrigger("INTERACT");
			}

			if (Interaction.TextToSpeech.HasFinishedTalking && mNeedListen) {
				Interaction.VocalManager.StartInstantReco();
				mNeedListen = false;
			}

		}

		private void OnSpeechRecognition(string iMsg)
		{
			// TODO add emotion event
			if (ContainsOneOf(iMsg, Dictionary.GetPhoneticStrings("accept")))
				YesAnswer();
			else if (ContainsOneOf(iMsg, Dictionary.GetPhoneticStrings("refuse")))
				NoAnswer();
			else
				mNeedListen = true;
		}

		private void ErrorSTT(STTError iError)
		{
			mNeedListen = true;
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
		}

		private void YesAnswer()
		{
			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("herewego"));
			Toaster.Hide();
			OnAnswer(mProposal);
		}

		private void NoAnswer()
		{
			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("nopb"));
			Toaster.Hide();
			OnAnswer("nogame");
		}



		void OnAnswer(string iAnswer)
		{
			switch (iAnswer) {
				case "playmath":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "PlayMath";
					new StartAppCmd("PlayMath").Execute();
					break;

				case "memory":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "MemoryGameApp";
					new StartAppCmd("MemoryGameApp").Execute();
					break;

				case "nogame":
					CompanionData.Instance.mTeachDesire += 10;
					mNoGame = true;
					break;
			}
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