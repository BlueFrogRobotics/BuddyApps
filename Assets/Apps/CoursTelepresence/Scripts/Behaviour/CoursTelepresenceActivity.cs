using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    /* This class contains useful callback during your app process */
    public class CoursTelepresenceActivity : AAppActivity
    {
        private RTMManager mRTMManager;
        private RTCManager mRTCManager;

        /*
		* Called before the App scene loading.
		*/
  //      public override void OnLoading(object[] iArgs)
		//{
            
  //          ExtLog.I(ExtLogModule.APP, typeof(CoursTelepresenceActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
		//}

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            Debug.Log("debut onawake"); 
            mRTMManager = (RTMManager)Objects[0];
            mRTCManager = (RTCManager)Objects[1];
            Debug.Log("fin onawake");
            ExtLog.I(ExtLogModule.APP, typeof(CoursTelepresenceActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(CoursTelepresenceActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(CoursTelepresenceActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
            mRTMManager.Logout();
            mRTCManager.Leave();
            mRTCManager.DestroyRTC();
        }
    }
}