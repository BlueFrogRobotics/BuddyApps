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
        private string lId;
        private string lChannelId;

        public override void OnAwake()
        {
            Debug.Log("TELEBUDDY AWAKE");
            mRTMManager = (RTMManager)Objects[0];
            mRTCManager = (RTCManager)Objects[1];
            mRTMManager.IdFromLaunch = lId;
            mRTMManager.ChannelIdFromLaunch = lChannelId;
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
            Debug.Log("TELEBUDDY FIN AWAKE");
        }

        public override void OnLoading(object[] iArgs)
        {
            Debug.LogError("TELEBUDDY parametre on loading");
            if (iArgs.Length > 1)
            {
                Debug.LogError("TELEBUDDY parametre charge: " + (string)iArgs[0] + " channel " + (string)iArgs[1]);
                lId = (string)iArgs[0];
                lChannelId = (string)iArgs[1];
            }
            else
            {
                lId = "";
                lChannelId = "";
            }
        }

        /*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.SUCCESS, LogInfo.LOADING, "TELEBUDDY On start...");

        }

        /*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            Debug.Log("TELEBUDDY QUIT");
            ExtLog.I(ExtLogModule.APP, typeof(TeleBuddyQuatreDeuxActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
            mRTMManager.Logout();
            mRTCManager.Leave();
        }
    }
}