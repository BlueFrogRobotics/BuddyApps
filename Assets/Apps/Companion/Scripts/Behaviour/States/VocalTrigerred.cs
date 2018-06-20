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
		private string mPreviousOperationResult;

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
			if (!mDetectionManager.ActiveReminder) {
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
			Interaction.VocalManager.ClearGrammars();
			Interaction.VocalManager.AddGrammar("companion_commands", LoadContext.APP);
			Interaction.VocalManager.AddGrammar("companion_questions", LoadContext.APP);
			Interaction.VocalManager.AddGrammar("weather", LoadContext.APP);
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

			Debug.Log("[COMPANION][ONBESTRECO] Reco vocal: " + iBestResult.Utterance);
			if (!Interaction.TextToSpeech.IsSpeaking) {
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

			// Is there a needed action?
			mActionTrigger = mActionManager.NeededAction(COMPANION_STATE.VOCAL_COMMAND);
			if (!string.IsNullOrEmpty(mActionTrigger)) {
				Trigger(mActionTrigger);
			} else {


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
						if (mDetectionManager.ActiveReminder) {
							Buddy.Reminder lReminder = mActionManager.InformNotifPriority(ReminderState.DELIVERED);
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
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			Debug.Log("VOCAL TRIGGERED STATE EXIT");
			mNeedListen = true;
			mSpeechInput = false;

			Interaction.VocalManager.ClearGrammars();
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
			Debug.Log("Question Type found : " + iType);
			string lSentence = "";
			if (iType != "Repeat")
				mLastBuddySpeech = "";

			string lMood = FindMood(iType);
			if (!string.IsNullOrEmpty(lMood))
				iType = "bml";


			Debug.Log("last human utterance : " + mLastHumanSpeech);

			mSpeechInput = false;

			switch (iType) {

				case "accept":
					Debug.Log("Accept VocalTrigger");
					SayKey("ilisten");
					mNeedListen = true;
					break;

				case "alarm":
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

				case "babyphone":
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("BabyPhone", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "battery":
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("informbattery")
						.Replace("[batterylevel]", BYOS.Instance.Primitive.Battery.EnergyLevel.ToString()));
					mNeedListen = true;
					break;

				case "bml":
					Debug.Log("Playing BML " + lMood);
					//if (string.IsNullOrEmpty(mVocalChat.Answer))
					//	Interaction.BMLManager.LaunchByName("AllIn");
					//else if (!Interaction.BMLManager.LaunchByName(mVocalChat.Answer)) {
					if (!Interaction.BMLManager.LaunchRandom(lMood))
						Say("I don't know the behaviour " + lMood);

					//}
					mNeedListen = true;
					break;


				case "buddylab":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("BuddyLab", mLastHumanSpeech);
					mLaunchingApp = true;
					break;


				case "calculationgame":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("PlayMath", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "canmove":
					SayKey("icanmove", true);

					if (CompanionData.Instance.mMovingDesire > 20)
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 3, "moodcanmove", "CAN_MOVE", EmotionalEventType.FULFILLED_DESIRE, InternalMood.EXCITED));

					else
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(3, 1, "moodcanmove", "CAN_MOVE", EmotionalEventType.INTERACTION, InternalMood.HAPPY));

					mActionManager.UnlockAll();
					mNeedListen = true;
					break;

				case "colourseen":
					Trigger("COLOUR");
					break;

				case "connection":
					Trigger("CONNECTION");
					break;

				case "copy":
					lSentence = Dictionary.GetRandomString("copy");
					Say(lSentence);
					Trigger("COPY");
					break;

				case "detectobject":
					if (BYOS.Instance.Perception.Obstacle.Detect().Length != 0)
						lSentence = BYOS.Instance.Dictionary.GetRandomString("seesomething");
					else
						lSentence = BYOS.Instance.Dictionary.GetRandomString("dontseeanything");
					Say(lSentence);
					mNeedListen = true;
					break;
				case "dance":
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

				case "demoshort":
					Debug.Log("Playing BML demoShort");
					Interaction.BMLManager.LaunchByName("DemoShort");
					mNeedListen = true;
					break;

				case "demofull":
					Debug.Log("Playing BML  demoFull");
					Interaction.BMLManager.LaunchByName("DemoFull");
					mNeedListen = true;
					break;

				case "date":
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

				case "definition":
					mNeedToGiveAnswer = true;
					break;

				case "dontmove":
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

				case "dosomething":
					Debug.Log("do something");
					if (UnityEngine.Random.Range(0, 2) == 0)
						RandomGame();
					else
						RandomBML();
					break;

				case "followme":
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

				case "freezedance":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("FreezeDance", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "guardian":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("Guardian", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "headdown": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headdown").Replace("[degrees]", "" + n), true);

						Debug.Log("head down " + n + " degrees + VocalChat.Answer: " + n);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition + n;
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "headup": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 25F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headup").Replace("[degrees]", "" + n), true);

						Debug.Log("head up " + n + " degrees + VocalChat.Answer: " + n);
						CompanionData.Instance.HeadPosition = Primitive.Motors.YesHinge.CurrentAnglePosition - n;
						//Primitive.Motors.YesHinge.SetPosition(Primitive.Motors.YesHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "headleft": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();

						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 35F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headleft").Replace("[degrees]", "" + n), true);

						Debug.Log("head left " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition + n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "headright": {
						mActionManager.CancelOrders();
						mActionManager.UnlockHead();


						float n = GetNextNumber(iUtterance);

						if (n == -1.23456F) {
							//default value
							n = 35F;
						}

						SayKey("accept", true);
						Say(Dictionary.GetRandomString("headright").Replace("[degrees]", "" + n), true);

						Debug.Log("head right " + n + " degrees + VocalChat.Answer: " + n);
						Primitive.Motors.NoHinge.SetPosition(Primitive.Motors.NoHinge.CurrentAnglePosition - (float)n, 100F);
						mMoving = true;
						mTimeMotion = Time.time;
					}
					break;

				case "heat":
					int[] lMatrix = BYOS.Instance.Primitive.ThermalSensor.MatrixArray;
					int lMax = RetrieveMaxInt(lMatrix);
					lSentence = Dictionary.GetRandomString("heat").Replace("[degree]", lMax.ToString());
					Say(lSentence);
					mNeedListen = true;
					break;


				case "hideseek":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("hideAndSeek", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "hour":
					if (BYOS.Instance.Language.CurrentLang == Language.FR) {
						lSentence = Dictionary.GetRandomString("givehour").Replace("[hour]", "" + DateTime.Now.Hour);
						lSentence = lSentence.Replace("[minute]", "" + DateTime.Now.Minute);
					} else
						lSentence = GiveHourInEnglish();
					Say(lSentence);
					mNeedListen = true;
					break;

				case "ihateyou":
					Debug.Log("VocalTrigger userhate");
					// react as eye poked
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-5, -4, "mooduserhate", "USER_HATE", EmotionalEventType.INTERACTION, InternalMood.SAD));
					mActionManager.EyeReaction();
					mNeedListen = true;
					break;

				case "iloveyou":
					Debug.Log("VocalTrigger userlove");
					// react as a caress
					BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(5, -2, "mooduserlove", "USER_LOVE", EmotionalEventType.INTERACTION, InternalMood.RELAXED));
					mActionManager.HeadReaction();
					mNeedListen = true;
					break;

				case "iot":
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Somfy", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "joke":
					Trigger("TELLJOKE");
					break;

				case "listenjoke":
					if (ContainsOneOf(mLastHumanSpeech, "knockknock"))
						Interaction.TextToSpeech.SayKey("whoisthere");
					else
						Interaction.TextToSpeech.SayKey("ilisten");
					Trigger("LISTENJOKE");
					break;

				case "jukebox":
					mActionManager.StartApp("Jukebox", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "mirror":
					int time = GetTime(mLastHumanSpeech);

					SetInteger("Duration", time);

					Trigger("MIRROR");
					break;

				case "mood":
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


				case "movebackward": {
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

				case "moveforward": {
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

				case "moveleft": {
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

				case "moveright": {
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

				case "memorygame":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("MemoryGame", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "nap":
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

				case "operation":


					Debug.Log("Operation " + mLastHumanSpeech);

					string lComputeOrder = mLastHumanSpeech.ToLower().Replace(BYOS.Instance.Dictionary.GetString("add"), "+");
					Debug.Log("Operation2 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace("plus", "+");
					Debug.Log("Operation3 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("divided"), "/");
					Debug.Log("Operation4 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("divideit"), "/");
					Debug.Log("Operation5 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("dividedper"), "/");
					Debug.Log("Operation6 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("minus"), "-");
					Debug.Log("Operation7 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("times"), "*");
					Debug.Log("Operation8 " + lComputeOrder);
					lComputeOrder = lComputeOrder.Replace(BYOS.Instance.Dictionary.GetString("power"), "^");
					Debug.Log("Operation9 " + lComputeOrder);



					mPreviousOperationResult = Compute(lComputeOrder).ToString();
					Say(Dictionary.GetRandomString("computeresult") + " " + mPreviousOperationResult);


					mState.text = "Vocal Triggered " + lComputeOrder + " = " + mPreviousOperationResult;

					mNeedListen = true;
					break;

				case "operationagain":

					// We add the current operation to the previous result

					Debug.Log("OperationAgain");
					string lComputeNewOrder = "";
					if (!string.IsNullOrEmpty(mPreviousOperationResult)) {
						lComputeNewOrder = mPreviousOperationResult + " ";



						lComputeNewOrder += mLastHumanSpeech.ToLower().Replace(BYOS.Instance.Dictionary.GetString("add"), "+");
						lComputeNewOrder = lComputeNewOrder.Replace("plus", "+");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("divided"), "/");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("divideit"), "/");
						lComputeNewOrder = lComputeNewOrder.Replace(BYOS.Instance.Dictionary.GetString("dividedper"), "/");
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


				case "play":
					CompanionData.Instance.mInteractDesire -= 30;

					Say("ok");
					Debug.Log("random game");
					RandomGame();
					break;

				case "photo":
					CompanionData.Instance.mInteractDesire -= 30;
					Debug.Log("starting app takephoto");
					mActionManager.StartApp("TakePhoto", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "pose":
					CompanionData.Instance.mInteractDesire -= 30;
					mActionManager.StartApp("Take Pose", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "quit":
					if (mActionManager.ThermalFollow)
						Trigger("FOLLOW");
					else if (mActionManager.Wandering && CompanionData.Instance.CanMoveHead)
						Trigger("WANDER");
					else
						Trigger("DISENGAGE");
					break;

				case "quizz":
					CompanionData.Instance.mInteractDesire -= 50;
					mActionManager.StartApp("QuizzGame", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

                case "radio":
                    CompanionData.Instance.mInteractDesire -= 20;
                    mActionManager.StartApp("Radioplayer", mLastHumanSpeech);
                    mLaunchingApp = true;
                    break;

                case "recipe":
					CompanionData.Instance.mInteractDesire -= 20;
					mActionManager.StartApp("Recipe", mLastHumanSpeech);
					mLaunchingApp = true;
					break;


				case "reminder":
					CompanionData.Instance.mHelpDesire -= 20;
					mActionManager.StartApp("Reminder", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "repeat":
					Interaction.TextToSpeech.SayKey("isaid", true);
					Interaction.TextToSpeech.Say("[500]", true);
					Interaction.TextToSpeech.Say(mLastBuddySpeech, true);
					mNeedListen = true;
					break;

				case "rlgl":
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

				case "tellsomething":
					Debug.Log("VocalTrigger tell something");
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introfunfact") + " " + Dictionary.GetRandomString("funfacts"));
					mNeedListen = true;
					break;

				case "timer":
					Debug.Log("VocalTrigger Timer");
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Timer", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "volume": {
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

				case "volumeDown": {
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

				case "volumeup": {

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

				case "wander":
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

				case "weather":
					Debug.Log("VocalTrigger Weather");
					CompanionData.Instance.mInteractDesire -= 10;
					mActionManager.StartApp("Weather", mLastHumanSpeech);
					mLaunchingApp = true;
					break;

				case "welcome":
					Debug.Log("Playing BML Welcome");
					Interaction.BMLManager.LaunchByName("Welcome");
					mNeedListen = true;
					break;

				case "lookAtMe":
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

		private string FindMood(string iSpeech)
		{
			if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("sad"))
				return MoodType.SAD.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("happy"))
				return MoodType.HAPPY.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("lovely"))
				return MoodType.LOVE.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("sick"))
				return MoodType.SICK.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("tired"))
				return MoodType.TIRED.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("angry"))
				return MoodType.ANGRY.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("grumpy"))
				return MoodType.GRUMPY.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("scared"))
				return MoodType.SCARED.ToString();
			else if (iSpeech.ToLower() == BYOS.Instance.Dictionary.GetString("neutral"))
				return MoodType.NEUTRAL.ToString();
			else
				return "";
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