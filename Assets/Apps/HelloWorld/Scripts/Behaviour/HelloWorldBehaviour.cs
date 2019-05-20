using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.HelloWorld
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class HelloWorldBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private HelloWorldData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			HelloWorldActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = HelloWorldData.Instance;
        }
    }
}