using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Tutorial
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TutorialBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TutorialData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TutorialActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TutorialData.Instance;
        }
    }
}