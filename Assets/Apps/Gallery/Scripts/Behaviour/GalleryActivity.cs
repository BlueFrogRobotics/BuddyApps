using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Gallery
{
    /* This class contains useful callback during your app process */
    public class GalleryActivity : AAppActivity
    {
        [SerializeField]
        private readonly string STR_WELCOME = "welcome";
        private readonly string STR_WELCOME_FROM_APP = "welcomefromapp";
        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
            
            // TODO: Deal with parameters (ie, name of an image, name of an application?)
            
            if (1 == iArgs.Length && typeof(string) == iArgs[1].GetType())
            {
                // TODO: Initialize with last parameter
                PhotoManager.GetInstance().Initialize();
                return;
            }

            // In case of error or no parameter, use default initialization
            PhotoManager.GetInstance().Initialize();

            Buddy.Vocal.SayKey(STR_WELCOME);
        }
        
        /*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }
    }
}