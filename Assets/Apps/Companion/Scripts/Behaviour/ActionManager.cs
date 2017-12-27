using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using System;

namespace BuddyApp.Companion
{
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

		public bool WanderingOrder { get; set; }
		public MoodType WanderingMood { get; set; }

		public bool Wandering { get; private set; }
		public bool ThermalFollow { get; private set; }
		//private RoombaNavigation mRoomba;

		//public RoombaNavigation Roomba { get; private set; }

		void Start()
		{
			WanderingOrder = false;
			WanderingMood = MoodType.NEUTRAL;
			mHeadCounter = 0;
			mEyeCounter = 0;
			mLastHeadTime = 0F;
			mLastEyeTime = 0F;
			mDurationMood = 5F;
			//mRoomba = BYOS.Instance.Navigation.Roomba;
			//mRoomba.enabled = false;
		}

		void Update()
		{
			//Debug.Log("Mood check: time - lastime, mTimeMood" + (Time.time - mTimeMood) + "    " + mTimeMood );
			if (Time.time - mTimeMood > mDurationMood && mTimeMood != 0F) {
				Debug.Log("Mood  back to neutral");
				if (Wandering) {
					StopWander();
					StartWander(WanderingMood);
				} else {
					BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
				}
				mTimeMood = 0F;
				mHeadCounter = 0;
				mEyeCounter = 0;
			}
		}

		public void StartWander(MoodType iMood)
		{
			Debug.Log("Start wander");
			if (ThermalFollow) {
				StopThermalFollow();
			}
			BYOS.Instance.Navigation.RandomWalk.StartWander(iMood);
			Wandering = true;
		}

		public void StopWander()
		{
			Debug.Log("Stop wander");
			BYOS.Instance.Navigation.Stop();
			Wandering = false;
		}

		public void StartThermalFollow(HumanFollowType iFollowType)
		{
			if (Wandering) {
				StopWander();
			}

			ThermalFollow = true;
			BYOS.Instance.Navigation.Follow<HumanFollow>().Facing(iFollowType);
		}

		public void StopThermalFollow()
		{
			BYOS.Instance.Navigation.Stop();
			ThermalFollow = false;
		}

		public void StopAllActions()
		{
			StopThermalFollow();
			StopWander();
		}

		public void HeadReaction()
		{
			Debug.Log("Head Reaction counter " + mHeadCounter);
			mTimeMood = Time.time;
			if (Time.time - mLastHeadTime < 5F)
				mHeadCounter++;
			else
				mHeadCounter = 0;

			mLastHeadTime = Time.time;

			//if (BYOS.Instance.Interaction.BMLManager.DonePlaying)
			//	if (mHeadCounter < 2) {
			//		BYOS.Instance.Interaction.Mood.Set(MoodType.SURPRISED);
			//		BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			//	} else if (mHeadCounter < 3) {
			//		BYOS.Instance.Interaction.BMLManager.LaunchRandom("surprised");
			//	} else if (mHeadCounter > 4) {
			//		BYOS.Instance.Interaction.BMLManager.LaunchRandom("love");
			//		mTimeMood = Time.time;
			//	}

			if (mHeadCounter < 2) {
				BYOS.Instance.Interaction.Mood.Set(MoodType.SURPRISED);
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			} else if (mHeadCounter < 5) {
				//TODO: play BML instead
				BYOS.Instance.Interaction.Mood.Set(MoodType.HAPPY);
				BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
				mTimeMood = Time.time;

			} else if (mHeadCounter > 4) {
				//TODO: play BML instead
				BYOS.Instance.Interaction.Mood.Set(MoodType.LOVE);
				mTimeMood = Time.time;
			}
		}

		public void EyeReaction()
		{
			Debug.Log("Time.time - mLastEyeTime " + (Time.time - mLastEyeTime));
			if (Time.time - mLastEyeTime < 5F)
				mEyeCounter++;
			else
				mEyeCounter = 0;
			mLastEyeTime = Time.time;

			//if (BYOS.Instance.Interaction.BMLManager.DonePlaying)
			//	if (mEyeCounter > 7)
			//		BYOS.Instance.Interaction.BMLManager.LaunchRandom("angry");
			//	else
			//		//BYOS.Instance.Interaction.Mood.Set(MoodType.GRUMPY);
			//		//BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
			//		//mTimeMood = Time.time;

			//		BYOS.Instance.Interaction.BMLManager.LaunchRandom("grumpy");


			if (mEyeCounter > 7) {
				//TODO: play BML instead

				if (BYOS.Instance.Interaction.BMLManager.DonePlaying && !Wandering)
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("angry");
				else if (Wandering) {
					StopWander();
					StartWander(MoodType.ANGRY);
					mTimeMood = Time.time;

				} else {
					BYOS.Instance.Interaction.Mood.Set(MoodType.ANGRY);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
					BYOS.Instance.Primitive.Speaker.Voice.Play(VoiceSound.SIGH);
					mTimeMood = Time.time;
				}
			} else {
				if (Wandering) {
					StopWander();
					StartWander(MoodType.GRUMPY);
					mTimeMood = Time.time;

				} else {
					//TODO: play BML instead
					BYOS.Instance.Interaction.Mood.Set(MoodType.GRUMPY);
					BYOS.Instance.Interaction.Face.SetEvent(FaceEvent.SCREAM);
					mTimeMood = Time.time;
				}
			}


		}

		internal void TimedMood(MoodType iMood, float iTime = 5F)
		{
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
