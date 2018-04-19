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
            if (iStrArgs != null && iIntArgs==null)
            {
                // We have an input sentence
                Debug.Log("loading 1");
                ReminderData.Instance.VocalRequest = iStrArgs[0];
                ReminderData.Instance.GiveReminder = false;
            }
            else if(iStrArgs != null && iIntArgs != null)
            {
                Debug.Log("loading 2");
                ReminderData.Instance.GiveReminder = true;
                ReminderData.Instance.SenderID = iIntArgs[0];
                ReminderData.Instance.Date = iStrArgs[0];
                //DateTime myDate;
                //if (!DateTime.TryParse(ReminderData.Instance.Date, out myDate))
                //{
                //    // handle parse failure
                //}
                //Debug.Log("date du bonheur: " + myDate.ToString());
                Debug.Log("date: " + iStrArgs[0] + " id: " + iIntArgs[0]);
            }
            else
            {
                Debug.Log("loading 3");
                ReminderData.Instance.VocalRequest = "";
                ReminderData.Instance.GiveReminder = false;
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