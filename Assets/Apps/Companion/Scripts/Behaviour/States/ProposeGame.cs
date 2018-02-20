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
	public class ProposeGame : AStateMachineBehaviour
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
			mKeyOptions.Add("freezedance");
			mKeyOptions.Add("rlgl");

		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Propose Game";
			mNeedListen = true;
			mProposal = mKeyOptions[UnityEngine.Random.Range(0, mKeyOptions.Count)];


			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.GAME;
			mTime = 0F;
			mNoGame = false;
			Interaction.Mood.Set(MoodType.HAPPY);

			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("attention") + " " + Dictionary.GetRandomString("propose" + mProposal));


			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);

			Toaster.Display<BinaryQuestionToast>().With(Dictionary.GetRandomString(mProposal), YesAnswer, NoAnswer);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;

			if (mTime > 60F || mNoGame) {
				iAnimator.SetTrigger("VOCALCOMMAND");
			}

			if (Interaction.TextToSpeech.HasFinishedTalking && mNeedListen) {
				Interaction.VocalManager.StartInstantReco();
				mNeedListen = false;
			}

		}

		private void OnSpeechRecognition(string iMsg)
		{
			//// TODO add emotion event
			if (ContainsOneOf(iMsg, Dictionary.GetPhoneticStrings("yes")))
				YesAnswer();
			else if (ContainsOneOf(iMsg, Dictionary.GetPhoneticStrings("no")))
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
		}

		private void YesAnswer()
		{
			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("herewego"));
			OnAnswer(mProposal);
		}

		private void NoAnswer()
		{
			BYOS.Instance.Interaction.TextToSpeech.Say(Dictionary.GetRandomString("nopb"));
			OnAnswer("nogame");
		}

		//private string RandomProposal(List<string> iProp)
		//{
		//    string  lProp = "";
		//    int lRdmOne = UnityEngine.Random.Range(1, 4);

		//    switch (lRdmOne) {
		//        case 1:
		//            lProp = "joke";
		//            break;
		//        case 2:
		//            lProp = "dance";
		//            break;
		//        case 3:
		//            lProp = iProp[UnityEngine.Random.Range(1, iProp.Count)] ;
		//            break;
		//    }
		//    return lProp;
		//}



		void OnAnswer(string iAnswer)
		{
			switch (iAnswer) {
				case "playmath":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "PlayMath";
					CompanionData.Instance.LandingTrigger = false;
					new StartAppCmd("PlayMath").Execute();
					break;

				case "freezedance":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "FreezeDanceApp";
					CompanionData.Instance.LandingTrigger = false;
					new StartAppCmd("FreezeDanceApp").Execute();
					break;

				case "rlgl":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "RLGLApp";
					CompanionData.Instance.LandingTrigger = false;
					new StartAppCmd("RLGLApp").Execute();
					break;

				case "memory":
					CompanionData.Instance.LastAppTime = DateTime.Now;
					CompanionData.Instance.LastApp = "MemoryGameApp";
					CompanionData.Instance.LandingTrigger = false;
					new StartAppCmd("MemoryGameApp").Execute();
					break;

				case "nogame":
					CompanionData.Instance.mInteractDesire += 10;
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