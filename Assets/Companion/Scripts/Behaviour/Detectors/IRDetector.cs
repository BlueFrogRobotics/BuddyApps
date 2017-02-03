using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Tells if Infrared sensors detected somehting
    /// </summary>
    public class IRDetector : MonoBehaviour
    {
        public bool IRDetected { get { return mIRDetected; } }

        private bool mIRDetected;
        private IRSensors mSensors;

        void Start()
        {
            mIRDetected = false;
            mSensors = BYOS.Instance.IRSensors;
        }
        
        void Update()
        {
            //Check the value of sensors with a set threshold
            mIRDetected = (mSensors.Left.Distance <= 0.70F) || 
                            (mSensors.Middle.Distance <= 0.70F) ||
                            (mSensors.Right.Distance <= 0.70F);
        }
    }
}