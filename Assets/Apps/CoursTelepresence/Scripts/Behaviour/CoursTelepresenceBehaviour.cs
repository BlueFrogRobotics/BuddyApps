using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CoursTelepresenceBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private CoursTelepresenceData mAppData;


        [SerializeField]
        Button Menu;

        [SerializeField]
        Button Steering;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			CoursTelepresenceActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = CoursTelepresenceData.Instance;

            Menu.onClick.AddListener(SwapSteeringActive);
            Steering.onClick.AddListener(SwapSteeringText);

        }


        private void SwapSteeringActive()
        {
            Steering.gameObject.SetActive(!Steering.gameObject.activeSelf);
        }

        private void SwapSteeringText()
        {
            if (Steering.GetComponentInChildren<Text>().text == "Static Navigation")
                Steering.GetComponentInChildren<Text>().text = "Dynamic Navigation";
            else
                Steering.GetComponentInChildren<Text>().text = "Static Navigation";
        }
    }
}