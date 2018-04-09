﻿using Buddy;
using Buddy.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
		private bool mMoving;
		private string mLastHumanSpeech;
		private bool mFirstErrorStt;
		private float mTimeHumanDetected;
		private float mTimeMotion;
		private string mPreviousOperationResult;

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
			// TODO remove this variable when resolved issue from core-2
			mFirstErrorStt = true;
			mLastHumanSpeech = "";
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "Vocal Triggered";
			Debug.Log("state: Vocal Triggered");

			mLastBuddySpeech = "";
			mTimeHumanDetected = 0F;
			mNeedToGiveAnswer = false;
			mError = false;
			mSpeechInput = false;
			mPreviousOperationResult = "";
			Interaction.VocalManager.EnableTrigger = false;
			Interaction.SpeechToText.OnBestRecognition.Add(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Add(ErrorSTT);
			mVocalChat.Activate();
			Interaction.VocalManager.EnableDefaultErrorHandling = false;
			mVocalChat.WithNotification = true;
			mVocalChat.OnQuestionTypeFound = SortQuestionType;

			mDetectionManager.StopSphinxTrigger();

			mNeedListen = true;
			mTime = 0F;
			mActionManager.StopAllBML();
			SayKey("ilisten");
		}


		void OnSpeechRecognition(string iText)
		{
			mLastHumanSpeech = iText;

			mState.text = "Vocal Triggered: reco " + iText;
			mError = false;
			mTime = 0F;
			mSpeechInput = true;
			Debug.Log("Reco vocal: " + iText);
			mVocalChat.SpecialRequest(iText);
			mFirstErrorStt = true;
		}

		void ErrorSTT(STTError iError)
		{
			if (mFirstErrorStt) {
				mFirstErrorStt = false;
				Debug.Log("Error STT ");

				mState.text = "Vocal Triggered: error " + iError.ToString();

				// To know if there is a connection issue
				if (iError == STTError.ERROR_NETWORK) {
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.BLINK_DOUBLE);
				}

				// If first error
				if (!mError) {
					mError = true;
					// Ask repeat
					SayKey("ilisten");
					mNeedListen = true;
				} else {
					// else go away
					Debug.Log("2cd error, go away");
					if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead)
						if (CompanionData.Instance.InteractDesire > 80 && CompanionData.Instance.MovingDesire < 22) {
							Trigger("LOOKINGFOR");
						} else {
							Trigger("WANDER");
						} else if (mActionManager.ThermalFollow)
						Trigger("FOLLOW");
					else if (mTimeHumanDetected < 5F)
						Trigger("INTERACT");
					else
						Trigger("IDLE");
				}
			}
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeHumanDetected += Time.deltaTime;

			switch (mDetectionManager.mDetectedElement) {

				case Detected.TOUCH:
					if (mDetectionManager.mFacePartTouched == FaceTouch.MOUTH) {
						mDetectionManager.mFacePartTouched = FaceTouch.NONE;
						mDetectionManager.mDetectedElement = Detected.NONE;
						mActionManager.StopAllActions();
						mNeedListen = true;
					}
					break;

				case Detected.THERMAL:
					mTimeHumanDetected = 0F;
					mDetectionManager.mDetectedElement = Detected.NONE;
					break;

				default:
					mDetectionManager.mDetectedElement = Detected.NONE;
					break;
			}


			mTime += Time.deltaTime;
			if (Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying && Interaction.SpeechToText.HasFinished) {
				if (!mVocalChat.BuildingAnswer && mNeedToGiveAnswer) {
					//Give answer:
					Debug.Log("give answer");
					Say(mVocalChat.Answer);
					mNeedToGiveAnswer = false;
					mFirstErrorStt = true;
				} else if (mMoving && !IsMoving() && Time.time - mTimeMotion > 3F) {
					Debug.Log("finished motion, need listen");
					mMoving = false;
					mNeedListen = true;
				} else if (mNeedListen && BYOS.Instance.Interaction.Face.IsStable && Interaction.SpeechToText.HasFinished) {
					Debug.Log("Vocal instant reco + mNeedListen: " + mNeedListen);

					//BYOS.Instance.Interaction.BMLManager.LaunchRandom("Listening");
					Interaction.VocalManager.StartInstantReco();
					mFirstErrorStt = true;
					mNeedListen = false;
					Debug.Log("Vocal instant reco 2 + mNeedListen: " + mNeedListen);
					mTime = 0F;
				} else if (!mVocalChat.BuildingAnswer && Interaction.VocalManager.RecognitionFinished && mTime > 15F && !mSpeechInput) {
					//Mb this was a wrong trigger, back to IDLE
					Debug.Log("Back to IDLE? ");
					if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead)
						Trigger("WANDER");
					else if (mActionManager.ThermalFollow && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
						Trigger("FOLLOW");
					else if (mTimeHumanDetected < 5F)
						Trigger("INTERACT");
					else
						Trigger("IDLE");
				} else {
					//Debug.Log("Why locked: building answer: " + mVocalChat.BuildingAnswer + " Reco finished: " + Interaction.VocalManager.RecognitionFinished + " mTime " + mTime + " speechInput: " + mSpeechInput);
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.Log("VOCAL TRIGGERED STATE EXIT");
			mNeedListen = true;
			mSpeechInput = false;
			Interaction.SpeechToText.OnBestRecognition.Remove(OnSpeechRecognition);
			Interaction.SpeechToText.OnErrorEnum.Remove(ErrorSTT);
			mVocalChat.DisActivate();
			mDetectionManager.StartSphinxTrigger();
			mDetectionManager.mDetectedElement = Detected.NONE;
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType)
		{




			Debug.Log("Question Type found : " + iType);
			string lSentence = "";
			if (iType != "Repeat")
				mLastBuddySpeech = "";
			if (iType != "Operation" && iType != "OperationAgain")
				mPreviousOperationResult = "";


			mSpeechInput = false;

			switch (iType) {




				case "Accept":
					Debug.Log("Accept VocalTrigger");
					SayKey("ilisten");
					mNeedListen = true;
					break;

				case "AccraAlarm":
					Debug.Log("Accra alarm triggered");
					StartApp("m777751028610", mLastHumanSpeech);
					break;

				case "Alarm":
					StartApp("Alarm", mLastHumanSpeech);
					break;

				case "Answer":
					if (string.IsNullOrEmpty(mVocalChat.Answer))
						if (mLastHumanSpeech.Contains("**")) {
							mActionManager.TimedMood(MoodType.GRUMPY);
							SayKey("badword");
						}
						// TODO: generate answer random
						else
							SayKey("noanswerfound");
					else
						Say(mVocalChat.Answer);

					mNeedListen = true;
					break;

				case "Babyphone":
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("BabyPhone", mLastHumanSpeech);
					break;

				case "Battery":
					//ces hack
					mNeedListen = true;
					Say("My battery is at " + (int)Primitive.Battery.EnergyLevel + " percent");
					break;

				case "BML":
					Debug.Log("Playing BML " + mVocalChat.Answer);
					if (string.IsNullOrEmpty(mVocalChat.Answer))
						Interaction.BMLManager.LaunchByName("AllIn");
					else if (!Interaction.BMLManager.LaunchByName(mVocalChat.Answer)) {
						if (!Interaction.BMLManager.LaunchRandom(mVocalChat.Answer))
							Say("I don't know the behaviour " + mVocalChat.Answer);

					}
					mNeedListen = true;
					break;

				case "BuddyLab":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("BuddyLab", mLastHumanSpeech);
					break;


				case "Calcul":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("PlayMath", mLastHumanSpeech);
					break;

				case "CanMove":
					SayKey("icanmove", true);
					CompanionData.Instance.MovingDesire += 20;
					mActionManager.UnlockAll();
					mNeedListen = true;
					break;


				case "Dance":
					Debug.Log("Playing BML dance");
					Interaction.BMLManager.LaunchRandom("dance");
					mNeedListen = true;
					break;

				case "DemoShort":
					Debug.Log("Playing BML demoShort");
					Interaction.BMLManager.LaunchByName("DemoShort");
					mNeedListen = true;
					break;

				case "DemoFull":
					Debug.Log("Playing BML demoFull");
					Interaction.BMLManager.LaunchByName("DemoFull");
					mNeedListen = true;
					break;

				case "Date":
					if (BYOS.Instance.Language.CurrentLang == Language.FR) {
						lSentence = Dictionary.GetRandomString("givedate").Replace("[weekday]", DateTime.Now.ToString("dddd", new CultureInfo("fr-FR")));
						lSentence = lSentence.Replace("[month]", "" + DateTime.Now.ToString("MMMM", new CultureInfo("fr-FR")));
					} else {
						lSentence = Dictionary.GetRandomString("givedate").Replace("[weekday]", DateTime.Now.ToString("dddd", new CultureInfo("en-US")));
						lSentence = lSentence.Replace("[month]", "" + DateTime.Now.ToString("MMMM", new CultureInfo("en-US")));
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
					SayKey("istopmoving", true);
					CompanionData.Instance.MovingDesire -= 20;
					mActionManager.LockWheels();
					mActionManager.WanderingMood = MoodType.NEUTRAL;
					mActionManager.WanderingOrder = false;
					mNeedListen = true;
					mActionManager.StopAllActions();
					break;

				case "DoSomething":
					Debug.Log("do something");
					if (UnityEngine.Random.Range(0, 2) == 0)
						RandomGame();
					else
						RandomBML();
					break;

				case "FollowMe":
					mActionManager.UnlockAll();
					if (!mActionManager.ThermalFollow) {
						CompanionData.Instance.InteractDesire -= 10;
						mActionManager.StartThermalFollow(HumanFollowType.BODY);
					}
					Interaction.TextToSpeech.SayKey("follow");
					Trigger("FOLLOW");
					break;

				case "FreezeDance":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("FreezeDance", mLastHumanSpeech);
					break;

				case "Guardian":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Guardian", mLastHumanSpeech);
					break;

				case "HeadDown": {
						CancelOrders();
						mActionManager.UnlockHead();

						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headdown").Replace("[degrees]", "" + n), true);

						Debug.Log("Head down " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition + (float)n;
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadUp": {
						CancelOrders();
						mActionManager.UnlockHead();

						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headup").Replace("[degrees]", "" + n), true);

						Debug.Log("Head up " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition - (float)n;
						//Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadLeft": {
						CancelOrders();
						mActionManager.UnlockHead();

						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 35;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headleft").Replace("[degrees]", "" + n), true);

						Debug.Log("Head left " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition + (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadRight": {
						CancelOrders();
						mActionManager.UnlockHead();

						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 35;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headright").Replace("[degrees]", "" + n), true);

						Debug.Log("Head right " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HideSeek":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("HideAndSeek", mLastHumanSpeech);
					break;

				//case "Hour":
				//	// HRI 2018
				//	if (BYOS.Instance.Language.CurrentLang == Language.FR) {
				//		lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
				//	} else {
				//		if (DateTime.Now.Hour < 13)
				//			lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour + " " + "am");
				//		else {
				//			lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + (DateTime.Now.Hour - 12) + " " + "pm");
				//		}
				//	}

				//

				//lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
				//lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
				//lSentence = lSentence.Replace("[second]", "" + DateTime.Now.Second);
				//Say(lSentence);
				//mNeedListen = true;
				//break;


				case "Hour":
					if (BYOS.Instance.Language.CurrentLang == Language.FR) {
						lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
						lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
					} else
						lSentence = GiveHourInEnglish();
					Say(lSentence);
					mNeedListen = true;
					break;


				case "IHateU":
					Debug.Log("Playing BML I hate u");
					Interaction.BMLManager.LaunchRandom("angry");
					mNeedListen = true;
					break;

				case "ILoveU":
					Debug.Log("Playing BML ILoveU");
					Interaction.BMLManager.LaunchRandom("love");
					mNeedListen = true;
					break;


				case "IOT":
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("Somfy", mLastHumanSpeech);
					break;

				case "Joke":
					Debug.Log("Playing BML joke");
					Interaction.BMLManager.LaunchRandom("joke");
					mNeedListen = true;
					break;

				case "Jukebox":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Jukebox", mLastHumanSpeech);
					break;

				case "MoveBackward": {
						CancelOrders();
						mActionManager.UnlockWheels();

						float n;
						//default value
						string nStr = "1";
						if (float.TryParse(mVocalChat.Answer, out n)) {
							nStr = mVocalChat.Answer;
						} else {
							n = 1F;
						}

						//Primitive.Motors.Wheels.MoveDistance(-150.0f, -150.0f, n, 0.02f);
						Dictionary<string, string> lParam = new Dictionary<string, string>();
						lParam.Add("MOVE_DISTANCE", nStr);

						Interaction.BMLManager.LaunchByName("MoveBackward", lParam);
						Debug.Log("MoveBackward: " + nStr);
						SayKey("accept", true);
						Say(Dictionary.GetRandomString("movebackward").Replace("[meters]", "" + nStr), true);
						mMoving = true;
						mTimeMotion = Time.time;
					}

					break;

				case "MoveForward": {
						CancelOrders();
						mActionManager.UnlockWheels();

						float n;
						//default value
						string nStr = "1";
						if (float.TryParse(mVocalChat.Answer, out n)) {
							nStr = mVocalChat.Answer;
						}

						//Primitive.Motors.Wheels.MoveDistance(-150.0f, -150.0f, n, 0.02f);

						Dictionary<string, string> lParam = new Dictionary<string, string>();
						lParam.Add("MOVE_DISTANCE", nStr);

						Interaction.BMLManager.LaunchByName("MoveForward", lParam);
						Debug.Log("MoveForward: " + nStr);
						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveforward").Replace("[meters]", "" + nStr), true);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "MoveLeft": {
						CancelOrders();
						mActionManager.UnlockWheels();

						int n = 0;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveleft").Replace("[degrees]", "" + n), true);

						Debug.Log("Move left " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.Wheels.TurnAngle((float)n, 200F, 0.02F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "MoveRight": {
						CancelOrders();
						mActionManager.UnlockWheels();

						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveright").Replace("[degrees]", "" + n), true);

						Debug.Log("Move right " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.Wheels.TurnAngle((float)-n, 200F, 0.02F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "Memory":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("MemoryGame", mLastHumanSpeech);
					break;

				case "Operation":


					Debug.Log("Operation");

					string lComputeOrder = mLastHumanSpeech.ToLower().Replace(BYOS.Instance.Dictionary.GetString("add"), "+");
					lComputeOrder = lComputeOrder.Replace("plus", "+");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("devide"), "/");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("devideit"), "/");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("devideper"), "/");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("minus"), "-");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("times"), "*");
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("power"), "^");



					mPreviousOperationResult = Compute(lComputeOrder).ToString();
					Say(Dictionary.GetRandomString("computeresult") + " " + mPreviousOperationResult);


					mState.text = "Vocal Triggered " + lComputeOrder + " = " + mPreviousOperationResult;

					mNeedListen = true;
					break;

				case "OperationAgain":

					// We add the current operation to the previous result

					Debug.Log("OperationAgain");
					string lComputeNewOrder = "";
					if (!string.IsNullOrEmpty(mPreviousOperationResult)) {
						lComputeNewOrder = mPreviousOperationResult + " ";



						lComputeNewOrder += mLastHumanSpeech.ToLower().Replace(BYOS.Instance.Dictionary.GetString("add"), "+");
						lComputeNewOrder = lComputeNewOrder.Replace("plus", "+");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("devide"), "/");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("devideit"), "/");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("devideper"), "/");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("minus"), "-");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("times"), "*");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("power"), "^");

						mPreviousOperationResult = Compute(lComputeNewOrder).ToString();
						Say(Dictionary.GetRandomString("computeresult") + " " + mPreviousOperationResult);


						mState.text = "Vocal Triggered " + lComputeNewOrder + " = " + mPreviousOperationResult;

					} else {
						// an error occured
						Say(Dictionary.GetRandomString("notunderstandcalcul"));
					}


					mNeedListen = true;

					break;


				case "Play":
					CompanionData.Instance.InteractDesire -= 30;

					Say("ok");
					Debug.Log("random game");
					RandomGame();
					break;

				case "Photo":
					CompanionData.Instance.InteractDesire -= 30;
					Debug.Log("starting app takephoto");
					StartApp("TakePhoto", mLastHumanSpeech);
					break;

				case "Pose":
					CompanionData.Instance.InteractDesire -= 30;
					StartApp("Take Pose", mLastHumanSpeech);
					break;

				case "Quit":
					if (mActionManager.ThermalFollow)
						Trigger("FOLLOW");
					else if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead)
						Trigger("WANDER");
					else
						Trigger("DISENGAGE");
					break;

				case "Quizz":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("QuizzGame", mLastHumanSpeech);
					break;

				case "Recipe":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Recipe", mLastHumanSpeech);
					break;

				case "Repeat":
					Interaction.TextToSpeech.SayKey("isaid", true);
					Interaction.TextToSpeech.Say("[500]", true);
					Interaction.TextToSpeech.Say(mLastBuddySpeech, true);
					mNeedListen = true;
					break;

				case "RLGL":
					CompanionData.Instance.InteractDesire -= 50;
					StartApp("RLGL", mLastHumanSpeech);
					break;

				case "Timer":
					Debug.Log("VocalTrigger Timer");
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("Timer", mLastHumanSpeech);
					break;


				case "Volume": {
						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value

							SayKey("getvolumeerror");
						}
						Say(Dictionary.GetRandomString("volume") + " " + n, true);

						mNeedListen = true;
					}
					break;

				case "VolumeDown": {
						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 3;
						}

						SayKey("accept", true);

						Debug.Log("Decrease volume by " + n);
						Primitive.Speaker.FX.Play(FXSound.BEEP_1);
						BYOS.Instance.Primitive.Speaker.VolumeDown(n);
						Say(Dictionary.GetRandomString("volumedown") + " " + n, true);
						mNeedListen = true;
					}
					break;

				case "VolumeUp": {
						int n;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 3;
						}
						SayKey("accept", true);
						Debug.Log("Increase volume by " + n);
						Primitive.Speaker.FX.Play(FXSound.BEEP_1);
						BYOS.Instance.Primitive.Speaker.VolumeUp(n);
						Say(Dictionary.GetRandomString("volumeup") + " " + n);
						mNeedListen = true;
					}
					break;

				case "Wander":
					mActionManager.UnlockAll();
					SayKey("wander");
					//TODO, maybe ask for interaction instead if Buddy really wants to interact
					CompanionData.Instance.InteractDesire -= 10;
					if (CompanionData.Instance.MovingDesire < 40)
						CompanionData.Instance.MovingDesire = 40;

					Debug.Log("Start wanderring by voice " + mVocalChat.Answer);
					mActionManager.WanderingOrder = true;
					mActionManager.WanderingMood = (MoodType)Enum.Parse(typeof(MoodType), mVocalChat.Answer, true);
					Trigger("WANDER");
					break;

				case "Weather":
					Debug.Log("VocalTrigger Weather");
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("Weather", mLastHumanSpeech);
					break;

				case "Welcome":
					Debug.Log("Playing BML Welcome");
					Interaction.BMLManager.LaunchByName("Welcome");
					mNeedListen = true;
					break;

				case "LookAtMe":
					//mReaction.SearchFace();
					mNeedListen = true;
					break;

				default:
					break;

			}

		}


		/// FUNCTIONS ///

		/// <summary>
		/// Give The Hour in English
		/// </summary>
		private string GiveHourInEnglish()
		{
			string lSentence = Dictionary.GetRandomString("givehour");

			if (lSentence.Contains("[Hour]"))
				lSentence = GiveHour(lSentence);
			else
				lSentence = GiveSpokenHour(lSentence);
			return (lSentence);
		}

		/// <summary>
		/// Harder way to tell what time it is
		/// </summary>
		/// <param name="iSentence">Sentence to say</param>
		/// <returns>Sentence to say</returns>
		private string GiveSpokenHour(string iSentence)
		{
			if (DateTime.Now.ToString("%h").Contains("12") && DateTime.Now.ToString("mm").Contains("00")) {
				iSentence = iSentence.Replace("[hour]", "Noon");
				iSentence = iSentence.Replace("[minutes]", string.Empty);
			} else if (DateTime.Now.ToString("%h").Contains("00") && DateTime.Now.ToString("mm").Contains("00")) {
				iSentence = iSentence.Replace("[hour]", "Midnight");
				iSentence = iSentence.Replace("[minutes]", string.Empty);
			} else if (DateTime.Now.ToString("mm").Contains("00")) {
				iSentence = iSentence.Replace("[minutes]", DateTime.Now.Hour.ToString());
				iSentence = iSentence.Replace("[hour]", "o'clock");
			} else if (Convert.ToInt32(DateTime.Now.ToString("mm")) > 0 && Convert.ToInt32(DateTime.Now.ToString("mm")) < 30) {
				if (DateTime.Now.ToString("mm").Contains("15"))
					iSentence = iSentence.Replace("[minutes]", "a quarter past");
				else
					iSentence = iSentence.Replace("[minutes]", DateTime.Now.ToString("mm") + " past");
				iSentence = iSentence.Replace("[hour]", DateTime.Now.Hour.ToString());
			} else if (Convert.ToInt32(DateTime.Now.ToString("mm")) == 30) {
				iSentence = iSentence.Replace("[minutes]", "half past");
				iSentence = iSentence.Replace("[hour]", DateTime.Now.Hour.ToString());
			} else if (Convert.ToInt32(DateTime.Now.ToString("mm")) > 30 && Convert.ToInt32(DateTime.Now.ToString("mm")) <= 59) {
				if (DateTime.Now.ToString("mm").Contains("45"))
					iSentence = iSentence.Replace("[minutes]", "a quarter to");
				else {
					int lTime = 60 - Convert.ToInt32(DateTime.Now.ToString("mm"));
					iSentence = iSentence.Replace("[minutes]", lTime + " to");
				}
				int lResult = Convert.ToInt32(DateTime.Now.ToString("%h")) + 1;
				iSentence = iSentence.Replace("[hour]", lResult.ToString());
			}
			return (iSentence);
		}

		/// <summary>
		///  Simple way to tell what time it is
		/// </summary>
		/// <param name="iSentence">Sentence to say</param>
		/// <returns>Sentence to say</returns>
		private string GiveHour(string iSentence)
		{
			int lTime = 0;
			if (DateTime.Now.Hour > 12) {
				lTime = DateTime.Now.Hour - 12;
				iSentence = iSentence.Replace("[Meridiem]", "pm");
			} else {
				lTime = DateTime.Now.Hour;
				iSentence = iSentence.Replace("[Meridiem]", "am");
			}
			iSentence = iSentence.Replace("[Hour]", lTime.ToString());
			if (Convert.ToInt32(DateTime.Now.Minute.ToString()) != 0)
				iSentence = iSentence.Replace("[Minutes]", DateTime.Now.Minute.ToString());
			else
				iSentence = iSentence.Replace("[Minutes]", string.Empty);
			return (iSentence);
		}


		private void RandomBML()
		{
			int i = UnityEngine.Random.Range(1, 7);

			switch (i) {
				case 1:
					Interaction.BMLManager.LaunchRandom("dance");
					break;
				case 2:
					Interaction.BMLManager.LaunchRandom("other");
					break;
				case 3:
					Interaction.BMLManager.LaunchRandom("happy");
					break;
				case 4:
					Interaction.BMLManager.LaunchRandom("joy");
					break;
				default:
					Interaction.BMLManager.LaunchRandom("joke");
					break;
			}
		}

		private void RandomGame()
		{
			int i = UnityEngine.Random.Range(1, 5);

			switch (i) {
				case 1:
					StartApp("PlayMath", mLastHumanSpeech);
					break;
				case 2:
					StartApp("FreezeDance", mLastHumanSpeech);
					break;
				case 3:
					StartApp("MemoryGame", mLastHumanSpeech);
					break;
				case 4:
					StartApp("RLGL", mLastHumanSpeech);
					break;
				default:
					StartApp("Weather", mLastHumanSpeech);
					break;
			}
		}

		private double Compute(string iSpeech)
		{
			string lSpeech = iSpeech.Trim();
			//lSpeech = lSpeech.Replace("x", "*");
			//lSpeech = lSpeech.Replace("÷", "/");

			//if (lSpeech.Contains("√")) {
			//	lSpeech = lSpeech.Replace("√", "sqrt");
			//	lSpeech = Regex.Replace(lSpeech, @"\d", "($0)").Replace("sqrt ", "sqrt");
			//}

			string pattern = @"(\s?)(\d+\.?((?<=\.)\d+)?)";
			Regex rgx = new Regex(pattern);
			lSpeech = rgx.Replace(lSpeech, "($2)");
			var parser = new ExpressionParser();

			Expression exp = parser.EvaluateExpression(lSpeech);
			Debug.Log("Result: " + exp.Value);  // prints: "Result: 522"

			return exp.Value;
		}

		private void CancelOrders()
		{
			mActionManager.WanderingMood = MoodType.NEUTRAL;
			mActionManager.WanderingOrder = false;
			mActionManager.StopAllActions();
		}

		private void StartApp(string iAppName, string iSpeech = null)
		{
			CancelOrders();
			Debug.Log("start app " + iAppName + "with param " + iSpeech);
			CompanionData.Instance.LastAppTime = Time.time;
			CompanionData.Instance.LastApp = iAppName;
			//new StartAppCmd(iAppName).Execute();
			new StartAppCmd(iAppName, new int[] { }, new float[] { }, new string[] { iSpeech }).Execute();

			mNeedListen = true;
		}

		private bool IsMoving()
		{
			return Primitive.Motors.Wheels.Status == MovingState.MOVING || Math.Abs(Primitive.Motors.YesHinge.DestinationAnglePosition - Primitive.Motors.YesHinge.CurrentAnglePosition) > 5F || Math.Abs(Primitive.Motors.NoHinge.DestinationAnglePosition - Primitive.Motors.NoHinge.CurrentAnglePosition) > 5F;
		}

		private void Say(string iSpeech, bool iQueue = false)
		{
			mLastBuddySpeech += iSpeech;
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