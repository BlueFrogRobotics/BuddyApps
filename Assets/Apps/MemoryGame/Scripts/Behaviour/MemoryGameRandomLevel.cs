using UnityEngine;
using System;
using System.Collections.Generic;
using BlueQuark;


namespace BuddyApp.MemoryGame
{
    public enum MemoryEvent
    {
        MOUTH = 0,
        RIGHT_EYE = 3,
        LEFT_EYE = 4,
        BACK_HEAD = 5,
        RIGHT_HEAD = 6,
        LEFT_HEAD = 7,
        TURN_LEFT = 8,
        TURN_RIGHT = 9
    };

	public class MemoryGameRandomLevel : MonoBehaviour
	{
		public float speed;
		public List<int> events;
		public int mCurrentLevel;
		public int NbLevels
		{
			get { return mNbLevels; }
		}

		private int mNbLevels;

		public void Init(int difficulty = 1, bool moveHead = true, bool moveBody = false)
		{
			System.Random lRdm = new System.Random();
			mCurrentLevel = 1;
			events = new List<int>();
			List<int> lEventPossibilities = new List<int>();
            lEventPossibilities.Add((int)MemoryEvent.MOUTH);
            lEventPossibilities.Add((int)MemoryEvent.LEFT_EYE);
            lEventPossibilities.Add((int)MemoryEvent.RIGHT_EYE);

            if (moveHead)
            {
                lEventPossibilities.Add((int)MemoryEvent.BACK_HEAD);
                lEventPossibilities.Add((int)MemoryEvent.RIGHT_HEAD);
                lEventPossibilities.Add((int)MemoryEvent.LEFT_HEAD);
            }
            if (moveBody)
            {
                lEventPossibilities.Add((int)MemoryEvent.TURN_LEFT);
                lEventPossibilities.Add((int)MemoryEvent.TURN_RIGHT);
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