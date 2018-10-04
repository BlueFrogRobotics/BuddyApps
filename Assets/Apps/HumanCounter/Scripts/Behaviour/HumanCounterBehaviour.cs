using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.HumanCounter
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class HumanCounterBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private HumanCounterData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			HumanCounterActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = HumanCounterData.Instance;
        }
    }
}