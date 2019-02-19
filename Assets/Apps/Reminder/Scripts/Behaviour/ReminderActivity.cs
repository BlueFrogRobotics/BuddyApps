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

            Debug.Log("----- INIT REMINDER DATA -----");
            ReminderDateManager.GetInstance().Initialize();

            if (iArgs == null || (iArgs != null && iArgs.Length != 1))
                return;

            Debug.Log("----- AFTER CHECK -----");

            /*
            ** There is a CompanionInput, so after InitState a PreProcessing state will occured.
            ** During PreProcessingState, CompanionInput is analyzed to find out what information is missing.
            ** Then the StateMachine is redirected to the right state, to complete the missing reminder informations.
            */

            //ReminderDateManager.GetInstance().CompanionInput = (SpeechInput)iArgs[0];

            Debug.Log("----- AFTER INIT COMPANION INPUT -----");

            if (ReminderDateManager.GetInstance().CompanionInput != null)
            {
                if (ReminderDateManager.GetInstance().CompanionInput.Rule != null)
                    Debug.LogWarning("COMPANION_RULE:" + ReminderDateManager.GetInstance().CompanionInput.Rule);
                if (ReminderDateManager.GetInstance().CompanionInput.Utterance != null)
                    Debug.LogWarning("COMPANION_UTTERANCE:" + ReminderDateManager.GetInstance().CompanionInput.Utterance);
            }
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
            ExtLog.I(ExtLogModule.APP, typeof(ReminderActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
            
            Buddy.Vocal.DefaultInputParameters = null;
            Buddy.Vocal.OnEndListening.Clear();

            Buddy.Vocal.StopAndClear();
            Buddy.GUI.Screen.OnTouch.Clear();
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
        }
    }
}