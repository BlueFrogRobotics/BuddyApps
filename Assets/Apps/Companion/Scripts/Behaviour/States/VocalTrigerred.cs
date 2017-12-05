using Buddy;
using Buddy.Command;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	//[RequireComponent(typeof(Reaction))]
	public class VocalTrigerred : AStateMachineBehaviour
	{
		private VocalHelper mVocalChat;
		private bool mSpeechInput;
		private bool mNeedListen;
		private float mTime;
		private bool mError;
		private string mLastBuddySpeech;
		private bool mNeedToGiveAnswer;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;
			mVocalChat = GetComponent<VocalHelper>();
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "Vocal Triggered";
			Debug.Log("state: Vocal Triggered");

			mLastBuddySpeech = "";
			mNeedToGiveAnswer = false;
			mSpeechInput = false;
			Interaction.VocalManager.EnableTrigger = false;
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);
			mVocalChat.Activate();
			Interaction.VocalManager.EnableDefaultErrorHandling = false;
			mVocalChat.WithNotification = true;
			mVocalChat.OnQuestionTypeFound = SortQuestionType;
			BYOS.Instance.Interaction.SphinxTrigger.StopRecognition();
			mNeedListen = true;
			mTime = 0F;
			SayKey("ilisten");
		}


		void OnSpeechRecognition(string iText)
		{
			mError = false;
			mTime = 0F;
			mSpeechInput = true;
			Debug.Log("Reco vocal: " + iText);
			mVocalChat.SpecialRequest(iText.ToLower());
		}

		void ErrorSTT(STTError iError)
		{
			Debug.Log("Error STT");
			// If first error
			if (!mError) {
				mError = true;
				// Ask repeat
				SayKey("ilisten");
				mNeedListen = true;
			} else {
				// else go away
				Debug.Log("2cd error, go away");
				Trigger("IDLE");
			}
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTime += Time.deltaTime;
			if (Interaction.TextToSpeech.HasFinishedTalking) {
				if (!mVocalChat.BuildingAnswer && mNeedToGiveAnswer) {
					//Give answer:
					Debug.Log("give answer");
					Say(mVocalChat.Answer);
					mNeedToGiveAnswer = false;
					mNeedListen = true;
				} else if (mNeedListen) {
					Debug.Log("Vocal instant reco");
					Interaction.VocalManager.StartInstantReco();
					mNeedListen = false;
					mTime = 0F;
				} else if (!mVocalChat.BuildingAnswer && Interaction.VocalManager.RecognitionFinished && mTime > 10F && !mSpeechInput) {
					//Mb this was a wrong trigger, back to IDLE
					Debug.Log("Back to IDLE: ");
					Trigger("IDLE");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.Log("VOCAL TRIGGERED STATE EXIT");
			mNeedListen = true;
			mSpeechInput = false;
			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			mVocalChat.DisActivate();
			BYOS.Instance.Interaction.SphinxTrigger.LaunchRecognition();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType)
		{
			Debug.Log("Question Type found : " + iType);
			string lSentence = "";

			switch (iType) {


				case "Accept":
					Debug.Log("Accept VocalTrigger");
					SayKey("ilisten");
					mNeedListen = true;
					break;

				case "Alarm":
					StartApp("Alarm");
					break;

				case "Answer":
					if (string.IsNullOrEmpty(mVocalChat.Answer))
						// TODO: generate answer random
						Say("I will just pretend I didn't hear that");
					else
						Say(mVocalChat.Answer);

					mNeedListen = true;
					break;

				case "Babyphone":
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("BabyApp");
					break;

				case "BML":
					Debug.Log("Playing BML " + mVocalChat.Answer);
					Interaction.BMLManager.LaunchByID(mVocalChat.Answer);
					break;

				case "Calcul":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("CalculGameApp");
					break;

				case "CanMove":
					CompanionData.Instance.CanMoveBody = true;
					break;

				case "Date":
					if (BYOS.Instance.Language.CurrentLang == Language.FR) {
						lSentence = Dictionary.GetRandomString("givedate").Replace("[weekday]", DateTime.Now.ToString("dddd", new CultureInfo("fr-FR")));
						lSentence = lSentence.Replace("[month]", "" + DateTime.Now.ToString("MMMM", new CultureInfo("fr-FR")));
					} else {
						lSentence = Dictionary.GetRandomString("givedate").Replace("[weekday]", DateTime.Now.ToString("dddd", new CultureInfo("en-EN")));
						lSentence = lSentence.Replace("[month]", "" + DateTime.Now.ToString("MMMM", new CultureInfo("en-EN")));
					}

					lSentence = lSentence.Replace("[day]", "" + DateTime.Now.Day);
					lSentence = lSentence.Replace("[year]", "" + DateTime.Now.Year);
					Say(lSentence);
					mNeedListen = true;
					break;

				case "Definition":
					mNeedToGiveAnswer = true;
					break;

				case "DontMove":
					mActionManager.StopAllActions();
					CompanionData.Instance.MovingDesire -= 20;
					CompanionData.Instance.CanMoveBody = false;
					break;

				case "FollowMe":
					if (!mActionManager.ThermalFollow) {
						Interaction.TextToSpeech.SayKey("follow");
						CompanionData.Instance.InteractDesire -= 10;
						mActionManager.StartThermalFollow();
						Trigger("FOLLOW");
					}
					break;

				case "FreezeDance":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("FreezeDanceApp");
					break;

				case "Guardian":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Guardian");
					break;

				case "HeadUp":
					Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition - 15F);
					break;

				case "HeadLeft":
					Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition - 20F);
					break;

				case "HeadRight":
					Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition + 20F);
					break;

				case "HeadDown":
					Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition + 15F);
					break;

				case "HideSeek":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("HideAndSeek");
					break;

				case "Hour":
					lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
					lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
					lSentence = lSentence.Replace("[second]", "" + DateTime.Now.Second);
					Say(lSentence);
					mNeedListen = true;
					break;

				case "IOT":
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("IOT");
					break;

				case "Jukebox":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("JukeboxApp_V2");
					break;

				case "Memory":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("MemoryGameApp");
					break;

				case "Photo":
					CompanionData.Instance.InteractDesire -= 30;
					StartApp("TakePhotoApp");
					break;

				case "Pose":
					CompanionData.Instance.InteractDesire -= 30;
					StartApp("TakePoseApp");
					break;

				case "Quit":
					if (mActionManager.ThermalFollow) {
						Trigger("FOLLOW");
					} else {
						Trigger("DISENGAGE");
					}
					break;

				case "Quizz":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("QuizzGameApp");
					break;

				case "Recipe":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Recipe");
					break;

				case "Repeat":
					Interaction.TextToSpeech.SayKey("isaid", true);
					Say(mLastBuddySpeech, true);
					mNeedListen = true;
					break;

				case "RLGL":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("RLGLApp");
					break;

				case "VolumeDown":
					Primitive.Speaker.FX.Play(FXSound.BEEP_1);
					BYOS.Instance.Primitive.Speaker.VolumeDown();
					break;

				case "VolumeUp":
					Primitive.Speaker.FX.Play(FXSound.BEEP_1);
					BYOS.Instance.Primitive.Speaker.VolumeUp();
					break;

				case "Wander":
					Say(Dictionary.GetString("wander"));
					Trigger("WANDER");
					//TODO, maybe ask for interaction instead if Buddy really wants to interact
					CompanionData.Instance.InteractDesire -= 10;
					if (CompanionData.Instance.MovingDesire < 50)
						CompanionData.Instance.MovingDesire = 50;

					Debug.Log("Start wanderring by voice");
					break;

				case "Weather":
					mNeedToGiveAnswer = true;
					break;

				case "LookAtMe":
					//mReaction.SearchFace();
					break;

				default:
					break;

			}

		}

		private void StartApp(string iAppName)
		{
			new StartAppCmd(iAppName).Execute();
			CompanionData.Instance.LastAppTime = Time.time;
			CompanionData.Instance.LastApp = iAppName;
		}

		private void Say(string iSpeech, bool iQueue = false)
		{
			mLastBuddySpeech = iSpeech;
			Interaction.TextToSpeech.Say(iSpeech, iQueue);
		}

		private void SayKey(string iSpeech, bool iQueue = false)
		{
			Say(Dictionary.GetRandomString(iSpeech), iQueue);
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