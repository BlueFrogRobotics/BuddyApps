using Buddy;
using Buddy.UI;

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
            public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
        {
            Utils.LogI(LogContext.APP, "On loading...");
            if (iStrArgs != null)
            {
                // We have an input sentence
                ReminderData.Instance.VocalRequest = iStrArgs[0];
            }
            else
            {
                ReminderData.Instance.VocalRequest = "";
            }
        }
		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            Utils.LogI(LogContext.APP, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            Utils.LogI(LogContext.APP, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            Utils.LogI(LogContext.APP, "On quit...");
        }
    }
}