using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class GalleryBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private GalleryData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			GalleryActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = GalleryData.Instance;
        }
    }
}