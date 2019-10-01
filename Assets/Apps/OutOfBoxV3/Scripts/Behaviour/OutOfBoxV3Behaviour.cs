using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.OutOfBoxV3
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class OutOfBoxV3Behaviour : MonoBehaviour
    {
        public Dropdown PhaseDropDown;

        [SerializeField]
        private Button HideDropDown;



        /*
         * Data of the application. Save on disc when app is quitted
         */
        private OutOfBoxV3Data mAppData;


        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            OutOfBoxV3Activity.Init(null);

            /*
			* Init your app data
			*/
            //Faire la partie qui change le outofboxdata.phase par rapport à la phase que lance le user
            mAppData = OutOfBoxV3Data.Instance;
            HideDropDown.onClick.AddListener(() => PhaseDropDown.gameObject.SetActive(!PhaseDropDown.gameObject.activeSelf));
        }
    }
}