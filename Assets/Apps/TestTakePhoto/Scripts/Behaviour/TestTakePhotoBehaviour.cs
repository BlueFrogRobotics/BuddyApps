using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TestTakePhoto
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestTakePhotoBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestTakePhotoData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TestTakePhotoActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TestTakePhotoData.Instance;
        }
    }
}