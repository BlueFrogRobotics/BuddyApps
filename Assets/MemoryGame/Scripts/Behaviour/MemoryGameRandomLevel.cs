using UnityEngine;
using System;
using System.Collections.Generic;
using Buddy;


namespace BuddyApp.MemoryGame
{
	public class MemoryGameRandomLevel
	{
		public float speed;
		public List<int> events;
		public int mCurrentLevel;
		public int NbLevels
		{
			get { return mNbLevels; }
		}

		private int mNbLevels;

		public MemoryGameRandomLevel(int difficulty = 1, bool iUseBody = true)
		{

			System.Random lRdm = new System.Random();
			mCurrentLevel = 1;
			events = new List<int>();
			List<int> lEventPossibilities = new List<int>();
			lEventPossibilities.Add(0);
			lEventPossibilities.Add(3);
			lEventPossibilities.Add(4);

			if (iUseBody) {
				lEventPossibilities.Add(8);
				lEventPossibilities.Add(9);
				lEventPossibilities.Add(10);
				lEventPossibilities.Add(11);
			}

			speed = 4.0f;
			if (difficulty < 0)
				difficulty = 1;

			

			if (difficulty == 0) {
				mNbLevels = 3;
			} else {
				mNbLevels = 5;
				speed = 5.0f;
			}


			if (difficulty > 1) {
				speed = 6.0f;
			}

			int lRandomEvent = lRdm.Next(lEventPossibilities.Count);


			for (int i = 0; i < mNbLevels * 3; ++i) {
				//Debug.Log(" lRandomEvent :" + lRandomEvent);
				events.Add(lEventPossibilities[lRandomEvent]);
				lRandomEvent = lRdm.Next(lEventPossibilities.Count);
			}


		}

	}



}