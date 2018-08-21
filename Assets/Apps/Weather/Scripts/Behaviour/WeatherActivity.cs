using UnityEngine.UI;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.Weather
{
    /* This class contains useful callback during your app process */
    public class WeatherActivity : AAppActivity
    {
        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iInputArgs)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On loading...");
            if (iInputArgs != null) {
                // We have an input sentence
                WeatherData.Instance.VocalRequest = (string)iInputArgs[0];
            } else {
                WeatherData.Instance.VocalRequest = "";
            }

        }

        /*
		* Callsed after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            //Buddy.Resources.LoadAtlas("Atlas_Meteo");
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.RUNNING, "On awake...");
        }

        /*
		* Called after every (syncrhone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

        /*
		* Called when App is leaving. All coroutine has been stoped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LEAVING, "On quit..."); 
        }
    }
}