using BlueQuark;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BuddyApp.TakePhoto
{
    /* This class contains useful callback during your app process */
    public sealed class TakePhotoActivity : AAppActivity
    {
        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iArgs)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On loading...");
            if (!Directory.Exists(Buddy.Platform.Application.PersistentDataPath + "Shared"))
                Directory.CreateDirectory(Buddy.Platform.Application.PersistentDataPath + "Shared");

        }

        /*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LEAVING, "On quit...");
        }
    }
}