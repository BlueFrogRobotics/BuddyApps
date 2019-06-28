using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TestEchoCancel
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class TestEchoCancelBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */
        private TestEchoCancelData mAppData;

        void Start()
        {
            /*
			* You can setup your App activity here.
			*/
            TestEchoCancelActivity.Init(null);

            /*
			* Init your app data
			*/
            mAppData = TestEchoCancelData.Instance;

            Buddy.Vocal.EnableTrigger = true;
            Buddy.Vocal.ListenOnTrigger = true;
        }

        public void OnToggleMusic()
        {

            int lRand = UnityEngine.Random.Range(1, 9);
            string lFile = "os_dance_0" + lRand;

            if (Buddy.Actuators.Speakers.IsBusy)
                Buddy.Actuators.Speakers.Media.Stop();
            else {
                Buddy.Actuators.Speakers.Media.Play(Buddy.Resources.Get<AudioClip>(lFile, Context.OS));
            }
        }

        public void OnToggleListen()
        {
            if (Buddy.Vocal.IsBusy)
                Buddy.Vocal.Stop();
            else
                Buddy.Vocal.Listen();

        }

        public void OnToggleEchoCancel()
        {
            Buddy.Sensors.Microphones.EnableEchoCancellation = !Buddy.Sensors.Microphones.EnableEchoCancellation;
            Buddy.Vocal.Say("echo cancel " + Buddy.Sensors.Microphones.EnableEchoCancellation);
        }

    }
}