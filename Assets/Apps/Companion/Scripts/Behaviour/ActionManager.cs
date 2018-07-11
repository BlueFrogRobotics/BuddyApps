using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;
using Buddy.UI;
using Buddy.Command;

namespace BuddyApp.Companion
{


	public enum BUDDY_ACTION
	{
		NONE,
		WANDER,
		DANCE,
		FOLLOW,
		GAME,
		EDUTAINMENT,
		SERVICE,
		JOKE,
		CHAT,
		TOUCH_INTERACT,
		LOOK_FOR_USER,
		ASK_USER_PROFILE,
		INFORM,
		EXPRESS_MOOD,
		NOTIFY
	}



	public enum COMPANION_STATE
	{
		IDLE,
		NAP,
		NOTIFY,
		USER_DETECTED,
		WANDER,
		DANCE,
		FOLLOW,
		TOUCHED,
		LOOK_FOR_USER,
		ASK_USER_PROFILE,
		VOCAL_COMMAND,
		EXPRESS_MOOD
	}

	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	public class ActionManager : MonoBehaviour
	{
		private int mHeadCounter;
		private float mLastHeadTime;
		private int mEyeCounter;
		private float mLastEyeTime;
		private float mTimeMood;
		private float mDurationMood;
		private float mTimeLastOrder;
		private DetectionManager mDetectionManager;
		private DesireManager mDesireManager;
		private InternalMood mInternalStateMood;


		public bool WanderingOrder { get; set; }
		public MoodType WanderingMood { get; set; }
		public BUDDY_ACTION CurrentAction { get; set; }
		public bool CurrentActionHumanOrder { get; set; }

		public bool Wandering { get; private set; }
		public bool ThermalFollow { get; private set; }
		public float LastMoodExpression;
		//private RoombaNavigation mRoomba;

		//public RoombaNavigation Roomba { get; private set; }

		void Start()
		{
			CurrentAction = BUDDY_ACTION.NONE;
			mInternalStateMood = BYOS.Instance.Interaction.InternalState.InternalStateMood;
			WanderingOrder = false;
			WanderingMood = MoodType.NEUTRAL;
			mHeadCounter = 0;
			mEyeCounter = 0;
			mLastHeadTime = 0F;
			mLastEyeTime = 0F;
			mDurationMood = 5F;
			mTimeLastOrder = 0F;
			LastMoodExpression = 0F;
			mDetectionManager = GetComponent<DetectionManager>();
			mDesireManager = GetComponent<DesireManager>();
			CurrentActionHumanOrder = false;
		}

		void Update()
		{
			//Debug.Log("Mood check: time - lastime, mTimeMood" + (Time.time - mTimeMood) + "    " + mTimeMood );
			if (Time.time - mTimeMood > mDurationMood && mTimeMood != 0F) {
				Debug.Log("Mood back to neutral");
				if (Wandering) {
					StartWander(WanderingMood);
				} else {
					BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
				}
				mTimeMood = 0F;
				mHeadCounter = 0;
				mEyeCounter = 0;
			}

			if (!ActiveAction()) {
				// if we are far from default pose, go to default pose:
				if (Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - CompanionData.Instance.mHeadPosition) > 8 && Time.time - mTimeLastOrder > 0.5F) {
					mTimeLastOrder = Time.time;

					BYOS.Instance.Primitive.Motors.YesHinge.SetPosition(CompanionData.Instance.mHeadPosition, 200);

				}
			}
		}




		//*************
		//*  ACTIONS  *
		//*************

		/// <summary>
		/// This function is used to tell if there is a needed action to performed
		/// </summary>
		/// <param name="iState"></param>
		/// <returns></returns>
		public string NeededAction(COMPANION_STATE iState)
		{
			if (mDetectionManager.ActiveUrgentReminder) {
				// we need to deliver the message from notification
				Debug.Log("[Companion][ActionManager] need to notify urgent");
				if (mDetectionManager.UserPresent(iState)) {
					return "NOTIF";
				} else {
					// Only if notif is an emergency
					return "LOOKINGFORSOMEONE";
				}
			}


			// TODO: add battery
			//if (mDetectionManager.mDetectedElement == Detected.BATTERY) {
			//}

			return "";
		}


		// TODO may be safer to return an element from a list of transitions?
		public string DesiredAction(COMPANION_STATE iState)
		{

			if (mDetectionManager.ActiveReminder) {
				// we need to deliver the message from notification
				Debug.Log("[Companion][ActionManager] need to notify");
				if (mDetectionManager.UserPresent(iState)) {
					return "VOCALCOMMAND";
				} else if (mDetectionManager.ActiveUrgentReminder) {
					// Only if notif is an emergency
					return "LOOKINGFORSOMEONE";
				}
			}

			if (mDesireManager.GetMaxDesireValue() > 40) {
				switch (mDesireManager.GetMainDesire()) {


					case DESIRE.EXPRESSMOOD:
						Debug.Log("[Companion][ActionManager] desired action expressmood");
						return "EXPRESSMOOD";

					// TODO: add state propose interact to ask for caress or propose game or ...
					case DESIRE.INTERACT:
						if (!mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.CanMoveBody) {
								Debug.Log("[Companion][ActionManager] desired action look 4");
								return "LOOKINGFORSOMEONE";
							} else {
								Debug.Log("[Companion][ActionManager] desired action IDLE");
								return "IDLE";
							} else if (CompanionData.Instance.CanMoveBody && CompanionData.Instance.mMovingDesire > 80) {
							Debug.Log("[Companion][ActionManager] desired action FOLLOW");
							return "FOLLOW";
						} else if (BYOS.Instance.Interaction.InternalState.Positivity > 3)
							if (CompanionData.Instance.mLearnDesire > CompanionData.Instance.mTeachDesire) {
								Debug.Log("[Companion][ActionManager] desired action ASKJOKE");
								return "ASKJOKE";
							} else {
								Debug.Log("[Companion][ActionManager] desired action TELLJOKE");
								return "TELLJOKE";
							} else {
							Debug.Log("[Companion][ActionManager] desired action PROPOSEGAME");
							return "PROPOSEGAME";
						}

					case DESIRE.MOVE:
						// if Buddy happy and user present, raise chances of dance:
						if (CompanionData.Instance.CanMoveBody) {

							int lChancesToDance = BYOS.Instance.Interaction.InternalState.Positivity;
							if (mDetectionManager.UserPresent(iState))
								lChancesToDance += 3;

							int lRand = UnityEngine.Random.Range(0, 9);
							if (lRand < lChancesToDance) {
								Debug.Log("[Companion][ActionManager] desired action DANCE");
								return "DANCE";
							} else {
								Debug.Log("[Companion][ActionManager] desired action WANDER");
								return "WANDER";
							}
						}
							 // TODO maybe use 2cd highest desire?
							 else {
							Debug.Log("[Companion][ActionManager] desired action IDLE");
							return "IDLE";
						}

					// TODO: add this
					case DESIRE.TEACH:
						if (mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.mHelpDesire > CompanionData.Instance.mInteractDesire) {
								Debug.Log("[Companion][ActionManager] desired action INFORM");
								return "INFORM";
							} else
								return "PROPOSEEDUTAINMENT";
						else if (CompanionData.Instance.CanMoveBody) {
							Debug.Log("[Companion][ActionManager] desired action LOOKINGFORSOMEONE");
							return "LOOKINGFORSOMEONE";
						} else {
							Debug.Log("[Companion][ActionManager] desired action IDLE");
							return "IDLE";
						}

					case DESIRE.HELP:
						if (mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.mTeachDesire > CompanionData.Instance.mInteractDesire) {
								Debug.Log("[Companion][ActionManager] desired action INFORM");
								return "INFORM";
							} else {
								Debug.Log("[Companion][ActionManager] desired action PROPOSESERVICE");
								return "PROPOSESERVICE";
							} else if (CompanionData.Instance.CanMoveBody) {
							Debug.Log("[Companion][ActionManager] desired action LOOKINGFORSOMEONE");
							return "LOOKINGFORSOMEONE";
						} else {
							Debug.Log("[Companion][ActionManager] desired action IDLE");
							return "IDLE";
						}

					case DESIRE.LEARN:
						if (mDetectionManager.UserPresent(iState))

							//TODO: Check how much info we have on present person
							if (BYOS.Instance.Interaction.InternalState.Positivity > 5 && CompanionData.Instance.mInteractDesire < 50) {
								Debug.Log("[Companion][ActionManager] desired action ASKJOKE");
								return "ASKJOKE";
							} else {
								Debug.Log("[Companion][ActionManager] desired action ASKINFO");
								return "ASKINFO";
							} else if (CompanionData.Instance.CanMoveBody) {
							Debug.Log("[Companion][ActionManager] desired action LOOKINGFORSOMEONE");
							return "LOOKINGFORSOMEONE";
						} else {
							Debug.Log("[Companion][ActionManager] desired action IDLE");
							return "IDLE";
						}

					default:
						Debug.Log("[Companion][ActionManager] desired action IDLE");
						return "IDLE";
				}
			} else {
				Debug.Log("[Companion][ActionManager] desired action IDLE");
				return "IDLE";
			}
		}

		/// <summary>
		/// Tells and display the notification
		/// </summary>
		/// <param name="iReminder"></param>
		internal void InformNotif(Buddy.Reminder iReminder, ReminderState iUpdateState = ReminderState.SHOWN, bool iTell = true, bool iDisplay = true)
		{
			// 1st Start saying
			if (iTell) {
				Debug.Log("Tell notif: " + iReminder.State + " " + iReminder.Type + " " + iReminder.Content);

				Debug.Log("[ActionManager] Say Reminder: " + iReminder.Say());

			}

			if (iDisplay) {
				Debug.Log("Display notif: " + iReminder.State + " " + iReminder.Type + " " + iReminder.Content);
				iReminder.DisplayNotif(iUpdateState);
			}
		}

		internal Buddy.Reminder InformNotifPriority(ReminderState iUpdateState = ReminderState.SHOWN, bool iTell = true, bool iDisplay = true)
		{

			List<Buddy.Reminder> lReminders = new List<Buddy.Reminder>();


			Debug.Log("[ActionManager] InformNotifPriority");
			lReminders = BYOS.Instance.DataBase.Memory.Procedural.GetIssuedReminders(ReminderState.SHOWN);

			Debug.Log("[ActionManager] InformNotifPriority got shown messages");

			if (lReminders.Count == 0)
				lReminders = BYOS.Instance.DataBase.Memory.Procedural.GetIssuedReminders(ReminderState.DELIVERED);


			Debug.Log("[ActionManager] InformNotifPriority lReminders count: " + lReminders.Count);

			if (lReminders.Count > 0) {
				Debug.Log("[ActionManager] InformNotifPriority lReminders count>0 ");
				Buddy.Reminder lReminder = new Buddy.Reminder();
				List<Buddy.Reminder> lRemindersUrgent = BYOS.Instance.DataBase.Memory.Procedural.GetIssuedUrgentReminders(ReminderState.DELIVERED);
				if (lRemindersUrgent.Count > 0)
					lReminder = BYOS.Instance.DataBase.Memory.Procedural.GetIssuedUrgentReminders(ReminderState.DELIVERED)[0];

				Debug.Log("[ActionManager] urgent reminder null: " + (lReminder == null));
				lReminder = lReminder ?? lReminders[0];


				Debug.Log("[ActionManager]inform notif");
				InformNotif(lReminder, iUpdateState, iTell, iDisplay);
				Debug.Log("[ActionManager]inform notif");

				return lReminder;
			}

			Debug.Log("[ActionManager] No reminder to tell");

			return null;
		}

		internal void StartApp(string iAppName, string iSpeech = null, bool iLandingTrigg = false)
		{
			CancelOrders();
			Debug.Log("start app " + iAppName + "with param " + iSpeech);
			CompanionData.Instance.LastAppTime = DateTime.Now;
			CompanionData.Instance.LastApp = iAppName;
			CompanionData.Instance.LandingTrigger = iLandingTrigg;
			new StartAppCmd(iAppName, new int[] { }, new float[] { }, new string[] { iSpeech }).Execute();
		}


		internal void CancelOrders()
		{
			WanderingMood = MoodType.NEUTRAL;
			WanderingOrder = false;
			StopAllActions();
		}

		public bool StartWander(MoodType iMood = MoodType.NEUTRAL)
		{
			if (CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				Debug.Log("Start wander");
				StopAllActions();
				BYOS.Instance.Navigation.RandomWalk.StartWander(iMood);
				Wandering = true;
				return true;
			} else
				return false;
		}

		public void StopWander()
		{
			Debug.Log("Stop wander");
			BYOS.Instance.Navigation.Stop();
			Wandering = false;
		}

		public bool StartThermalFollow(HumanFollowType iFollowType)
		{
			if (CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
				StopAllActions();

				ThermalFollow = true;
				BYOS.Instance.Navigation.Follow<HumanFollow>().Facing(iFollowType);
				return true;
			} else
				return false;
		}

		public void StopThermalFollow()
		{
			BYOS.Instance.Navigation.Stop();
			ThermalFollow = false;
		}

		public void StopAllActions()
		{
			if (ThermalFollow)
				StopThermalFollow();
			if (Wandering)
				StopWander();
			StopAllBML();
			BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
		}

		public bool ActiveAction()
		{
			return (Wandering || ThermalFollow || !BYOS.Instance.Interaction.BMLManager.DonePlaying);
		}

		//***************
		//*  REACTIONS  *
		//***************

		public string LaunchReaction(COMPANION_STATE iState, Detected iDetectedElement)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			string lNeededAction;

			if (mDetectionManager.HumanDectected(iDetectedElement))
				lNeededAction = NeededAction(COMPANION_STATE.USER_DETECTED);
			else
				lNeededAction = NeededAction(iState);



			if (string.IsNullOrEmpty(lNeededAction)) {
				switch (iDetectedElement) {
					case Detected.TRIGGER:
						Debug.Log("[Companion][ActionManager] reaction vocal trigger");
						// TODO: add exception states if needed
						if (iState == COMPANION_STATE.NAP)
							return "IDLE";
						else
							return "VOCALCOMMAND";

					case Detected.MOUTH_TOUCH:
						Debug.Log("[Companion][ActionManager] reaction robot mouth touched");
						// TODO: add exception states if needed

						return "VOCALCOMMAND";

					case Detected.TOUCH:
						Debug.Log("[Companion][ActionManager] reaction robot touched");
						// TODO: add exception states if needed

						if (iState == COMPANION_STATE.NAP)
							return "IDLE";
						else
							return "ROBOTTOUCHED";

					case Detected.KIDNAPPING:
						Debug.Log("[Companion][ActionManager] reaction kidnapping");
						// TODO: add exception states if needed
						BYOS.Instance.Interaction.Mood.Set(MoodType.TIRED);
						return "KIDNAPPING";

					case Detected.BATTERY:
						Debug.Log("[Companion][ActionManager] reaction battery low");
						// TODO: add exception states if needed
						return "CHARGE";

					case Detected.THERMAL:
						Debug.Log("[Companion][ActionManager] reaction thermal");
						// TODO: add exception states if needed


						if ((iState == COMPANION_STATE.WANDER && CompanionData.Instance.mMovingDesire > 40) || iState == COMPANION_STATE.NAP)
							return "";

						StopAllActions();
						if (BYOS.Instance.Interaction.InternalState.Positivity > 3)
							BYOS.Instance.Interaction.BMLManager.LaunchRandom("joy");
						else if (BYOS.Instance.Interaction.InternalState.Positivity > -2)
							BYOS.Instance.Interaction.BMLManager.LaunchRandom("surprised");
						else
							BYOS.Instance.Interaction.BMLManager.LaunchRandom(Internal2FaceMood(mInternalStateMood));

						// if we look for a user it is because we have a desire...
						if (iState == COMPANION_STATE.LOOK_FOR_USER) {
							// launch the desire with "fake" user detected state to tell user is present
							return DesiredAction(COMPANION_STATE.USER_DETECTED);
						} else
							return "INTERACT";

					case Detected.HUMAN_RGB:
						Debug.Log("[Companion][ActionManager] reaction human rgb");
						// TODO: add exception states if needed

						if (iState == COMPANION_STATE.NAP)
							return "";
						else
							return "INTERACT";

					default:
						Debug.Log("[Companion][ActionManager] reaction default");
						return "";
				}

			} else
				return lNeededAction;

		}


		public void HeadReaction()
		{

			if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.SAD) {
				if (!ActiveAction()) {
					Debug.Log("No action + face poked -> play sad BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("sad");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + face poked -> play sad wander");
					StartWander(MoodType.SAD);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + face poked  -> play sad");
					TimedMood(MoodType.SAD);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
				}

			} else if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.IDLE) {
				//surprise
				if (!ActiveAction()) {
					Debug.Log("No action + face poked -> play surprise BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("surprised");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + face poked -> play happy wander");
					StartWander(MoodType.HAPPY);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + face poked  -> play Surprise");
					if (BYOS.Instance.Interaction.VocalManager.RecognitionFinished)
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
					TimedMood(MoodType.SURPRISED);
				}

			} else if (BYOS.Instance.Interaction.InternalState.Positivity < 0) {
				//TODO: play BML instead

				if (!ActiveAction()) {
					Debug.Log("No action + face poked -> play grumpy BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("grumpy");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + face poked -> play grumpy wander");
					StartWander(MoodType.GRUMPY);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + eye poked  -> play grumpy");
					TimedMood(MoodType.GRUMPY);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
				}



			} else if (BYOS.Instance.Interaction.InternalState.Positivity > 0) {
				if (!ActiveAction()) {
					Debug.Log("No action + face poked -> play random love BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("love");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + face poked -> play love wander");
					StartWander(MoodType.LOVE);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + face poked  -> play love");
					BYOS.Instance.Interaction.Mood.Set(MoodType.LOVE);
					mTimeMood = Time.time;
				}
			}

		}



		public void EyeReaction()
		{

			if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.ANGRY) {
				//TODO: play BML instead

				if (!ActiveAction()) {
					Debug.Log("No action + eye poked -> play angry BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("angry");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + eye poked -> play angry wander");
					StopWander();
					StartWander(MoodType.ANGRY);
					mTimeMood = Time.time;

				} else {
					Debug.Log("BML + eye poked -> play angry ");
					TimedMood(MoodType.ANGRY);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
					if (BYOS.Instance.Interaction.VocalManager.RecognitionFinished)
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.SIGH);
				}
			} else if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.GRUMPY || BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.BITTER) {
				if (!ActiveAction()) {
					Debug.Log("No action + eye poked -> play grumpy BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("grumpy");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + eye poked -> play grumpy wander");
					StopWander();
					StartWander(MoodType.GRUMPY);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + eye poked  -> play grumpy");
					TimedMood(MoodType.GRUMPY);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
				}

			} else if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.SAD) {
				if (!ActiveAction()) {
					Debug.Log("No action + eye poked -> play sad BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("sad");
					mTimeMood = Time.time;
				} else if (Wandering) {
					Debug.Log("wander + eye poked -> play sad wander");
					StopWander();
					StartWander(MoodType.SAD);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					Debug.Log("no action + eye poked  -> play sad");
					TimedMood(MoodType.SAD);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
				}

			} else if (BYOS.Instance.Interaction.InternalState.Positivity > 0) {
				if (!ActiveAction()) {
					Debug.Log("No action + eye poked -> play random neutral BML");
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");
					mTimeMood = Time.time;
				} else if (BYOS.Instance.Interaction.VocalManager.RecognitionFinished)
					BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);


			}


		}


		//public void HeadReactionOld()
		//{
		//	Debug.Log("Head Reaction counter " + mHeadCounter);
		//	mTimeMood = Time.time;
		//	if (Time.time - mLastHeadTime < 5F)
		//		mHeadCounter++;
		//	else
		//		mHeadCounter = 0;

		//	mLastHeadTime = Time.time;

		//	if (mHeadCounter < 2) {
		//		BYOS.Instance.Interaction.Mood.Set(MoodType.SURPRISED);
		//		BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
		//	} else if (mHeadCounter < 5) {
		//		BYOS.Instance.Interaction.Mood.Set(MoodType.HAPPY);
		//		BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
		//		mTimeMood = Time.time;

		//	} else if (mHeadCounter > 4) {

		//		if (BYOS.Instance.Interaction.BMLManager.DonePlaying && !Wandering)
		//			BYOS.Instance.Interaction.BMLManager.LaunchRandom("love");
		//		else {
		//			BYOS.Instance.Interaction.Mood.Set(MoodType.LOVE);
		//			mTimeMood = Time.time;
		//		}
		//	}
		//}

		//public void EyeReactionOld()
		//{
		//	Debug.Log("Time.time - mLastEyeTime " + (Time.time - mLastEyeTime));
		//	if (Time.time - mLastEyeTime < 5F)
		//		mEyeCounter++;
		//	else
		//		mEyeCounter = 0;
		//	mLastEyeTime = Time.time;


		//	if (mEyeCounter > 7) {

		//		if (!ActiveAction()) {
		//			Debug.Log("No action + eye poked -> play angry BML");
		//			BYOS.Instance.Interaction.BMLManager.LaunchRandom("angry");
		//			mTimeMood = Time.time;
		//		} else if (Wandering) {
		//			Debug.Log("wander + eye poked -> play angry wander");
		//			StopWander();
		//			StartWander(MoodType.ANGRY);
		//			mTimeMood = Time.time;

		//		} else {
		//			Debug.Log("BML + eye poked -> play angry wander");
		//			TimedMood(MoodType.ANGRY);
		//			BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
		//			BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.SIGH);
		//		}
		//	} else {
		//		if (Wandering) {
		//			Debug.Log("wander + eye poked -> play grumpy wander");
		//			StopWander();
		//			StartWander(MoodType.GRUMPY);
		//			mTimeMood = Time.time;

		//		} else {
		//			Debug.Log("no action + eye poked  -> play grumpy wander");
		//			TimedMood(MoodType.GRUMPY);
		//			BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
		//		}
		//	}


		//}


		//***************
		//*  BML        *
		//***************

		internal void StopAllBML()
		{
			BYOS.Instance.Interaction.BMLManager.StopAllBehaviors();

			// Can't know if eyes are closed, open them in case...
			BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.OPEN_EYES);

			if (BYOS.Instance.Interaction.Mood.CurrentMood != MoodType.NEUTRAL) {
				BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
			}
		}


		//***************
		//*  MOODS      *
		//***************

		internal void ShowInternalMood(float iTime = 5F)
		{
			TimedMood(Internal2FaceMood(mInternalStateMood), iTime);
		}

		internal MoodType Internal2FaceMood(InternalMood iInternalMood)
		{

			if (iInternalMood == InternalMood.BITTER)
				return MoodType.GRUMPY;
			else if (iInternalMood == InternalMood.EXCITED || iInternalMood == InternalMood.RELAXED)
				return MoodType.HAPPY;
			else if (iInternalMood == InternalMood.IDLE)
				return MoodType.NEUTRAL;
			else
				return (MoodType)Enum.Parse(typeof(MoodType), iInternalMood.ToString());
		}


		internal void TimedMood(MoodType iMood, float iTime = 5F)
		{
			if (Wandering)
				StartWander(iMood);
			if (BYOS.Instance.Interaction.Mood.CurrentMood != iMood) {
				StopAllBML();
				BYOS.Instance.Interaction.Mood.Set(iMood);
			}

			mTimeMood = Time.time;
			mDurationMood = iTime;
		}

		internal void SetMood(MoodType iMood, float iTime = 5F)
		{
			StopAllBML();

			BYOS.Instance.Interaction.Mood.Set(iMood);
			mTimeMood = Time.time;
			mDurationMood = iTime;
		}


		internal void LookAt(int x, int y)
		{
			BYOS.Instance.Interaction.Face.LookAt(x, y);

			//TODO
			//BYOS.Instance.Interaction.Face.LookAt(x, y, true);
		}

		internal void LookCenter()
		{
			BYOS.Instance.Interaction.Face.LookAt(FaceLookAt.CENTER);
		}


		internal void RandomActionWander()
		{
			StopAllActions();
			if (UnityEngine.Random.Range(0, 2) == 0)
				//RandomMoodWander();
				ShowInternalMood(10F);
			else
				RandomBMLWander();
		}

		internal void RandomBMLWander()
		{
			StopAllActions();
			BYOS.Instance.Interaction.BMLManager.LaunchRandom("wander");
		}

		internal void RandomMoodWander()
		{
			int i = UnityEngine.Random.Range(1, 10);

			switch (i) {
				case 1:
					StartWander(MoodType.SAD);
					break;
				case 2:
					StartWander(MoodType.HAPPY);
					break;
				case 3:
					StartWander(MoodType.SICK);
					break;
				case 4:
					StartWander(MoodType.LOVE);
					//StartWander(MoodType.TIRED);
					break;
				case 5:
					StartWander(MoodType.SCARED);
					break;
				case 6:
					StartWander(MoodType.ANGRY);
					break;
				case 7:
					StartWander(MoodType.LOVE);
					break;
				default:
					StartWander(MoodType.NEUTRAL);
					break;
			}
		}


		//***************
		//*  LOCKS      *
		//***************


		internal void LockAll()
		{
			LockWheels();
			LockHead();
		}

		internal void UnlockAll()
		{
			UnlockWheels();
			UnlockHead();
		}

		internal void LockWheels()
		{
			CompanionData.Instance.CanMoveBody = false;
			BYOS.Instance.Primitive.Motors.Wheels.Locked = true;
		}

		internal void UnlockWheels()
		{
			CompanionData.Instance.CanMoveBody = true;
			BYOS.Instance.Primitive.Motors.Wheels.Locked = false;
		}

		internal void LockHead()
		{
			CompanionData.Instance.CanMoveHead = false;
			BYOS.Instance.Primitive.Motors.YesHinge.Locked = true;
			BYOS.Instance.Primitive.Motors.NoHinge.Locked = true;
		}

		internal void UnlockHead()
		{
			CompanionData.Instance.CanMoveHead = true;
			BYOS.Instance.Primitive.Motors.NoHinge.Locked = false;
			BYOS.Instance.Primitive.Motors.YesHinge.Locked = false;
		}

	}
}