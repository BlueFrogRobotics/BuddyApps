using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.BuddyLab
{
    /* This class contains useful callback during your app process */
    public sealed class BuddyLabActivity : AAppActivity
    {
        private ItemControlUnit mItemControl;


		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            //ExtLog.I(Context.APP, "On awake...");
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.RUNNING, "On awake...");
            mItemControl =(ItemControlUnit)Objects[0];
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
        }
    }
}