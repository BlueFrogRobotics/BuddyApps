using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.OutOfBox
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class OutOfBoxBehaviour : MonoBehaviour
    {
        public Dropdown PhaseDropDown;
        
        [SerializeField]
        private Button HideDropDown;

        /*
         * Data of the application. Save on disc when app is quitted
         */
        private OutOfBoxData mAppData;


        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			OutOfBoxActivity.Init(null);
			
			/*
			* Init your app data
			*/
            //Faire la partie qui change le outofboxdata.phase par rapport à la phase que lance le user
            mAppData = OutOfBoxData.Instance;

            HideDropDown.onClick.AddListener(() => PhaseDropDown.gameObject.SetActive(!PhaseDropDown.gameObject.activeSelf));
        }
    }
}