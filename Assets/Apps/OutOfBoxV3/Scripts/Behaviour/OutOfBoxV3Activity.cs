using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.OutOfBoxV3
{
    /* This class contains useful callback during your app process */
    public class OutOfBoxV3Activity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(OutOfBoxV3Activity), LogStatus.START, LogInfo.LOADING, "On loading...");
		}

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(OutOfBoxV3Activity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(OutOfBoxV3Activity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
            OutOfBoxUtilsVThree.Init();
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            OutOfBoxV3Data.Instance.NameOfPhotoTaken = null;
            
            ExtLog.I(ExtLogModule.APP, typeof(OutOfBoxV3Activity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }

    }
}