using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ReminderBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private ReminderData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			ReminderActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = ReminderData.Instance;
        }
    }
}