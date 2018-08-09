using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BuddyLab
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class BuddyLabBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private BuddyLabData mAppData;

        private string mNameOpenProject;
        public string NameOpenProject { get { return mNameOpenProject; } set { mNameOpenProject = value; } }

        [SerializeField]
        private ItemControlUnit itemControlUnit;

        void Awake()
        {
			/*
			* You can setup your App activity here.
			*/
			BuddyLabActivity.Init(null, itemControlUnit);
			
			/*
			* Init your app data
			*/
            mAppData = BuddyLabData.Instance;
        }

    }
}