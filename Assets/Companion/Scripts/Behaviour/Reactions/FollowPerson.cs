using UnityEngine;
using System.Collections;

using BuddyOS;
using BuddyOS.Command;

namespace BuddyApp.Companion
{
	public class FollowPerson : MonoBehaviour {

		// link to other features
		private Mood mMood;
		private ThermalSensor mThermlSensor;

		// local variables
		private bool isFollowingSomeone;

		// Use this for initialization
		void Start () {
			isFollowingSomeone = false;
			//init the therml trcker
			// face front 
			// mid temperature

		}
		
		// Update is called once per frame
		void Update () {
			if(isFollowingSomeone)
			{
			// if I'm following someone already 
				// depending of the previous position
				// i search the current position
				int[] position = detectThermalPosition();
				// if i don't find anything once I count
				// if i don't find anybody for several frame I stop
				//lostFollowing();
				// i go to this new position
			}
			else // if isFollowingSomeone
			{
			// if I'm looking for someone
				// look round until i find a signature
				// set the target
				// go to this trget
			}
		}

		private void findFollowing() // 
		{
			if (isFollowingSomeone)
				return;
			isFollowingSomeone = true;
			mMood.Set (MoodType.HAPPY);

		}

		private void lostFollowing() // start looking for someone
		{
			if (!isFollowingSomeone)
				return;
			isFollowingSomeone = false;
			mMood.Set (MoodType.THINKING);
					
		}

		private void finishedFollowing()
		{
			// exit of this function

		}

		private int[] detectThermalPosition()
		{
			int[] valueMatrix = mThermlSensor.Matrix;
			int[] sumVert = new int[4];
			int[] sumHor = new int[4];
			for (int i = 0; i < 16; i++) 
			{
				sumHor [i/4] += valueMatrix [i]; // we sum for each line
				sumVert [i%4] += valueMatrix [i]; // we sum for each colomn
			}

			int maxHorizontal = -1;
			int maxVertical = -1;
			int valueHorizontalMax = 0;
			int valueVerticalMax = 0;
			// TODO : here we can limite the max temperture
			for (int i = 0; i < 4; i++) 
			{
				if (valueHorizontalMax < sumHor [i])
					maxHorizontal = i;
				if (valueVerticalMax < sumVert [i])
					maxVertical = i;
			}
			if (maxHorizontal == 0 || maxVertical == -1)
				Debug.Log ("We ve got a problem finding the position of the hot spot");
			else
				Debug.Log ("hot spot is detected at " + maxHorizontal + "  " + maxVertical);
			
			int[] returnedValue = new int[]{maxHorizontal,maxVertical };
			return returnedValue;
		}

	}
}
