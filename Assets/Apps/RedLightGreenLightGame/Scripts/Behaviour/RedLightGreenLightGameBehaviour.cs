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

        private bool mTargetClicked;
        public bool TargetClicked { get { return mTargetClicked; } set { mTargetClicked = value; } }

        private bool mFirstTurn;
        public bool FirstTurn { get { return mFirstTurn; } set { mFirstTurn = value; } }

        private bool mGameplay;
        public bool Gameplay { get { return mGameplay; } set { mGameplay = value; } }

        private int mLife;
        public int Life { get { return mLife; } set { if (value < 0) mLife = 0; else mLife = value; } }

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
            mTargetClicked = false;
            mFirstTurn = false;
            mGameplay = false;
            Life = 3;
            Debug.Log("contient oui: " + BYOS.Instance.Dictionary.ContainsPhonetic("non", "yes"));
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

        public void OnClickTarget()
        {
            Debug.Log("TARGET CLICKED!");
            mTargetClicked = true;
        }
    }
}