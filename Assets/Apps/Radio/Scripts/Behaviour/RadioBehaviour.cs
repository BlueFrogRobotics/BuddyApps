using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radio
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public sealed class RadioBehaviour : MonoBehaviour
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
            yield return iRadio.GetRadioInformations("europe_1_9144", lInfos, iToken);
            Debug.Log("info: " + lInfos);
            RadioList lRadioList = new RadioList();
            //List<RadioInfos> lRadios = new List<RadioInfos>();
            yield return iRadio.SearchRadioName("europe 1", iToken, lRadioList);

            StreamList lStreamList = new StreamList();
            yield return iRadio.GetRadiosStreams("europe_1_9144", iToken, lStreamList);
            Debug.Log(lStreamList);

            ShowInfos lShowInfos = new ShowInfos();
            yield return iRadio.GetLiveInformations("europe_1_9144", iToken, lShowInfos);
            Debug.Log(lShowInfos);
            //Debug.Log("radios lenght" + lRadioList.Radios.Count);
            //Debug.Log("liiiiiiiink: " + lRadioList.Radios[0].Permalink);
            //Debug.Log("infos name: " + lInfos.Name);
            //Debug.Log("infos logo: " + lInfos.LogoURL);
        }
    }
}