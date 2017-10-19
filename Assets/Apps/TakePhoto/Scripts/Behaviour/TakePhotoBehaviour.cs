using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TakePhoto
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TakePhotoBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TakePhotoData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TakePhotoActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TakePhotoData.Instance;
        }
    }
}