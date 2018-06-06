using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radioplayer
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RadioplayerBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RadioplayerData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			RadioplayerActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = RadioplayerData.Instance;
        }
    }
}