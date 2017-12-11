using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.ExperienceCenter
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ExperienceCenterBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private ExperienceCenterData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			ExperienceCenterActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = ExperienceCenterData.Instance;
        }
    }
}