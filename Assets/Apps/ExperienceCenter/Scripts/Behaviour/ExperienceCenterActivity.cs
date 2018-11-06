using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.ExperienceCenter
{
    /* This class contains useful callback during your app process */
    public class ExperienceCenterActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		//public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		//{ 
		//	Utils.LogI(LogContext.APP, "On loading...");
		//}

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            //Utils.LogI(LogContext.APP, "On awake...");
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            //Utils.LogI(LogContext.APP, "On start...");
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
			TcpServer lTcpServer =  GameObject.Find ("AIBehaviour").GetComponent<TcpServer> ();
			lTcpServer.StopServer ();
            //Utils.LogI(LogContext.APP, "On quit...");
        }
    }
}