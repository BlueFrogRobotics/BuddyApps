using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TestCoroutineDisplay
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestCoroutineDisplayBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestCoroutineDisplayData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TestCoroutineDisplayActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TestCoroutineDisplayData.Instance;
        }
    }
}