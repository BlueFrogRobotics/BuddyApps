using UnityEngine.UI;
using UnityEngine;

using Buddy;
using System;
using System.IO;
using System.Collections.Generic;

namespace BuddyApp.Companion
{
	/* A basic monobehaviour as "AI" behaviour for your app */
	public class CompanionBehaviour : MonoBehaviour
	{


		public List<UserProfile> Profiles { get; set; }

		/*
         * Modified data from the UI interaction
         */
		[SerializeField]
		private Text text;
		private static CompanionBehaviour sInstance;

		/*
         * API of the robot
         */
		private TextToSpeech mTextToSpeech;

		/*
         * Data of the application. Save on disc when app quit happened
         */
		private CompanionData mAppData;
		/*
         * Init refs to API and your app data
         */


		public UserProfile mCurrentUser;




		public static void SaveProfiles()
		{
			string lFileName = BYOS.Instance.Resources.GetPathToRaw("usersProfile");
			Utils.SerializeXML(sInstance.Profiles.ToArray(), lFileName);
		}


		public UserProfile AddProfile(string iName, string iCity, UserTastes iTastes, string iOccupation)
		{

			UserProfile lProfile = new UserProfile() {
				FirstName = iName,
				CityAddress = iCity,
				Tastes = iTastes,
				Occupation = iOccupation
			};
			Profiles.Add(lProfile);
			SaveProfiles();

			return lProfile;

		}

		public UserProfile AddProfile(string iName)
		{
			string[] lWords = iName.Split(' ');
			UserProfile lProfile;

			if (lWords.Length < 2) {

				lProfile = new UserProfile() {
					FirstName = iName
				};
			} else {

				lProfile = new UserProfile() {
					FirstName = lWords[0],
					LastName = iName.Replace(lWords[0], "")
				};
			}

			Profiles.Add(lProfile);
			SaveProfiles();

			return lProfile;

		}

		public UserProfile AddProfile(UserAccount iAccount)
		{
			UserProfile lProfile = GetProfile(iAccount);
			if (lProfile == null) {
				lProfile = new UserProfile() {
					FirstName = iAccount.FirstName,
					LastName = iAccount.LastName,
					BirthDate = iAccount.BirthDate.ToLongDateString(),
				};
				Profiles.Add(lProfile);
				SaveProfiles();
			}

			return lProfile;
		}


		void Start()
		{
			mCurrentUser = null;
			if (mCurrentUser == null)
				Debug.Log("start CurrentUser Null");
			else

				Debug.Log("start CurrentUser not Null");

				sInstance = this;
			mTextToSpeech = BYOS.Instance.Interaction.TextToSpeech;
			mAppData = CompanionData.Instance;

			Debug.Log("Get file usersProfile");
			string lFileName = BYOS.Instance.Resources.GetPathToRaw("usersProfile");
			Debug.Log("Get file usersProfile 2");

			if (File.Exists(lFileName)) {
				Debug.Log(" file usersProfile exists");
				Profiles = new List<UserProfile>(Utils.UnserializeXML<UserProfile[]>(lFileName));
			} else {
				Debug.Log(" file usersProfile creation");
				Profiles = new List<UserProfile>();
			}


			Debug.Log(" create profile from account");
			// Check if user in Buddy Account and add them if needed
			for (int i = 0; i < BYOS.Instance.DataBase.GetUsers().Length; ++i) {
				AddProfile(BYOS.Instance.DataBase.GetUsers()[i]);
			}
			Debug.Log(" create profile from account");

		}

		private UserProfile GetProfile(UserAccount iAccount)
		{
			for (int i = 0; i < Profiles.Count; ++i) {
				if (Profiles[i].FirstName == iAccount.FirstName && Profiles[i].FirstName == iAccount.LastName)
					return Profiles[i];
			}
			return null;
		}

		/*
         * A sample of use of data (here for basic display purpose)
         */
		void Update()
		{


			//if (mCurrentUser != null)
			//	Debug.Log("companionbehaviour CurrentUser not Null");

			// Update speech rate / pitch with mood
			if (Math.Abs(BYOS.Instance.Interaction.InternalState.Positivity) < 10) {
				if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.Pitch, (1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity)))
					BYOS.Instance.Interaction.TextToSpeech.SetPitch(1.0F + 0.03F * BYOS.Instance.Interaction.InternalState.Positivity);

				//If max value
			} else if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.Pitch, Math.Sign(BYOS.Instance.Interaction.InternalState.Positivity) * 1.3F))
				BYOS.Instance.Interaction.TextToSpeech.SetPitch(Math.Sign(BYOS.Instance.Interaction.InternalState.Positivity) * 1.3F);


			if (Math.Abs(BYOS.Instance.Interaction.InternalState.Energy) < 10) {
				if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.SpeechRate, 1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy))
					BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(1.0F + 0.02F * BYOS.Instance.Interaction.InternalState.Energy);
				//If max value
			} else if (!EquivalentFloat(BYOS.Instance.Interaction.TextToSpeech.SpeechRate, Math.Sign(BYOS.Instance.Interaction.InternalState.Energy) * 1.2F))
				BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(Math.Sign(BYOS.Instance.Interaction.InternalState.Energy) * 1.2F);


			if (text.enabled != CompanionData.Instance.Debug) {
				text.enabled = CompanionData.Instance.Debug;
			}

			// ensure motors stay in same state as config
			if (BYOS.Instance.Primitive.Motors.Wheels.Locked == CompanionData.Instance.CanMoveBody) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked wheels");
				BYOS.Instance.Primitive.Motors.Wheels.Locked = !CompanionData.Instance.CanMoveBody;
			}

			if (BYOS.Instance.Primitive.Motors.YesHinge.Locked == CompanionData.Instance.CanMoveHead) {
				// fixing issue
				Debug.Log("fixing unconsistancy locked head");
				BYOS.Instance.Primitive.Motors.YesHinge.Locked = !CompanionData.Instance.CanMoveHead;
				BYOS.Instance.Primitive.Motors.NoHinge.Locked = !CompanionData.Instance.CanMoveHead;
			}
		}

		private bool EquivalentFloat(float iA, float iB)
		{
			if (Math.Abs(iA - iB) < 0.001)
				return true;
			else
				return false;
		}

	}
}