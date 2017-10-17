using UnityEngine.UI;
using UnityEngine;
using System;

using Buddy;

namespace BuddyApp.FreezeDance
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class FreezeDanceBehaviour : MonoBehaviour
    {
        private Dictionary mDico;
        private TextToSpeech mTTS;
        private Face mFace;
        private float mTime;

        private bool mIsMoving;
        private bool mIsOccupied;
        private bool mStartMusic;
        private bool mPauseMusic;
        private bool mIsSad;
        private bool mNeutral;
        private bool mIsOnGame;
        private bool mSayOnce;
        private bool mChrono;
        private float mAudioClipLength;
        private bool mIsSetRandomStop;
        private float mRandomStopDelay;
        private float mElapsedTime;

        private AudioSource speaker;
        private MotionDetection mMotion;
        public Action OnMovementDetect;

        void Awake()
        {
            mDico = BYOS.Instance.Dictionary;
        }

        /*
         * Init refs to API and your app data
         */
        void Start()
        {
            mMotion = BYOS.Instance.Perception.Motion;
            if(!BYOS.Instance.Primitive.RGBCam.IsOpen)
                BYOS.Instance.Primitive.RGBCam.Open(RGBCamResolution.W_176_H_144);
            
            mMotion.OnDetect(OnMovementInternal, 30);
            BYOS.Instance.Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            speaker = gameObject.GetComponent<AudioSource>();
            mIsSad = false;
            mIsOnGame = false;
            mSayOnce = false;
            mChrono = true;
            mStartMusic = false;
            mIsSetRandomStop = false;
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