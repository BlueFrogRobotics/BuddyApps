using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.AutomatedTest
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class AutomatedTestBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private AutomatedTestData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			AutomatedTestActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = AutomatedTestData.Instance;
        }
    }
}