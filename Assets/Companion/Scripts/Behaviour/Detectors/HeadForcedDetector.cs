using UnityEngine;
using UnityEngine.UI;
using BuddyOS;

namespace BuddyApp.Companion
{
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
            if (mPreviousDestAngle != mHinge.DestinationAnglePosition) {
                mCommandExecuting = true;
                mPreviousDestAngle = mHinge.DestinationAnglePosition;
            }

            if (mCommandExecuting) {
                mHeadForcedDetected = false;
                if (Mathf.Abs(mHinge.CurrentAnglePosition - mHinge.DestinationAnglePosition) > ANGLE_THRESH)
                    return;
                else
                    mCommandExecuting = false;
            }

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