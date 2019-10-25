using UnityEngine.UI;
using UnityEngine;
using System;

using BlueQuark;

namespace BuddyApp.FreezeDance
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class FreezeDanceBehaviour : MonoBehaviour
    {

        private MotionDetector mMotionDetection;
        public Action OnMovementDetect;
        private bool mChangeMusic;
        public bool ChangeMusic { get { return mChangeMusic; } set { mChangeMusic = value; } }

        void Awake()
        {
            mChangeMusic = true;
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mMotionDetection = Buddy.Perception.MotionDetector;
            if(!Buddy.Sensors.RGBCamera.IsOpen)
                Buddy.Sensors.RGBCamera.Open(RGBCameraMode.COLOR_320X240_15FPS_RGB);            

            MotionDetectorParameter lMotionParam = new MotionDetectorParameter();
            lMotionParam.RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, 240);
            mMotionDetection.OnDetect.AddP(OnMovementInternal, lMotionParam);
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
        }

        private bool OnMovementInternal(MotionEntity[] iEntities)
        {
            if (OnMovementDetect != null && iEntities.Length>12)
                OnMovementDetect();
            return true;
        }
    }
}