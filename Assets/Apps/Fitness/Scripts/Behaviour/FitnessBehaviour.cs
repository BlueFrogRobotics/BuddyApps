using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Fitness
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class FitnessBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private FitnessData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			FitnessActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = FitnessData.Instance;
        }
    }
}