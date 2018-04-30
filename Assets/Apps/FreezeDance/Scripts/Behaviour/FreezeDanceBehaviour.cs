using UnityEngine.UI;
using UnityEngine;
using System;

using Buddy;

namespace BuddyApp.FreezeDance
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class FreezeDanceBehaviour : MonoBehaviour
    {

        private MotionDetection mMotion;
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
            mMotion = BYOS.Instance.Perception.Motion;
            if(!BYOS.Instance.Primitive.RGBCam.IsOpen)
                BYOS.Instance.Primitive.RGBCam.Open(RGBCamResolution.W_176_H_144);
            
            mMotion.OnDetect(OnMovementInternal, 3);
            BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

        /*
         * A sample of use of data (here for basic display purpose)
         */
        void Update()
        {
        }

        private bool OnMovementInternal(MotionEntity[] iEntities)
        {
            if (OnMovementDetect != null && iEntities.Length>20)
                OnMovementDetect();
            return true;
        }

        
    }
}