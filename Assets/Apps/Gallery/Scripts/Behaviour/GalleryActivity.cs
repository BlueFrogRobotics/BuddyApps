using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
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
        private readonly string STR_APP_NAME = "app_name";

        [SerializeField]
        private object[] iArgs = null;

        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.START, LogInfo.LOADING, "On loading...");

            // TODO: Deal with parameters (ie, name of an image, name of an application?)
            this.iArgs = iArgs;
            
            string strAppName = (null != iArgs && 0 < iArgs.Length && typeof(string) == iArgs[0].GetType()) ? (string)iArgs[0] : null;
            string strFirstPhotoName = (null != iArgs && 1 < iArgs.Length && typeof(string) == iArgs[1].GetType()) ? (string)iArgs[1] : null;

            // Initialize photo manager (scan directoies)
            PhotoManager.GetInstance().Initialize(strAppName, strFirstPhotoName);
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

            string strAppName = (null != iArgs && 0 < iArgs.Length && typeof(string) == iArgs[0].GetType()) ? ((string)iArgs[0].ToString()) : null;

            // Vocal initialization and welcome
            //Buddy.GUI.Screen.OnTouch.Add((iTouch) => { Buddy.Vocal.StopListening(); });
            Buddy.Vocal.OnTrigger.Add((iAction) => Buddy.Vocal.SayKeyAndListen("ilisten"));

            if (null == strAppName) // At least 2 parameters
            {
                Buddy.Vocal.SayKey(STR_WELCOME);
            }
            else
            {
                Buddy.Vocal.Say(Buddy.Resources.GetRandomString(STR_WELCOME_FROM_APP).Replace(STR_APP_NAME, strAppName));
            }
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(GalleryActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");

            Buddy.Vocal.OnEndListening.Clear();

            if (Buddy.Vocal.IsBusy)
            {
                Buddy.Vocal.Stop();
            } 

            PhotoManager.GetInstance().Free();
        }
    }
}