using Buddy;
using Buddy.UI;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Radioplayer
{
    /* This class contains useful callback during your app process */
    public class RadioplayerActivity : AAppActivity
    {
        private RadioStream mStream;

		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
            string lRadio = "";
            if (iStrArgs!=null && iStrArgs.Length>0)
            {
                Debug.Log("recu de radio: " + iStrArgs[0]);
                lRadio = ExtractRadio(iStrArgs[0]);
            }
            if (lRadio != "")
                RadioplayerData.Instance.DefaultRadio = lRadio;
            //Debug.Log("la radio: "+ExtractRadio("lance la radio europe 1"));
            //RadioplayerData.Instance.DefaultRadio = "voltage";
        }

		/*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            Utils.LogI(LogContext.APP, "On awake...");
            mStream = (RadioStream)Objects[0];
        }

		/*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            Utils.LogI(LogContext.APP, "On start...");
            BYOS.Instance.Header.DisplayParametersButton = false;
        }

		/*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            mStream.Stop();
            Utils.LogI(LogContext.APP, "On quit...");
        }

        private string ExtractRadio(string iText)
        {
            string[] lWords = iText.Split(' ');
            if (lWords == null)
                return "";
            string lResult = "";
            for(int i=0; i<lWords.Length; i++)
            {
                if (lWords[i].Contains("radio") && i < lWords.Length - 1)
                {
                    for(int j=i+1; j< lWords.Length; j++)
                        lResult += (lWords[j]+" ");
                }

            }
            return lResult;
        }
    }
}