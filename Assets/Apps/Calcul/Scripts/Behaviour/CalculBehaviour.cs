using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Calcul
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class CalculBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private CalculData mAppData;

        public Score Score { get; set; }

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			CalculActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = CalculData.Instance;

            Score = new Score();
        }

        public static string GetVocalEquation(string statement)
        {
            if (statement.Contains("÷"))
                statement = statement.Replace("÷", Buddy.Resources.GetString("dividedby"));
            if (statement.Contains("×"))
                statement = statement.Replace("×", Buddy.Resources.GetString("xtimesy"));
            if (statement.Contains("-"))
                statement = statement.Replace("-", Buddy.Resources.GetString("minus"));
            if (statement.Contains("="))
                statement = statement.Replace("=", Buddy.Resources.GetString("equal"));

            return statement;
        }
    }
}