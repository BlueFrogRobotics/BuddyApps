using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class User {

		private static User sInstance;

		private string mName = "";

		private int mId = 0;

		private GameParameters mGameParameters = GameParameters.LoadDefault();

		// private DegreeList mDegrees TODO

		// private ScoreList mPalmares TODO

		public string Name 
		{
			get 
			{
				return mName;
			}

			set 
			{
				mName = value;
			}
		}

		public int Id 
		{
			get 
			{
				return mId;
			}
		}

		public GameParameters GameParameters 
		{
			get 
			{
				return mGameParameters;
			}
		}
			
		/*
         * Singleton access
         */
		public static User Instance
		{
			get
			{
				if (sInstance == null)
					sInstance = new User();
				return sInstance;
			}
		}
	}
}
