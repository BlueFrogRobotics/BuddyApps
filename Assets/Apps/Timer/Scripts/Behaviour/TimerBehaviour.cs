using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Timer
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TimerBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TimerData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TimerActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TimerData.Instance;
        }
    }
}