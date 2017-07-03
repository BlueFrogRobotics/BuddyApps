using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

using Buddy;


namespace BuddyApp.Companion
{
	/// <summary>
	/// Behavior to make Buddy's eyes follow a hot source on thermal sensor
	/// </summary>
	public class EyesFollowThermal : MonoBehaviour
	{

		//external variables
		private ThermalDetector mThermalDetector;
		private Face mFace;

		// local variables
		private int targetToLookAtH;
		private int targetToLookAtV;
		private int previousEyesTargetPositionH;
		private int previousEyesTargetPositionV;
		private int curiousTargetH;
		private int curiousTargetV;
		private int stepVisionTargetH;
		private int stepVisionTargetV;
		private float mTimeLastOrder;
		private const float TIME_BEFORE_BEING_CURIOUS = 5.0f;


		// Use this for initialization
		void Start()
		{
			mThermalDetector = GetComponent<ThermalDetector>();
			mFace = BYOS.Instance.Interaction.Face;

			// eyes looking straight
			targetToLookAtH = Screen.width / 2;
			targetToLookAtV = Screen.height / 2;
			previousEyesTargetPositionH = Screen.width / 2;
			previousEyesTargetPositionV = Screen.height / 2;

			// those value won't change 
			stepVisionTargetH = Screen.width / 8;
			stepVisionTargetV = Screen.height / 8;

			curiousTargetH = Screen.width / 8;
			curiousTargetV = Screen.height / 8;

			mTimeLastOrder = Time.time;
		}

		// Update is called once per frame
		void Update()
		{

			// Following the hotspot when there is one
			int[] position = mThermalDetector.PositionHotSpot;
			//mThermalDetector.PositionHotSpot gives : new int[]{maxHorizontal,maxVertical};
			int maxHorizontal = position[0];
			int maxVertical = position[1];

			if (maxHorizontal == -1 || maxVertical == -1) { // if we don't find anybody to look at

				if (Time.time - mTimeLastOrder > TIME_BEFORE_BEING_CURIOUS) {
					// set a curiosityPoint
					curiousTargetH = Random.Range(0, Screen.width);
					curiousTargetV = Random.Range(0, Screen.height);
					mTimeLastOrder = Time.time;
				}
				targetToLookAtH = curiousTargetH;
				targetToLookAtV = curiousTargetV;
				// check for a time since last order from thermal
				// if this time is more than a threshold we give a target and a time for the eyes
				// reset the last time order
			} else { //hot spot has been detected so we target that

				//Debug.Log ("hot spot is detected at " + maxHorizontal + "  " + maxVertical);

				// Conversion into screen position
				// maxHorizontal means the line which is the hottest so it becomes vertical when projected
				targetToLookAtH = (1 + maxVertical * 2) * stepVisionTargetH;
				targetToLookAtV = (1 + maxHorizontal * 2) * stepVisionTargetV;
				curiousTargetH = targetToLookAtH;
				curiousTargetV = targetToLookAtV;
				// Debug.Log ("maxHorizontal " + maxHorizontal ); 
				// Debug.Log ("maxVertical " + maxVertical ); 

				//reset the timer 
				mTimeLastOrder = Time.time;
			}

			// Filtering
			targetToLookAtH = (int)(0.1 * (double)targetToLookAtH + 0.9 * (double)previousEyesTargetPositionH);
			previousEyesTargetPositionH = targetToLookAtH;
			targetToLookAtV = (int)(0.1 * (double)targetToLookAtV + 0.9 * (double)previousEyesTargetPositionV);
			previousEyesTargetPositionV = targetToLookAtV;
			// setting the target for the eyes
			mFace.LookAt(Screen.width - targetToLookAtH, Screen.height - targetToLookAtV);
		}
	}
}

