using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RedLightGreenLightGameBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RedLightGreenLightGameData mAppData;

        private float mTimer;
        public float Timer { get { return mTimer; } set { mTimer = value; } }

        private bool mIsPlayerPositionning;
        public bool IsPlayerPositionning { get { return mIsPlayerPositionning; } set { mIsPlayerPositionning = value; } }

        private float mTimerClosedEyes;
        public float TimerClosedEyes { get { return mTimerClosedEyes; } set { mTimerClosedEyes = value; } }

        private float mTimerDetectionPlayer;
        public float TimerDetectionPlayer { get { return mTimerDetectionPlayer; } set { mTimerDetectionPlayer = value; } }

        private float mTargetSpeed;
        public float TargetSpeed { get { return mTargetSpeed; } set { mTargetSpeed = value; } }

        private float mSensibilityDetection;
        public float SensibilityDetection { get { return mSensibilityDetection; } set { mSensibilityDetection = value; } }

        private Texture2D mPictureMoving;
        public Texture2D PictureMoving { get { return mPictureMoving; } set { mPictureMoving = value; } }

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			RedLightGreenLightGameActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = RedLightGreenLightGameData.Instance;
            mTimer = 0.0F;
            mIsPlayerPositionning = false;

        }

        void Update()
        {
            mTimer += Time.deltaTime;
        }

        internal void OpenFlash()
        {
            //open the flash
        }

        internal void CloseFlash()
        {
            //close the flash
        }
    }
}