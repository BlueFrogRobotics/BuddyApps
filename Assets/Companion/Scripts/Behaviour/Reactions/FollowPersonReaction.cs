using UnityEngine;
using System.Collections;

using BuddyOS;
using BuddyOS.Command;
using BuddyFeature.Navigation;

namespace BuddyApp.Companion
{
	public class FollowPersonReaction : MonoBehaviour {

		// TODO : when the robot does not find anybody he should navigate
		// TODO : add the up and down mouvement

		// link to other features
		private ThermalDetector mThermalDetector;
		private NoHinge mNoHinge;
		private YesHinge mYesHinge;
		private Wheels mWheels;
		private IRSensors mIRsensors;
		private USSensors mUSSensors;

		// local variables
		private bool mIsFollowingSomeone;
		private int mPreviousEyesTargetPositionH;
		private int mPreviousEyesTargetPositionV;

		// Use this for initialization
		void Start () {
			mIsFollowingSomeone = false;
			mThermalDetector = GetComponent<ThermalDetector>();
			mPreviousEyesTargetPositionH = Screen.width / 2;
			mPreviousEyesTargetPositionV = Screen.width / 2;
            
			mNoHinge = BYOS.Instance.Motors.NoHinge;
			mYesHinge = BYOS.Instance.Motors.YesHinge;
			mWheels = BYOS.Instance.Motors.Wheels;
			mIRsensors = BYOS.Instance.IRSensors;
			mUSSensors = BYOS.Instance.USSensors;
		}
		
		// Update is called once per frame
		void Update () {
			if (!mThermalDetector.ThermalDetected) {
				//Debug.Log ("no Human Detected");
				return;
			}

			int[] lPosition = mThermalDetector.PositionHotSpot;
			// we use vertical for left and right because it's the vertical projection
			//returnedValue = new int[]{maxHorizontal,maxVertical };
			int lMaxHorizontal = lPosition[0];
			int lMaxVertical = lPosition[1];

			if (lMaxHorizontal == -1 || lMaxVertical == -1){
				//Debug.Log ("We ve got a problem finding the position of the hot spot");
				return;
			}
			else {
				int lWheelSpeedLeft = 0;
				int lWheelSpeedRight = 0;

                // TODO this should be redone when integrating the new thermal sensor
                switch(lMaxVertical)
                {
                    case 0:
                        lWheelSpeedRight += 80;
                        lWheelSpeedLeft -= 80;
                        break;

                    case 1:
                        lWheelSpeedRight += 40;
                        lWheelSpeedLeft -= 40;
                        break;

                    case 2:
                        lWheelSpeedLeft += 40;
                        lWheelSpeedRight -= 40;
                        break;

                    case 3:
                        lWheelSpeedLeft += 80;
                        lWheelSpeedRight -= 80;
                        break;

                    default:
                        break;
                }
                /* TODO if the thermal tracking is working we can remove this block
				// we use vertical for left and right because it's the verticl projection
				if(maxVertical == 3 || maxVertical == 2)
				{
					//Debug.Log("I saw someone at my right > " + maxHorizontal + " " + maxVertical);
					//Debug.Log ("I saw someone at my right");
					// on tourne la tete a droite
					//mNoHinge.SetPosition(mNoHinge.CurrentAnglePosition + 10);
					// et le robot a droite
					wheelSpeedLeft += 80;
					wheelSpeedRight -= 80;
				}
				else if(maxVertical == 0 || maxVertical == 1)
				{
					//Debug.Log("I saw someone at my left > " + maxHorizontal + " " + maxVertical);
					//Debug.Log("I saw someone at my left");
					// on tourne la tete a gauche
					//mNoHinge.SetPosition(mNoHinge.CurrentAnglePosition - 10);
					// et le robot a gauche
					wheelSpeedRight += 80;
					wheelSpeedLeft -= 80;
				}
                */

                switch (lMaxVertical)
                {
                    case 0:
                        //we lower the head
                        //Disable if head moves too high
                        //mYesHinge.SetPosition(mYesHinge.CurrentAnglePosition - 10);
                        break;

                    case 1:
                        break;

                    case 2:
                        break;

                    case 3:
                        // we lift the head
                        mYesHinge.SetPosition(mYesHinge.CurrentAnglePosition + 10);
                        break;

                    default:
                        break;
                }
                /*
                if (maxHorizontal == 0) 
				{
					// on baisse la tete
					// mYesHinge.SetPosition(mYesHinge.CurrentAnglePosition - 10);
				}
				if (maxHorizontal == 3) 
				{
					// on monte la tete
					// mYesHinge.SetPosition(mYesHinge.CurrentAnglePosition + 10);
				}
                */

				// if there is room we can go
				if (NoFrontObstacles()) {
					lWheelSpeedLeft += 150;
					lWheelSpeedRight += 150;
				}
				mWheels.SetWheelsSpeed(lWheelSpeedLeft,lWheelSpeedRight,300);

			}
		}

		private bool NoFrontObstacles()
		{
			//Debug.Log ("distance received : " + mIRsensors.Middle.Distance);
			if (mIRsensors.Middle.Distance > 0.4f &&
				mIRsensors.Right.Distance > 0.4f &&
				mIRsensors.Left.Distance > 0.4f && 
				mUSSensors.Left.Distance > 0.4f &&
				mUSSensors.Right.Distance > 0.4f)
				return true;
			return false;
		}
	}
}
