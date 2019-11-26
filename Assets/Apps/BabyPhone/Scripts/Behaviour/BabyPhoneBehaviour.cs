using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BabyPhone
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BabyPhoneBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private BabyPhoneData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			BabyPhoneActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = BabyPhoneData.Instance;
        }
    }
}