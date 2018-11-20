using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    /* This class contains useful callback during your app process */
    public class ReminderActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(ReminderActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
		}

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(ReminderActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(ReminderActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            Debug.Log("----- ON QUIT ... -----");
            StartCoroutine(CloseApp());
            ExtLog.I(ExtLogModule.APP, typeof(ReminderActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }

        public IEnumerator CloseApp()
        {
            Debug.Log("----- WAITING FOR QUIT ... -----");
            yield return new WaitUntil(() => Buddy.GUI.Toaster.IsBusy);
            Debug.Log("----- TOASTER NOT BUSY -----");
            yield return new WaitUntil(() => Buddy.Vocal.IsSpeaking);
            Debug.Log("----- NO TTS, OK TO QUIT -----");
        }
    }
}