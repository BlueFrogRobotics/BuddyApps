using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    public class CliffDetector : MonoBehaviour
    {
        public bool CliffDetected { get { return mCliffDetected; } }

        private bool mCliffDetected;
        private CliffSensors mSensors;

        void Start()
        {
            mCliffDetected = false;
            mSensors = BYOS.Instance.CliffSensors;
        }

        void Update()
        {
            mCliffDetected = mSensors.BackLeft.IsCliff || mSensors.BackRight.IsCliff ||
                            mSensors.FrontLeft.IsCliff || mSensors.FrontRight.IsCliff;
        }
    }
}