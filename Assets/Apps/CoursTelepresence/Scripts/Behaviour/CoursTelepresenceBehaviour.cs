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
        //public const string APP_ID = "7b13f4916a6b43e0b23958b18926d596";

        private void Awake()
        {
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();

            CoursTelepresenceActivity.Init(null, mRTMManager, mRTCManager);
        }

        void Start()
        {
            mAppData = CoursTelepresenceData.Instance;
        }


    }
}