using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;

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
		INFORM_MOOD,
		NOTIFY
	}



	public enum COMPANION_STATE
	{
		IDLE,
		USER_DETECTED,
		WANDER,
		DANCE,
		FOLLOW,
		TOUCHED,
		LOOK_FOR_USER,
		ASK_USER_PROFILE,
		VOCAL_COMMAND,
		INFORM_MOOD
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
				if (Math.Abs(BYOS.Instance.Primitive.Motors.YesHinge.CurrentAnglePosition - CompanionData.Instance.HeadPosition) > 8 && Time.time - mTimeLastOrder > 0.5F) {
					mTimeLastOrder = Time.time;

					BYOS.Instance.Primitive.Motors.YesHinge.SetPosition(CompanionData.Instance.HeadPosition, 200);

				}
			}
		}




		//*************
		//*  ACTIONS  *
		//*************

		// TODO may be better to return an element from a list of transitions?
		public string LaunchDesiredAction(COMPANION_STATE iState)
		{
			if (mDesireManager.GetMaxDesireValue() > 40) {
				switch (mDesireManager.GetMainDesire()) {


					case DESIRE.EXPRESSMOOD:
						return "EXPRESSMOOD";

					// TODO: add state propose interact to ask for caress or propose game or ...
					case DESIRE.INTERACT:
						if (!mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.CanMoveBody)
								return "LOOKINGFORSOMEONE";
							else
								return "IDLE";
						else if (CompanionData.Instance.CanMoveBody && CompanionData.Instance.mMovingDesire > 80)
							return "FOLLOW";
						else if (BYOS.Instance.Interaction.InternalState.Positivity > 3)
							if (CompanionData.Instance.mLearnDesire > CompanionData.Instance.mTeachDesire)
								return "ASKJOKE";
							else
								return "TELLJOKE";
						else
							return "PROPOSEGAME";

					case DESIRE.MOVE:
						// if Buddy happy and user present, raise chances of dance:
						if (CompanionData.Instance.CanMoveBody) {

							int lChancesToDance = BYOS.Instance.Interaction.InternalState.Positivity;
							if (mDetectionManager.UserPresent(iState))
								lChancesToDance += 3;

							int lRand = UnityEngine.Random.Range(0, 9);
							if (lRand < lChancesToDance)
								return "DANCE";
							else
								return "WANDER";
						} else
							// TODO maybe use 2cd highest desire?
							return "IDLE";

					// TODO: add this
					case DESIRE.TEACH:
						if (mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.mHelpDesire > CompanionData.Instance.mInteractDesire)
								return "INFORM";
							else
								return "PROPOSEEDUTAINMENT";
						else if (CompanionData.Instance.CanMoveBody)
							return "LOOKINGFORSOMEONE";
						else
							return "IDLE";

					case DESIRE.HELP:
						if (mDetectionManager.UserPresent(iState))
							if (CompanionData.Instance.mTeachDesire > CompanionData.Instance.mInteractDesire)
								return "INFORM";
							else
								return "PROPOSESERVICE";
						else if (CompanionData.Instance.CanMoveBody)
							return "LOOKINGFORSOMEONE";
						else
							return "IDLE";

					case DESIRE.LEARN:
						if (mDetectionManager.UserPresent(iState))

							//TODO: Check how much info we have on present person
							if (BYOS.Instance.Interaction.InternalState.Positivity > 5 && CompanionData.Instance.mInteractDesire < 50)
								return "ASKJOKE";
							else
								return "ASKINFO";
						else if (CompanionData.Instance.CanMoveBody)
							return "LOOKINGFORSOMEONE";
						else
							return "IDLE";

					default:
						return "IDLE";
				}
			} else
				return "IDLE";
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

			switch (iDetectedElement) {
				case Detected.TRIGGER:
					Debug.Log("[Companion][ActionManager] reaction vocal trigger");
					// TODO: add exception states if needed
					return "VOCALTRIGGERED";

				case Detected.MOUTH_TOUCH:
					Debug.Log("[Companion][ActionManager] reaction robot mouth touched");
					// TODO: add exception states if needed
					return "VOCALTRIGGERED";

				case Detected.TOUCH:
					Debug.Log("[Companion][ActionManager] reaction robot touched");
					// TODO: add exception states if needed
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
					return "INTERACT";

				case Detected.HUMAN_RGB:
					Debug.Log("[Companion][ActionManager] reaction human rgb");
					// TODO: add exception states if needed
					return "INTERACT";

				default:
					return "";
			}
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
					if(BYOS.Instance.Interaction.SpeechToText.HasFinished)
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
					if (BYOS.Instance.Interaction.SpeechToText.HasFinished)
						BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.SIGH);
				}
			} else if (BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.GRUMPY || BYOS.Instance.Interaction.InternalState.InternalStateMood == InternalMood.SALTY) {
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
				} else if (BYOS.Instance.Interaction.SpeechToText.HasFinished)
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

			if (iInternalMood == InternalMood.SALTY)
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