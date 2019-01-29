using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Korian
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class KorianBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private KorianData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			KorianActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = KorianData.Instance;
        }
    }
}