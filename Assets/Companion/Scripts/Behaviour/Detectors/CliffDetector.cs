using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Tells when Buddy is on a cliff
    /// </summary>
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
            //Check cliff sensors values and return true when one of them detects a cliff
            mCliffDetected = mSensors.BackLeft.IsCliff || mSensors.BackRight.IsCliff ||
                            mSensors.FrontLeft.IsCliff || mSensors.FrontRight.IsCliff;
        }
    }
}