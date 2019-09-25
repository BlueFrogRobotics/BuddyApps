using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.DailyInfo
{
    /* This class contains useful callback during your app process */
    public class DailyInfoActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(object[] iArgs)
		{ 
			ExtLog.I(ExtLogModule.APP, typeof(DailyInfoActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
            if (iArgs != null && iArgs.Length > 1)
            {
                DailyInfoData.Instance.InfosFileName = (string)iArgs[0];
                DailyInfoData.Instance.VocalRequest = (string)iArgs[1];
            }
            else
            {
                //DailyInfoData.Instance.InfosFileName = "menu";
                DailyInfoData.Instance.InfosFileName = "program";
                DailyInfoData.Instance.VocalRequest = "quel est le programme aujourd'hui";
            }
        }

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(DailyInfoActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(DailyInfoActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(DailyInfoActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }
    }
}