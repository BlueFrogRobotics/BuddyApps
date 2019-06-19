using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Wikipedia
{
    /* This class contains useful callback during your app process */
    public class WikipediaActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(WikipediaActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
            WikipediaData.Instance.Utterance = (null != iArgs && 0 < iArgs.Length && typeof(string) == iArgs[0].GetType()) ? (string)iArgs[0] : null;
        }

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(WikipediaActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(WikipediaActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(WikipediaActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }
    }
}