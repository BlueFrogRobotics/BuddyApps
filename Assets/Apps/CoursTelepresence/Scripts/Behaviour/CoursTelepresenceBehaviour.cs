using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CoursTelepresenceBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private CoursTelepresenceData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			CoursTelepresenceActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = CoursTelepresenceData.Instance;
        }
    }
}