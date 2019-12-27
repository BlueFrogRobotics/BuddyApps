using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Scheduler
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class SchedulerBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private SchedulerData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			SchedulerActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = SchedulerData.Instance;
        }
    }
}