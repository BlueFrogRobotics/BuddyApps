using Buddy;
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
		//private VocalHelper mVocalChat;
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
			//mVocalChat = GetComponent<VocalHelper>();
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}


		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mActionManager.CurrentAction = BUDDY_ACTION.CHAT;

			mLaunchingApp = false;
			mDetectionManager.mDetectedElement = Detected.NONE;
			mDetectionManager.mFacePartTouched = FaceTouch.NONE;
			mState.text = "Vocal Triggered";
			Debug.Log("state : Vocal Triggered");

			mTimeHumanDetected = 0F;

			mDetectionManager.StopSphinxTrigger();

			InitVocal();

			mNeedListen = true;
			mTime = 0F;
			mActionManager.StopAllBML();
			if (mDetectionManager.ActiveReminders.Count == 0) {
				Say(Dictionary.GetString("ilisten"));
			}
		}

		private void InitVocal()
		{

			// TODO remove this variable when resolved issue from core-2
			mFirstErrorStt = true;
			mLastHumanSpeech = "";
			mLastBuddySpeech = "";
			mNeedToGiveAnswer = false;
			mError = false;
			mSpeechInput = false;

			Interaction.VocalManager.EnableTrigger = false;
			Interaction.VocalManager.UseVocon = true;
			Interaction.VocalManager.AddGrammar("companion_commands", LoadContext.APP);
			Interaction.VocalManager.AddGrammar("companion_questions", LoadContext.APP);
			Interaction.VocalManager.OnVoconBest = OnSpeechRecognition;
			Interaction.VocalManager.OnVoconEvent = EventVocon;

			//           //Interaction.VocalManager.OnEndReco = GetAnswer;
			Interaction.VocalManager.OnError = ErrorSTT;
			Interaction.VocalManager.EnableDefaultErrorHandling = false;
		}

		private void EventVocon(VoconEvent iEvent)
		{
			mState.text = "Vocal Triggered " + iEvent.ToString();
			Debug.Log("Vocal triggered vocon event:" + iEvent.ToString());
		}

		void OnSpeechRecognition(VoconResult iBestResult)
		{
			// TODO: remove fix after vocon multi callback call fix
			if (Interaction.TextToSpeech.IsSpeaking) {
				if (string.IsNullOrEmpty(iBestResult.Utterance))
					ErrorSTT(STTError.ERROR_NO_MATCH);
				else {

					mLastHumanSpeech = iBestResult.Utterance;

					mState.text = "Vocal Triggered: reco " + mLastHumanSpeech;
					mError = false;
					mTime = 0F;
					mSpeechInput = true;
					Debug.Log("Reco vocal: " + mLastHumanSpeech);

					SortQuestionType(GetRealStartRule(iBestResult.StartRule), iBestResult.Utterance);

					mFirstErrorStt = true;
				}
			}
		}

		void ErrorSTT(STTError iError)
		{
			if (mFirstErrorStt) {
				mFirstErrorStt = false;
				Debug.Log("Error STT " + iError.ToString());

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
						Interaction.TextToSpeech.Stop();
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
			if (Interaction.TextToSpeech.HasFinishedTalking && Interaction.BMLManager.DonePlaying && Interaction.VocalManager.RecognitionFinished) {
				/*if (!mVocalChat.BuildingAnswer && mNeedToGiveAnswer) {
					//Give answer:
					Debug.Log("give answer");
					Say(mVocalChat.Answer);
					mNeedToGiveAnswer = false;
					mFirstErrorStt = true;
				} else */
				if (mMoving && !IsMoving() && Time.time - mTimeMotion > 3F) {
					Debug.Log("finished motion, need listen");
					mMoving = false;
					mNeedListen = true;
				} else if (mNeedListen && BYOS.Instance.Interaction.Face.IsStable && Interaction.VocalManager.RecognitionFinished) {
					Debug.Log("Vocal instant reco + mNeedListen: " + mNeedListen);

					// If we need to listen, it means we already answered.
					// We can tell a notification if needed:
					if (mDetectionManager.ActiveReminders.Count > 0) {
						mActionManager.TellNotif(mDetectionManager.ActiveReminders[0]);
					}



					//BYOS.Instance.Interaction.BMLManager.LaunchRandom("Listening");
					Interaction.VocalManager.StartInstantReco();
					mFirstErrorStt = true;
					mNeedListen = false;
					Debug.Log("Vocal instant reco 2 + mNeedListen: " + mNeedListen);
					mTime = 0F;
				} else if (/*!mVocalChat.BuildingAnswer &&*/ Interaction.VocalManager.RecognitionFinished && mTime > 15F && !mSpeechInput) {
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
				Debug.Log("Launching app failed");
				mLaunchingApp = false;
				mNeedListen = true;
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.Log("VOCAL TRIGGERED STATE EXIT");
			mNeedListen = true;
			mSpeechInput = false;

			Interaction.VocalManager.RemoveGrammar("companion_commands", LoadContext.APP);
			Interaction.VocalManager.RemoveGrammar("companion_questions", LoadContext.APP);
			mDetectionManager.StartSphinxTrigger();

			Interaction.VocalManager.UseVocon = false;
			Interaction.VocalManager.OnError = null;
			Interaction.VocalManager.OnVoconBest = null;
			Interaction.VocalManager.OnVoconEvent = null;
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

		//Sort the type of the question returned by the Vocal Chat.
		//It either corresponds to orders on movement or launch applications
		private void SortQuestionType(string iType, string iUtterance)
		{
			iType = char.ToUpper(iType[0]) + iType.Substring(1);
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
					mActionManager.StartApp("Alarm", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				//case "Answer":
				//	if (string.IsNullOrEmpty(mVocalChat.Answer))
				//		if (mLastHumanSpeech.Contains("**")) {
				//			mActionManager.TimedMood(MoodType.GRUMPY);
				//			SayKey("badword");
				//		}
				//		// TODO: generate answer random
				//		else
				//			SayKey("noanswerfound");
				//	else
				//		Say(mVocalChat.Answer);

				//	mNeedListen = true;
				//	break;

				case "Babyphone":
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("BabyPhone", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Battery":
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("informbattery")
						.Replace("[batterylevel]", BYOS.Instance.Primitive.Battery.EnergyLevel.ToString()));
					mNeedListen = true;
					break;

				//case "BML":
				//	Debug.Log("Playing BML " + mVocalChat.Answer);
				//	if (string.IsNullOrEmpty(mVocalChat.Answer))
				//		Interaction.BMLManager.LaunchByName("AllIn");
				//	else if (!Interaction.BMLManager.LaunchByName(mVocalChat.Answer)) {
				//		if (!Interaction.BMLManager.LaunchRandom(mVocalChat.Answer))
				//			Say("I don't know the behaviour " + mVocalChat.Answer);

				//	}
				//	mNeedListen = true;
				//	break;

				case "BuddyLab":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("BuddyLab", mLastHumanSpeech);
					mLaunchingApp = true;
					break;


				case "Calcul":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("PlayMath", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "CanMove":


					SayKey("icanmove", true);

					if (CompanionData.Instance.mMovingDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 3, "moodcanmove", "CAN_MOVE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.EXCITED));

					else
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 1, "moodcanmove", "CAN_MOVE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));

					mActionManager.UnlockAll();
					mNeedListen = true;
					break;

				case "ColourSeen":
					Trigger("COLOUR");
					break;

				case "Connection":
					Trigger("CONNECTION");
					break;

				case "Copy":
					lSentence = Dictionary.GetRandomString("copy");
					Say(lSentence);
					Trigger("COPY");
					break;

				case "DetectObject":
					if (BYOS.Instance.Perception.Obstacle.Detect().Length != 0)
						lSentence = BYOS.Instance.Dictionary.GetRandomString("seesomething");
					else
						lSentence = BYOS.Instance.Dictionary.GetRandomString("dontseeanything");
					Say(lSentence);
					mNeedListen = true;
					break;
				case "Dance":
					Debug.Log("Playing BML dance");

					if (CompanionData.Instance.mMovingDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 3, "mooddance", "DANCE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.EXCITED));

					else {
						mActionManager.TimedMood(MoodType.GRUMPY);
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 0, "moodforceddance", "DANCE", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.BITTER));
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
					Debug.Log("Playing BML  demoFull");
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
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-2, 0, "moodcantmove", "CANT_MOVE", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.BITTER));
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
					mActionManager.StartApp("FreezeDance", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Guardian":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("Guardian", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "HeadDown": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headdown").Replace("[degrees]", "" + n), true);

						Debug.Log("Head down " + n + " degrees + VocalChat.Answer: " + n);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition + n;
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadUp": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headup").Replace("[degrees]", "" + n), true);

						Debug.Log("Head up " + n + " degrees + VocalChat.Answer: " + n);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition - n;
						//Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadLeft": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 35F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headleft").Replace("[degrees]", "" + n), true);

						Debug.Log("Head left " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition + n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "HeadRight": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();


						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 35F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headright").Replace("[degrees]", "" + n), true);

						Debug.Log("Head right " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "Heat":
					int[] lMatrix = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
					int lMax = RetrieveMaxInt(lMatrix);
					lSentence = Dictionary.GetRandomString("heat").Replace("[degree]", lMax.ToString());
					Say(lSentence);
					mNeedListen = true;
					break;


				case "HideSeek":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("HideAndSeek", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Hour":
					// HRI 2018
					if (BYOS.Instance.Language.CurrentLang == Language.FR) {
						lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
					} else {
						if (DateTime.Now.Hour < 13)
							lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour + " " + "am");
						else {
							lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + (DateTime.Now.Hour - 12) + " " + "pm");
						}
					}

					//

					//lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
					lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
					lSentence = lSentence.Replace("[second]", "" + DateTime.Now.Second);
					Say(lSentence);
					mNeedListen = true;
					break;

				case "IOT":
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Somfy", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Joke":
					Trigger("TELLJOKE");
					break;

				case "ListenJoke":
					if (ContainsOneOf(mLastHumanSpeech, "knockknock"))
						Interaction.TextToSpeech.SayKey("whoisthere");
					else
						Interaction.TextToSpeech.SayKey("ilisten");
					Trigger("LISTENJOKE");
					break;

				case "Jukebox":
					mActionManager.StartApp("Jukebox", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Mirror":
					int time = GetTime(mLastHumanSpeech);

					SetInteger("Duration", time);

					Trigger("MIRROR");
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
						mActionManager.CancelOrders();
						mActionManager.UnlockWheels();

						float n;
						//default value
						string nStr = GetStringNextNumber(iUtterance);
						if (!float.TryParse(nStr, out n)) {
							// If no float, default value
							nStr = "1";
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
						mActionManager.CancelOrders();
						mActionManager.UnlockWheels();

						float n;
						//default value
						string nStr = GetStringNextNumber(iUtterance);
						if (!float.TryParse(nStr, out n)) {
							// If no float, default value
							nStr = "1";
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
						mActionManager.CancelOrders();
						mActionManager.UnlockWheels();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveleft").Replace("[degrees]", "" + n), true);

						Debug.Log("Move left " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.Wheels.TurnAngle((float)n, 200F, 0.02F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "MoveRight": {
						mActionManager.CancelOrders();
						mActionManager.UnlockWheels();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("moveright").Replace("[degrees]", "" + n), true);

						Debug.Log("Move right " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.Wheels.TurnAngle((float)-n, 200F, 0.02F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "Memory":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("MemoryGame", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Nap":
					int lTimeInSeconds = GetTime(mLastHumanSpeech);

					if (lTimeInSeconds == 0)
						if (ContainsOneOf(mLastHumanSpeech, Dictionary.GetPhoneticStrings("aquarterhour")))
							lTimeInSeconds = 15 * 60;
						else if (ContainsOneOf(mLastHumanSpeech, Dictionary.GetPhoneticStrings("halfhour")))
							lTimeInSeconds = 30 * 60;
						else if (ContainsOneOf(mLastHumanSpeech, Dictionary.GetPhoneticStrings("threequarterhour")))
							lTimeInSeconds = 45 * 60;
						else
							// If Time is not specified
							lSentence = Dictionary.GetRandomString("nap");
					else {
						int[] lTime = SecondsToHour(lTimeInSeconds);

						// Get the correct sentence
						lSentence = Dictionary.GetRandomString("naptime");
						lSentence = SetNapSentence(lTime[0], "hour", lSentence);
						lSentence = SetNapSentence(lTime[1], "minute", lSentence);
						lSentence = SetNapSentence(lTime[2], "second", lSentence);
					}

					Say(lSentence);
					SetInteger("Duration", lTimeInSeconds);

					Trigger("NAP");
					break;

				case "Operation":
					Say(Dictionary.GetRandomString("computeresult") + " " + Compute(mLastHumanSpeech).ToString());
					mNeedListen = true;
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
					mActionManager.StartApp("TakePhoto", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Pose":
					CompanionData.Instance.mInteractDesire -= 30;
					mActionManager.StartApp("Take Pose", mLastHumanSpeech);
					mLaunchingApp = true;
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
					mActionManager.StartApp("QuizzGame", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Recipe":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("Recipe", mLastHumanSpeech);
					mLaunchingApp = true;
					break;


				case "Reminder":
					CompanionData.Instance.mHelpDesire -= 20;
					mActionManager.StartApp("Reminder", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Repeat":
					Interaction.TextToSpeech.SayKey("isaid", true);
					Interaction.TextToSpeech.Say("[500]", true);
					Interaction.TextToSpeech.Say(mLastBuddySpeech, true);
					mNeedListen = true;
					break;

				case "RLGL":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("RLGL", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				//case "SwitchLanguage":
				//	// TODO get command from OS
				//	if (string.IsNullOrEmpty(mVocalChat.Answer)) {
				//		Interaction.TextToSpeech.SayKey("whichlanguage", true);
				//	} else
				//		Interaction.TextToSpeech.SayKey("okispeaklang", true);
				//	break;

				case "TellSomething":
					Debug.Log("VocalTrigger tell something");
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introfunfact") + " " + Dictionary.GetRandomString("funfacts"));
					mNeedListen = true;
					break;

				case "Timer":
					Debug.Log("VocalTrigger Timer");
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Timer", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "Iloveyou":
					Debug.Log("VocalTrigger userlove");
					// react as a caress
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(5, -2, "mooduserlove", "USER_LOVE", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
					mActionManager.HeadReaction();
					mNeedListen = true;
					break;

				case "Ihateyou":
					Debug.Log("VocalTrigger userhate");
					// react as eye poked
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-5, -4, "mooduserhate", "USER_HATE", EmotionalEventType.INTERACTION, InternalMood.SAD));
					mActionManager.EyeReaction();
					mNeedListen = true;
					break;

				case "Volume": {
						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							SayKey("getvolumeerror");
						} else {
							BYOS.Instance.Primitive.Speaker.ChangeVolume((int)n);
							Say(Dictionary.GetRandomString("volume") + " " + n, true);
						}

						mNeedListen = true;
					}
					break;

				case "VolumeDown": {
						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 3F;
						}

						SayKey("accept", true);

						Debug.Log("Decrease volume by " + n);
						Primitive.Speaker.FX.Play(FXSound.BEEP_1);
						BYOS.Instance.Primitive.Speaker.VolumeDown((int)n);
						Say(Dictionary.GetRandomString("volumedown") + " " + n, true);
						mNeedListen = true;
					}
					break;

				case "VolumeUp": {

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 3F;
						}

						SayKey("accept", true);
						Debug.Log("Increase volume by " + n);
						Primitive.Speaker.FX.Play(FXSound.BEEP_1);
						BYOS.Instance.Primitive.Speaker.VolumeUp((int)n);
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

					//Debug.Log("Start wanderring by voice " + mVocalChat.Answer);
					mActionManager.WanderingOrder = true;
					//mActionManager.WanderingMood = (MoodType)Enum.Parse(typeof(MoodType), mVocalChat.Answer, true);
					// TODO get it from vocalhelper code
					Trigger("WANDER");
					break;

				case "Weather":
					Debug.Log("VocalTrigger Weather");
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Weather", mLastHumanSpeech);
					mLaunchingApp = true;
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
					//Maybe a question / answer thing
					Debug.Log("[COMPANION] I believe this is a question? " + iType.ToLower());
					try {
						SayKey(iType.ToLower(), true);
					} catch {
						Debug.Log("[COMPANION] Answer key not found: " + iType.ToLower());
					}
					mNeedListen = true;
					break;

			}

		}

		private string GetStringNextNumber(string iText)
		{
			if (!string.IsNullOrEmpty(iText)) {
				string[] lWords = iText.Split(' ');
				for (int i = 0; i < lWords.Length; ++i) {
					try {
						float.Parse(lWords[i]);
						return lWords[i];
					} catch {
						continue;
					}
				}
			}
			return "";
		}

		private float GetNextNumber(string iText)
		{
			if (!string.IsNullOrEmpty(iText)) {
				string[] lWords = iText.Split(' ');
				for (int i = 0; i < lWords.Length; ++i) {
					try {
						return float.Parse(lWords[i]);
					} catch {
						continue;
					}
				}
			}
			return -1.23456F;
		}


		private string SetNapSentence(int iNumber, string iTimeUnit, string iSentence)
		{
			Debug.Log(iNumber);
			if (iNumber == 0)
				iSentence = iSentence.Replace("[" + iTimeUnit + "]", string.Empty);
			else if (iNumber == 1)
				iSentence = iSentence.Replace("[" + iTimeUnit + "]", iNumber.ToString() + " " + Dictionary.GetRandomString(iTimeUnit));
			else
				iSentence = iSentence.Replace("[" + iTimeUnit + "]", iNumber.ToString() + " " + Dictionary.GetRandomString(iTimeUnit) + "s");

			return (iSentence);
		}

		/// <summary>
		/// Recover Time In Seconds
		/// </summary>
		/// <param name="iSpeech">Speech</param>
		/// <returns>Time in Seconds</returns>
		private int GetTime(string iSpeech)
		{
			string lDigitHour = Regex.Match(iSpeech, "\\d{1,2} \\W*((?i)hour(?-i))").Value;
			string lDigitMinute = Regex.Match(iSpeech, "\\d{1,2} \\W*((?i)minute(?-i))").Value;
			string lDigitSecond = Regex.Match(iSpeech, "\\d{1,2} \\W*((?i)second(?-i))").Value;

			lDigitHour = Regex.Match(lDigitHour, "\\d{1,2}").Value;
			lDigitMinute = Regex.Match(lDigitMinute, "\\d{1,2}").Value;
			lDigitSecond = Regex.Match(lDigitSecond, "\\d{1,2}").Value;

			if (string.IsNullOrEmpty(lDigitHour))
				lDigitHour = "0";
			if (string.IsNullOrEmpty(lDigitMinute))
				lDigitMinute = "0";
			if (string.IsNullOrEmpty(lDigitSecond))
				lDigitSecond = "0";

			return ((int.Parse(lDigitHour) * 3600) + (int.Parse(lDigitMinute) * 60) + int.Parse(lDigitSecond));
		}

		/// <summary>
		/// Recover the Time in (Hour, Minutes, Seconds)
		/// </summary>
		/// <param name="TimeInSeconds">time in seconds</param>
		/// <returns>Array of int with 0 : hour, 1 : minutes, 2 : seconds</returns>
		private int[] SecondsToHour(int iTimeInSeconds)
		{
			int lHour = iTimeInSeconds / 3600;
			int lMinute = (iTimeInSeconds % 3600) / 60;
			int lSecond = ((iTimeInSeconds % 3600) % 60);

			Debug.Log(lHour + " heure " + lMinute + " minute " + lSecond + "seconde");

			int[] lResult = { lHour, lMinute, lSecond };

			return (lResult);
		}

		private int RetrieveMaxInt(int[] iMatrix)
		{
			int lMax = -200;

			foreach (int item in iMatrix) {
				if (lMax < item)
					lMax = item;
			}
			return (lMax);
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
					mActionManager.StartApp("PlayMath", mLastHumanSpeech);
					mLaunchingApp = true;
					break;
				case 2:
					mActionManager.StartApp("FreezeDance", mLastHumanSpeech);
					mLaunchingApp = true;
					break;
				case 3:
					mActionManager.StartApp("MemoryGame", mLastHumanSpeech);
					mLaunchingApp = true;
					break;
				case 4:
					mActionManager.StartApp("RLGL", mLastHumanSpeech);
					mLaunchingApp = true;
					break;
				default:
					mActionManager.StartApp("Weather", mLastHumanSpeech);
					mLaunchingApp = true;
					break;
			}
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

		/// <summary>
		/// Change format of the StartRule (startrule#yes -> yes)
		/// </summary>
		/// <param name="iStartRuleVocon">Old format</param>
		/// <returns>New format</returns>
		public static string GetRealStartRule(string iStartRuleVocon)
		{
			if (!string.IsNullOrEmpty(iStartRuleVocon) && iStartRuleVocon.Contains("#")) {
				string lStartRule = iStartRuleVocon.Substring(iStartRuleVocon.IndexOf("#") + 1);
				return (lStartRule);
			}
			return (string.Empty);
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

		private bool ContainsOneOf(string iSpeech, string iKeySpeech)
		{
			string[] iListSpeech = BYOS.Instance.Dictionary.GetPhoneticStrings(iKeySpeech);


			for (int i = 0; i < iListSpeech.Length; ++i) {

				if (string.IsNullOrEmpty(iListSpeech[i]))
					continue;

				string[] words = iSpeech.Split(' ');
				if (words.Length < 2 && !string.IsNullOrEmpty(words[0])) {
					if (words[0].ToLower() == iListSpeech[i].ToLower()) {
						return true;
					}
				} else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
					return true;
			}
			return false;
		}


	}
}