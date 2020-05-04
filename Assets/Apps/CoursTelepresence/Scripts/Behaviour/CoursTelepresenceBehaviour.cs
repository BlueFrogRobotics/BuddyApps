using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CoursTelepresenceBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private CoursTelepresenceData mAppData;


        private RTMManager mRTMManager;
        private RTCManager mRTCManager;

        public const string APP_ID = "dc949460a57e4fb0990a219b799ccf13";

        private void Awake()
        {
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();

            /*
			* You can setup your App activity here.
			*/
            CoursTelepresenceActivity.Init(null, mRTMManager, mRTCManager);
            Debug.Log("apres init dans awake");
        }

        void Start()
        {
            /*
			* Init your app data
			*/
            mAppData = CoursTelepresenceData.Instance;


        }


    }
}