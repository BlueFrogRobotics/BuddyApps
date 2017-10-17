using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLight
{
    /// <summary>
    /// Movement manager of the application RLGL. This class has reference to the differents stimuli and subscribes/unsubscribes to their callback.
    /// </summary>
    public class RLGLMovementManager : MonoBehaviour
    {
        private MotionDetection mMotionDetection;


        private bool mIsMovementDetected;
        public bool IsMovementDetected { get { return mIsMovementDetected; } set { mIsMovementDetected = value; } }

        // Use this for initialization
        void Start()
        {
            mMotionDetection = BYOS.Instance.Perception.Motion;
        }

        /// <summary>
        /// Subscribe to the movement stimuli callback
        /// </summary>
        public void LinkDetectorEvent()
        {
            mMotionDetection.OnDetect(OnMovementDetected);
        }

        /// <summary>
        /// Unsubscribe to the movement stimuli callback
        /// </summary>
        public void UnlinkDetectorEvent()
        {
            mMotionDetection.StopAllOnDetect();
        }


        private bool OnMovementDetected(MotionEntity[] iMotionEntity)
        {
            if(!mIsMovementDetected)
                return true;

            return mIsMovementDetected;
        }
    }
}

