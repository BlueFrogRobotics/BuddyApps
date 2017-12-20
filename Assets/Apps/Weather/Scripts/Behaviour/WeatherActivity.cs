using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Weather
{
    /* This class contains useful callback during your app process */
    public class WeatherActivity : AAppActivity
    {
		/*
		* Called before the App scene loading.
		*/
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
			if (iStrArgs != null) {
				// We have an input sentence
				WeatherData.Instance.VocalRequest = iStrArgs[0];
			}else {
				WeatherData.Instance.VocalRequest = "";
			}

		}

		/*
		* Callsed after every Awake() in your scene
		*/
		public override void OnAwake()
        {
            Utils.LogI(LogContext.APP, "On awake...");
            BYOS.Instance.Resources.LoadAtlas("Atlas_Meteo");
        }

		/*
		* Called after every (syncrhone) Start() in your scene
		*/
        public override void OnStart()
        {
            Utils.LogI(LogContext.APP, "On start...");
        }

		/*
		* Called when App is leaving. All coroutine has been stoped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            Utils.LogI(LogContext.APP, "On quit...");
        }
    }
}