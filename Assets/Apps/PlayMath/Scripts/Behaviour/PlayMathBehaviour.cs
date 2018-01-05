using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.PlayMath
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class PlayMathBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private PlayMathData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			PlayMathActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = PlayMathData.Instance;
        }
    }
}