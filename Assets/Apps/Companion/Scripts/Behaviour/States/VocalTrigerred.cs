using Buddy;
using Buddy.Command;
using System;
using System.Collections.Generic;
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
		private bool mLaunchingApp;
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

			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;

			// TODO remove this variable when resolved issue from core-2
			mFirstErrorStt = true;
			mLaunchingApp = false;
			mLastHumanSpeech = "";
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "Vocal Triggered";
			Debug.Log("state: Vocal Triggered");

			mLastBuddySpeech = "";
			mTimeHumanDetected = 0F;
			mNeedToGiveAnswer = false;
			mError = false;
			mSpeechInput = false;
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
			Say(Dictionary.GetString("ilisten"));
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
						if (CompanionData.Instance.mInteractDesire > 80 && CompanionData.Instance.mMovingDesire < 22) {
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
			if (Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying) {
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

				// When launching app failed
			} else if (!Interaction.TextToSpeech.HasFinishedTalking && mLaunchingApp) {
				mLaunchingApp = false;
				mNeedListen = true;
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
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType)
		{




			Debug.Log("Question Type found : " + iType);
			string lSentence = "";
			if (iType != "Repeat")
				mLastBuddySpeech = "";


			mSpeechInput = false;

			switch (iType) {

				case "Accept":
					Debug.Log("Accept VocalTrigger");
					SayKey("ilisten");
					mNeedListen = true;
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
					CompanionData.Instance.mInteractDesire -= 10;
					StartApp("BabyPhone", mLastHumanSpeech);
					break;

				case "Battery":
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("informbattery")
						.Replace("[batterylevel]", BYOS.Instance.Primitive.Battery.EnergyLevel.ToString()));
					mNeedListen = true;
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
					CompanionData.Instance.mInteractDesire -= 20;
					StartApp("BuddyLab", mLastHumanSpeech);
					break;


				case "Calcul":
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("PlayMath", mLastHumanSpeech);
					break;

				case "CanMove":
					if (CompanionData.Instance.mMovingDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 3, "moodcanmove", "CAN_MOVE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.EXCITED));

					else
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 1, "moodcanmove", "CAN_MOVE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));

					mActionManager.UnlockAll();
					mNeedListen = true;
					break;


				case "Dance":
					Debug.Log("Playing BML dance");

					if (CompanionData.Instance.mMovingDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 3, "mooddance", "DANCE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.EXCITED));

					else {
						mActionManager.TimedMood(MoodType.GRUMPY);
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 0, "moodforceddance", "DANCE", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.SALTY));
					}

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

					if (CompanionData.Instance.mMovingDesire > 20) {
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 0, "moodcantmove", "CANT_MOVE", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.SALTY));
						mActionManager.TimedMood(MoodType.GRUMPY);

					}


					CompanionData.Instance.mMovingDesire -= 20;
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

					if (CompanionData.Instance.mInteractDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, 0, "moodfollow", "FOLLOW_ORDER", EmotionalEventType.FULFILLED_DESIRE, InternalMood.HAPPY));


					if (!mActionManager.ThermalFollow) {
						CompanionData.Instance.mInteractDesire -= 10;
						mActionManager.StartThermalFollow(HumanFollowType.BODY);
					}
					Interaction.TextToSpeech.SayKey("follow");
					Trigger("FOLLOW");
					break;

				case "FreezeDance":
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("FreezeDance", mLastHumanSpeech);
					break;

				case "Guardian":
					CompanionData.Instance.mInteractDesire -= 20;
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
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("HideAndSeek", mLastHumanSpeech);
					break;

				case "Hour":
					lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
					lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
					lSentence = lSentence.Replace("[second]", "" + DateTime.Now.Second);
					Say(lSentence);
					mNeedListen = true;
					break;

				case "IOT":
					CompanionData.Instance.mInteractDesire -= 10;
					StartApp("Somfy", mLastHumanSpeech);
					break;

				case "Joke":
					Trigger("TELLJOKE");
					break;

				case "ListenJoke":
					Interaction.TextToSpeech.SayKey("ilisten");
					Trigger("LISTENJOKE");
					break;

				case "Jukebox":
					StartApp("Jukebox", mLastHumanSpeech);
					break;

				case "Mood":
					EmotionalEvent lEventMood = Interaction.InternalState.ExplainMood();
					if (lEventMood == null) {
						Say(Dictionary.GetRandomString("nomood"));
					} else {
						Debug.Log("key: " + Interaction.InternalState.ExplainMood().ExplanationKey + " dico value: " + Dictionary.GetRandomString(Interaction.InternalState.ExplainMood().ExplanationKey));
						mActionManager.ShowInternalMood();
						Say(Dictionary.GetRandomString("ifeel") + " " + Dictionary.GetString(Interaction.InternalState.InternalStateMood.ToString().ToLower()) + " "
							+ Dictionary.GetRandomString("because") + " " + Dictionary.GetRandomString(Interaction.InternalState.ExplainMood().ExplanationKey), true);

					}

					mNeedListen = true;
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
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("MemoryGame", mLastHumanSpeech);
					break;

				case "Play":
					CompanionData.Instance.mInteractDesire -= 30;

					Say("ok");
					Debug.Log("random game");
					RandomGame();
					break;

				case "Photo":
					CompanionData.Instance.mInteractDesire -= 30;
					Debug.Log("starting app takephoto");
					StartApp("TakePhoto", mLastHumanSpeech);
					break;

				case "Pose":
					CompanionData.Instance.mInteractDesire -= 30;
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
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("QuizzGame", mLastHumanSpeech);
					break;

				case "Recipe":
					CompanionData.Instance.mInteractDesire -= 20;
					StartApp("Recipe", mLastHumanSpeech);
					break;

				case "Repeat":
					Interaction.TextToSpeech.SayKey("isaid", true);
					Interaction.TextToSpeech.Say("[500]", true);
					Interaction.TextToSpeech.Say(mLastBuddySpeech, true);
					mNeedListen = true;
					break;

				case "RLGL":
					CompanionData.Instance.mInteractDesire -= 50;
					StartApp("RLGL", mLastHumanSpeech);
					break;

				case "Timer":
					Debug.Log("VocalTrigger Timer");
					CompanionData.Instance.mInteractDesire -= 10;
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
					CompanionData.Instance.mInteractDesire -= 10;
					if (CompanionData.Instance.mMovingDesire < 40)
						CompanionData.Instance.mMovingDesire = 40;

					Debug.Log("Start wanderring by voice " + mVocalChat.Answer);
					mActionManager.WanderingOrder = true;
					mActionManager.WanderingMood = (MoodType)Enum.Parse(typeof(MoodType), mVocalChat.Answer, true);
					Trigger("WANDER");
					break;

				case "Weather":
					Debug.Log("VocalTrigger Weather");
					CompanionData.Instance.mInteractDesire -= 10;
					StartApp("Weather", mLastHumanSpeech);
					break;

				case "LookAtMe":
					//mReaction.SearchFace();
					mNeedListen = true;
					break;

				default:
					break;

			}

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

		private void CancelOrders()
		{
			mActionManager.WanderingMood = MoodType.NEUTRAL;
			mActionManager.WanderingOrder = false;
			mActionManager.StopAllActions();
		}

		private void StartApp(string iAppName, string iSpeech = null, bool iLandingTrigg = false)
		{

			CancelOrders();
			Debug.Log("start app " + iAppName + "with param " + iSpeech);
			CompanionData.Instance.LastAppTime = DateTime.Now;
			CompanionData.Instance.LastApp = iAppName;
			CompanionData.Instance.LandingTrigger = iLandingTrigg;
			//new StartAppCmd(iAppName).Execute();
			new StartAppCmd(iAppName, new int[] { }, new float[] { }, new string[] { iSpeech }).Execute();
			mLaunchingApp = true;
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
			for (int i = 0; i < iListSpeech.Length; ++i) {
				string[] words = iListSpeech[i].Split(' ');
				if (words.Length < 2 && !string.IsNullOrEmpty(words[0])) {
					if (words[0].ToLower() == iSpeech.ToLower()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;

		}

	}
}