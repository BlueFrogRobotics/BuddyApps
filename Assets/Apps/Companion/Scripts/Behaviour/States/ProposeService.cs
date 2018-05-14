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
	public class ProposeService : AStateMachineBehaviour
	{
		private bool mNoGame;

		private float mTime;

		private List<string> mKeyOptions;

		private string mProposal;

		//private TextToSpeech mTTS = BYOS.Instance.Interaction.TextToSpeech;
		private bool mNeedListen;

		public override void Start()
		{
			mProposal = "";
			mKeyOptions = new List<string>();
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			mKeyOptions = new List<string>();
			//mKeyOptions.Add("iot");
			mKeyOptions.Add("weather");
			mKeyOptions.Add("jukebox");

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Propose Service";
			mNeedListen = true;
			mProposal = mKeyOptions[UnityEngine.Random.Range(0, mKeyOptions.Count)];


			mDetectionManager.StopSphinxTrigger();
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.GAME;
			mTime = 0F;
			mNoGame = false;
			Interaction.Mood.Set(MoodType.HAPPY);

			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("attention") + " " + Dictionary.GetRandomString("propose" + mProposal));


			Interaction.VocalManager.EnableDefaultErrorHandling = false;

			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);

			//Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetString("propose" + mProposal), YesAnswer, NoAnswer);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (mTime > 20F || mNoGame) {
				iAnimator.SetTrigger("INTERACT");
				CompanionData.Instance.mHelpDesire -= 30;
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
			//Toaster.Hide();
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
		}

		private void YesAnswer()
		{
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("herewego"));
			//Toaster.Hide();
			OnAnswer(mProposal);
		}

		private void NoAnswer()
		{
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("nopb"));
			//Toaster.Hide();
			OnAnswer("nogame");
		}

		////private string RandomProposal(List<string> iProp)
		////{
		////    string  lProp = "";
		////    int lRdmOne = UnityEngine.Random.Range(1, 4);

		////    switch (lRdmOne) {
		////        case 1:
		////            lProp = "joke";
		////            break;
		////        case 2:
		////            lProp = "dance";
		////            break;
		////        case 3:
		////            lProp = iProp[UnityEngine.Random.Range(1, iProp.Count)] ;
		////            break;
		////    }
		////    return lProp;
		////}



		void OnAnswer(string iAnswer)
		{
			switch (iAnswer) {
				case "jukebox":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "Jukebox";
					CompanionData.Instance.LandingTrigger = true;
					new StartAppCmd("Jukebox").Execute();
					break;

				case "weather":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "Weather";
					CompanionData.Instance.LandingTrigger = true;
					new StartAppCmd("Weather").Execute();
					break;

				//case "somfy":
				//	CompanionData.Instance.LastAppTime = DateTime.Now;
				//	CompanionData.Instance.LastApp = "Somfy";
				//	CompanionData.Instance.LandingTrigger = true;
				//	new StartAppCmd("Somfy").Execute();
				//	break;

				case "nogame":
					CompanionData.Instance.mHelpDesire -= 50;
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