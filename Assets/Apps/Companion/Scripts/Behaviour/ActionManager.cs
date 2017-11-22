using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;

namespace BuddyApp.Companion
{
	/// <summary>
	/// Manager class that have reference to the differents stimuli and subscribes to their callbacks
	/// </summary>
	[RequireComponent(typeof(RoombaNavigation))]
	public class ActionManager : MonoBehaviour
	{
		private int mMouthCounter;
		private float mLastMouthTime;
		private int mEyeCounter;
		private float mLastEyeTime;
		private float mTimeMood;

		public bool Wandering { get; private set; }
		public bool ThermalFollow { get; private set; }


		/// <summary>
		/// Speaker volume
		/// </summary>
		public int Volume { get; set; }

		public RoombaNavigation Roomba { get; private set; }

		void Start()
		{
			mMouthCounter = 0;
			mEyeCounter = 0;
			mLastMouthTime = 0F;
			mLastEyeTime = 0F;
			Volume = BYOS.Instance.Primitive.Speaker.GetVolume();
			Roomba = BYOS.Instance.Navigation.Roomba;
			Roomba.enabled = false;
		}

		void Update()
		{
			//Debug.Log("Mood check: time - lastime, mTimeMood" + (Time.time - mTimeMood) + "    " + mTimeMood );
			if (Time.time - mTimeMood > 5F && mTimeMood != 0F) {
				Debug.Log("Mood back to neutral");
				BYOS.Instance.Interaction.Mood.Set(MoodType.NEUTRAL);
				mTimeMood = 0F;
				mMouthCounter = 0;
				mEyeCounter = 0;
			}
        }

		public void StartWander()
		{
			if (ThermalFollow) {
				StopThermalFollow();
			}
			Roomba.enabled = true;
			Wandering = true;
		}

		public void StopWander()
		{
			Roomba.enabled = false;
			Wandering = false;
		}

		public void StartThermalFollow()
		{
			if (Roomba.enabled) {
				StopWander();
			}
			BYOS.Instance.Navigation.Follow<HumanFollow>().Facing();

		}

		public void StopThermalFollow()
		{
			BYOS.Instance.Navigation.Stop();
			ThermalFollow = false;
		}

		public void StopAllActions()
		{
			StopWander();
			StopThermalFollow();
		}

		public void MouthReaction()
		{
			if (mMouthCounter > 2) {
				//TODO: play BML instead
				BYOS.Instance.Interaction.Mood.Set(MoodType.SICK);
				mTimeMood = Time.time;
			} else {
				if (Time.time - mLastMouthTime < 5F)
					mMouthCounter++;
				else
					mMouthCounter = 0;
				mLastMouthTime = Time.time;
			}
		}

		public void EyeReaction()
		{
			if (mEyeCounter > 2) {
				//TODO: play BML instead
				BYOS.Instance.Interaction.Mood.Set(MoodType.GRUMPY);
				mTimeMood = Time.time;
			} else {
				Debug.Log("Time.time - mLastEyeTime " + (Time.time - mLastEyeTime));
				if (Time.time - mLastEyeTime < 5F)
					mEyeCounter++;
				else
					mEyeCounter = 0;
				mLastEyeTime = Time.time;
			}
		}

	}
}
