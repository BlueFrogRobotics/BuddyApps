using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radio
{
    /* This class contains useful callback during your app process */
    public class RadioActivity : AAppActivity
    {
        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(RadioActivity), LogStatus.START, LogInfo.LOADING, "On loading...");


            // Get radio name from vocal request
            if (iArgs != null && iArgs.Length > 0)
            {
                RadioData.Instance.SetRadio((string)iArgs[0]);
            }
        }

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(RadioActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(RadioActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(RadioActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");

            // Stop radio webservices
            Buddy.WebServices.Radio.Stop();

            // Disable echo cancellation if it wasn't on at start
            if (RadioData.Instance.StopEchoCancellation)
                Buddy.Sensors.Microphones.EnableEchoCancellation = false;

            Buddy.Vocal.OnEndListening.Clear();
        }

        
    }
}