using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.ChangeXMLTeleBuddy
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class ChangeXMLTeleBuddyBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private ChangeXMLTeleBuddyData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			ChangeXMLTeleBuddyActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = ChangeXMLTeleBuddyData.Instance;
        }
    }
}