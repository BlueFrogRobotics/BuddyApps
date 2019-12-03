using UnityEngine.UI;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.MemoryGame
{
    /* This class contains useful callback during your app process */
    public class MemoryGameActivity : AAppActivity
    {
        /*
		* Called before the App scene loading.
		*/
        public override void OnLoading(object[] iArgs)
        {
            ExtLog.I(ExtLogModule.APP, typeof(MemoryGameActivity), LogStatus.START, LogInfo.LOADING, "On loading...");
        }

        /*
		* Called after every Awake() in your scene
		*/
        public override void OnAwake()
        {
            ExtLog.I(ExtLogModule.APP, typeof(MemoryGameActivity), LogStatus.START, LogInfo.RUNNING, "On awake...");
            Buddy.GUI.Header.OnClickParameters.Add(OnClickParameters);
        }

        private void OnClickParameters()
        {
            Animator.SetTrigger("Parameters");
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        /*
		* Called after every (synchrone) Start() in your scene
		*/
        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, typeof(MemoryGameActivity), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
        }

        /*
		* Called when App is leaving. All coroutines have been stopped and data been saved. The scene will be unloaded just after OnQuit()
		*/
        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, typeof(MemoryGameActivity), LogStatus.START, LogInfo.STOPPING, "On quit...");
        }
    }
}