using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TacheAskUserKORIAN
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TacheAskUserKORIANBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TacheAskUserKORIANData mAppData;



        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			TacheAskUserKORIANActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = TacheAskUserKORIANData.Instance;
        }
    }
}