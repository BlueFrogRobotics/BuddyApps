using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Somfy
{
    /* This class contains useful callback during your app process */
    public class SomfyActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(object[] iArgs)
        {
            if (iArgs != null && iArgs.Length>0)
            {
                // We have an input sentence
                SomfyData.Instance.VocalRequest = (string)iArgs[0];
            }
            else
            {
                SomfyData.Instance.VocalRequest = "";
            }
            //	Utils.LogI(LogContext.APP, "On loading...");
            //          if (iStrArgs != null)
            //          {
            //              // We have an input sentence
            //              SomfyData.Instance.VocalRequest = iStrArgs[0];
            //          }
            //          else
            //          {
            //              SomfyData.Instance.VocalRequest = "";
            //          }
        }

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            //if (Objects != null)
            //{
            //    // We have an input sentence
            //    SomfyData.Instance.VocalRequest = (string)Objects[0];
            //}
            //else
            //{
            //    SomfyData.Instance.VocalRequest = "";
            //}
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
            //Utils.LogI(LogContext.APP, "On quit...");
        }
    }
}