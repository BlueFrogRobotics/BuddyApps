using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Tells if someone is forcing Buddy's head in a direction that is not set by the running programm
    /// </summary>
    public class HeadForcedDetector : MonoBehaviour
    {
        public bool HeadForcedDetected { get { return mHeadForcedDetected; } }

        public const float ANGLE_THRESH = 7.0F;

        private bool mHeadForcedDetected;
        private bool mCommandExecuting;
        private float mPreviousDestAngle;
        private Hinge mHinge;

        void Start()
        {
            mHinge = BYOS.Instance.Motors.NoHinge;
            mHeadForcedDetected = false;
            mCommandExecuting = false;
            mPreviousDestAngle = mHinge.DestinationAnglePosition;
        }
        
        void Update()
        {
            //Check if Buddy is executing a changing head position command
            if (mPreviousDestAngle != mHinge.DestinationAnglePosition) {
                mCommandExecuting = true;
                mPreviousDestAngle = mHinge.DestinationAnglePosition;
            }

            //We decided not to detect any interaction while a command is being executed
            //This means the user has to wait for Buddy to stop moving his head for the detector to be effective
            if (mCommandExecuting) {
                mHeadForcedDetected = false;
                if (Mathf.Abs(mHinge.CurrentAnglePosition - mHinge.DestinationAnglePosition) > ANGLE_THRESH)
                    return;
                else
                    mCommandExecuting = false;
            }

            //If there is a difference between the set position and the current one, the head is being forced
            if (Mathf.Abs(mHinge.CurrentAnglePosition - mHinge.DestinationAnglePosition) > ANGLE_THRESH * 2)
                mHeadForcedDetected = true;
            else
                mHeadForcedDetected = false;
        }

        public void MoveHeadLeft()
        {
            float lDestination = mHinge.DestinationAnglePosition + 10.0F;
            mHinge.SetPosition(lDestination);
        }

        public void MoveHeadRight()
        {
            float lDestination = mHinge.DestinationAnglePosition - 10.0F;
            mHinge.SetPosition(lDestination);
        }
    }
}