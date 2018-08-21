using UnityEngine.UI;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.Diagnostic
{
    /* This class contains useful callback during your app process */
    public class DiagnosticActivity : AAppActivity
    {
		public override void OnLoading(object[] iInputArgs)
		{
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On loading...");
        }

        public override void OnStart()
		{
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");
            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        public override void OnQuit()
		{
            //ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LEAVING, "On quit...");
        }
    }
}