using UnityEngine;
using BuddyOS;

namespace BuddyApp.Companion
{
    /// <summary>
    /// Tells if Ultrasound sensors detected somehting
    /// </summary>
    public class USDetector : MonoBehaviour
    {
        public bool USFrontDetected { get { return mUSFrontDetected; } }
        public bool USBackDetected { get { return mUSBackDetected; } }

        private bool mUSFrontDetected;
        private bool mUSBackDetected;
        private USSensors mSensors;

        void Start()
        {
            mUSBackDetected = false;
            mUSFrontDetected = false;
            mSensors = BYOS.Instance.USSensors;
        }

        void Update()
        {
            //Check sensors values against a set threshold
            mUSFrontDetected = (mSensors.Left.Distance <= 0.7F) || (mSensors.Right.Distance <= 0.7F);
            mUSBackDetected = (mSensors.Back.Distance <= 0.7F);
        }
    }
}