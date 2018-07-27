using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueQuark;

namespace BuddyApp.Diagnostic
{
    public class ConnexionWindow : MonoBehaviour
    {

        [SerializeField]
        private Text connexionStatus;

        [SerializeField]
        private Text wifi;

        [SerializeField]
        Button buttonPing;

        private bool mIsPinging;

        // Use this for initialization
        void Start()
        {
            mIsPinging = false;
            
        }

        // Update is called once per frame
        void Update()
        {

        }


        IEnumerator CheckConnection()
        {
            //mIsPinging = true;
            //const float timeout = 10f;
            //float startTime = Time.timeSinceLevelLoad;
            //var ping = new Ping("8.8.8.8");

            //Buddy.IO.WiFi.StartWifiScan();

            //while (true)
            //{
            //    connexionStatus.text = "Checking network...";
            //    if (ping.isDone)
            //    {
            //        float lPingTime = (Time.timeSinceLevelLoad - startTime)*1000;
            //        connexionStatus.text = "Network available. Ping received in "+ lPingTime+" ms";
            //        mIsPinging = false;
            //        yield break;
            //    }
            //    if (Time.timeSinceLevelLoad - startTime > timeout)
            //    {
            //        connexionStatus.text = "No network.";
            //        mIsPinging = false;
            //        yield break;
            //    }

            //    wifi.text = "wifi: "+Buddy.IO.WiFi.GetCurrentWifiAPName();
            //    yield return new WaitForEndOfFrame();
            //}

            // A delete après avoir tout décommenté
            yield return new WaitForEndOfFrame();
        }

        public void Ping()
        {
            if(!mIsPinging)
                StartCoroutine(CheckConnection());
        }

    }
}