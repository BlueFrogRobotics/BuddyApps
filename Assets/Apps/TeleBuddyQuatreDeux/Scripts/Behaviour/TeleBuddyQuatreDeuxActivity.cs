using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    /* This class contains useful callback during your app process */
    public class TeleBuddyQuatreDeuxActivity : AAppActivity
    {
        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        public override void OnAwake()
        {
            Debug.LogError("TELEBUDDY AWAKE");
            mRTMManager = (RTMManager)Objects[0];
            mRTCManager = (RTCManager)Objects[1];
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
            Debug.LogError("TELEBUDDY FIN AWAKE");
        }

        /*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");

        }

        /*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            Debug.LogError("TELEBUDDY QUIT");
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
            mRTMManager.Logout();
            mRTCManager.Leave();
        }
    }
}