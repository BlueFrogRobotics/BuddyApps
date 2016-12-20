using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class HeadForcedDetector : MonoBehaviour
    {
        public bool HeadForcedDetected { get { return mHeadForcedDetected; } }

        private bool mHeadForcedDetected;
        private Hinge mHinge;

        void Start()
        {
            mHinge = BYOS.Instance.Motors.NoHinge;
            mHeadForcedDetected = false;
        }
        
        void Update()
        {
            if (Mathf.Abs(mHinge.CurrentAnglePosition - mHinge.DestinationAnglePosition) > 5F)
                mHeadForcedDetected = true;
            else
                mHeadForcedDetected = false;
        }
    }
}