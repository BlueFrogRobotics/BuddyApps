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
			BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();
			SayKey("ilisten");
		}


		void OnSpeechRecognition(string iText)
		{
			mLastHumanSpeech = iText;

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
				// If first error
				if (!mError) {
					mError = true;
					// Ask repeat
					SayKey("ilisten");
					mNeedListen = true;
				} else {
					// else go away
					Debug.Log("2cd error, go away");
					if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
						Trigger("WANDER");
					else if (mActionManager.ThermalFollow)
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
				} else if (mNeedListen) {
					Debug.Log("Vocal instant reco");

					//BYOS.Instance.Interaction.BMLManager.LaunchRandom("Listening");
					Interaction.VocalManager.StartInstantReco();
					mFirstErrorStt = true;
					mNeedListen = false;
					mTime = 0F;
				} else if (!mVocalChat.BuildingAnswer && Interaction.VocalManager.RecognitionFinished && mTime > 15F && !mSpeechInput) {
					//Mb this was a wrong trigger, back to IDLE
					Debug.Log("Back to IDLE? ");
					if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
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
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("BabyPhone", mLastHumanSpeech);
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
					StartApp("CalculGame", mLastHumanSpeech);
					break;

				case "CanMove":
					mActionManager.UnlockAll();
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

				case "HeadUp":
					{
						CancelOrders();
						mActionManager.UnlockHead();

						int n = 0;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headup").Replace("[degrees]", "" + n), true);

						Debug.Log("Head up " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
                    }
					break;

				case "HeadLeft":
					{
						CancelOrders();
						mActionManager.UnlockHead();

						int n = 0;
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

				case "HeadRight":
					{
						CancelOrders();
						mActionManager.UnlockHead();

						int n = 0;
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

				case "HeadDown":
					{
						CancelOrders();
						mActionManager.UnlockHead();

						int n = 0;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value
							n = 25;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headdown").Replace("[degrees]", "" + n), true);

						Debug.Log("Head down " + n + " degrees + VocalChat.Answer: " + mVocalChat.Answer);
						Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition + (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HideSeek":
					CompanionData.Instance.InteractDesire -= 50;
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
					CompanionData.Instance.InteractDesire -= 10;
					StartApp("Somfy", mLastHumanSpeech);
					break;

				case "Jukebox":
					CompanionData.Instance.InteractDesire -= 20;
					StartApp("Jukebox", mLastHumanSpeech);
					break;

				case "MoveBackward":
					{
						CancelOrders();
						mActionManager.UnlockWheels();

						float n = 1F;
						//default value
						string nStr = "1";
						if (float.TryParse(mVocalChat.Answer, out n)) {
							nStr = mVocalChat.Answer;
						}

						Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!MoveBackward: launch command" );
						Primitive.Motors.Wheels.MoveDistance(-150.0f, -150.0f, n, 0.02f);
						//Dictionary<string, string> mySmallDic = new Dictionary<string, string>();
						//mySmallDic["MOVE_DISTANCE"] = nStr;
						//Interaction.BMLManager.LaunchByName("MoveBackward", mySmallDic);
						Debug.Log("MoveBackward: " + nStr);
						SayKey("accept", true);
						Say(Dictionary.GetRandomString("movebackward").Replace("[meters]", "" + nStr), true);
						mMoving = true;
						mTimeMotion = Time.time;
					}

					break;

				case "MoveForward":
					{
						CancelOrders();
						mActionManager.UnlockWheels();

						float n = 1F;
						//default value
						string nStr = "1";
						if (float.TryParse(mVocalChat.Answer, out n)) {
							nStr = mVocalChat.Answer;
						}

						//Primitive.Motors.Wheels.MoveDistance(-150.0f, -150.0f, n, 0.02f);

						Dictionary<string, string> param = new Dictionary<string, string>();
						param.Add("MOVE_DISTANCE", nStr);

						Interaction.BMLManager.LaunchByName("MoveForward", param);

						//Dictionary<string, string> mySmallDic = new Dictionary<string, string>();
						//mySmallDic["MOVE_DISTANCE"] = nStr;
						//Debug.Log("Move forward bml: " + Interaction.BMLManager.LaunchRandom("move", mySmallDic));
						Debug.Log("MoveForward: " + nStr);
						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveforward").Replace("[meters]", "" + nStr), true);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "MoveLeft":
					{
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

				case "MoveRight":
					{
						CancelOrders();
						mActionManager.UnlockWheels();

						int n = 0;
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
					else if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody)
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


				case "Volume":
					{
						int n = 0;
						if (!int.TryParse(mVocalChat.Answer, out n)) {
							//default value

							SayKey("getvolumeerror");
						}
						Say(Dictionary.GetRandomString("volume") + " " + n, true);

						mNeedListen = true;
					}
					break;

				case "VolumeDown":
					{
						int n = 0;
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

				case "VolumeUp":
					{
						int n = 0;
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
					if (CompanionData.Instance.MovingDesire < 50)
						CompanionData.Instance.MovingDesire = 50;

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

				case "LookAtMe":
					//mReaction.SearchFace();
					mNeedListen = true;
					break;

				default:
					break;

			}

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