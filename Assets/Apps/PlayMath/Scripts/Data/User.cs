using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class User {

		private static User sInstance;

		public string Name { get; set; }

		public int Id { get; }

		public GameParameters GameParameters { get; }

		// private DegreeList mDegrees TODO

		public ScoreSummaryList Scores { get; }
			
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

		public User() {
			this.Name = "buddy";
			this.Id = 0;
			this.GameParameters = GameParameters.LoadDefault();
			this.Scores = ScoreSummaryList.LoadDefault();
		}
	}
}
