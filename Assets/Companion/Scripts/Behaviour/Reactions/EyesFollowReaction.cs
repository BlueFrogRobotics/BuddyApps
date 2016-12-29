using UnityEngine;
using System.Collections;

using BuddyOS;

namespace BuddyApp.Companion
{
	public class EyesFollowReaction : MonoBehaviour {

		//external variables
		private ThermalDetector mThermalDetector;
		private Face mFace;

		// local variables
		private int targetToLookAtH;
		private int targetToLookAtV;
		private int previousEyesTargetPositionH;
		private int previousEyesTargetPositionV;
		private int stepVisionTargetH;
		private int stepVisionTargetV;

		// Use this for initialization
		void Start () {
			mThermalDetector = GetComponent<ThermalDetector>();
			mFace = BYOS.Instance.Face;

			// eyes looking straight
			targetToLookAtH = Screen.width / 2;
			targetToLookAtV = Screen.height / 2;
			previousEyesTargetPositionH = Screen.width / 2;
			previousEyesTargetPositionV = Screen.height / 2;

			// those value won't change 
			stepVisionTargetH = Screen.width / 8;
			stepVisionTargetV = Screen.height / 8;
		}
		
		// Update is called once per frame
		void Update () {
			
			int[] position = mThermalDetector.PositionHotSpot;
			//returnedValue = new int[]{maxHorizontal,maxVertical };
			int maxHorizontal = position[0];
			int maxVertical = position[1];
			if (maxHorizontal == -1 || maxVertical == -1){
				targetToLookAtH = Screen.width / 2;
				targetToLookAtV = Screen.height / 2;
                Debug.Log("nobody to look at, trying to look straigt");
				// TODO : when not whatching something for a while the robot should look around
			}
			else {
				//Debug.Log ("hot spot is detected at " + maxHorizontal + "  " + maxVertical);

				// Convertion into screen position
				// maxHorizontal means the line wich is the hotter so it become vertical when projected
				targetToLookAtH = (1 + maxVertical*2) * stepVisionTargetH;
				targetToLookAtV = (1 + maxHorizontal*2) * stepVisionTargetV;

				// Debug.Log ("maxHorizontal " + maxHorizontal ); 
				// Debug.Log ("maxVertical " + maxVertical ); 
			}

			// Filtering
			targetToLookAtH = (int)(0.1 * (double)targetToLookAtH + 0.9 * (double)previousEyesTargetPositionH);
			previousEyesTargetPositionH = targetToLookAtH;
			targetToLookAtV = (int)(0.1 * (double)targetToLookAtV + 0.9 * (double)previousEyesTargetPositionV);
			previousEyesTargetPositionV = targetToLookAtV;
			// setting the target for the eyes
			mFace.LookAt (Screen.width - targetToLookAtH, Screen.height - targetToLookAtV);

		}
	}
}
