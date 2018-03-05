using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.SharedWIP
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class SharedWIPBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private SharedWIPData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			SharedWIPActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = SharedWIPData.Instance;
        }
    }
}