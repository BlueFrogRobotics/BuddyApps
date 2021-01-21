using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TestHDCam
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestHDCamBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestHDCamData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TestHDCamActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TestHDCamData.Instance;
        }
    }
}