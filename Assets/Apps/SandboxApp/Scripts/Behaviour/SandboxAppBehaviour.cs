using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.SandboxApp
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class SandboxAppBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private SandboxAppData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			SandboxAppActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = SandboxAppData.Instance;
        }


    }
}