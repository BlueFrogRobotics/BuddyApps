using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radio
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class RadioBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private RadioData mAppData;

        void Start()
        {
			/*
			* You can setup your App activity here.
			*/
			RadioActivity.Init(null);
			
			/*
			* Init your app data
			*/
            mAppData = RadioData.Instance;

            RadioStream radio = GetComponent<RadioStream>();
            string lToken = radio.GetToken();
            Debug.Log("token: " + lToken);
            //radio.Play("nostalgie", lToken);
            StartCoroutine(AskInfos(radio, lToken));
        }

        private IEnumerator AskInfos(RadioStream iRadio, string iToken)
        {
            RadioInfos lInfos = new RadioInfos();
            yield return iRadio.GetRadioInformations("nostalgie", lInfos, iToken);
            Debug.Log("infos name: " + lInfos.Name);
            Debug.Log("infos logo: " + lInfos.LogoURL);
        }
    }
}