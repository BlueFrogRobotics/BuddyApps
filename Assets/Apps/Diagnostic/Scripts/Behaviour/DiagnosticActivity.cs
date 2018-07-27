using UnityEngine.UI;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.Diagnostic
{
    /* This class contains useful callback during your app process */
    public class DiagnosticActivity : AAppActivity
    {
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
		}

        public override void OnStart()
		{
			Utils.LogI(LogContext.APP, "On start...");
            Buddy.GUI.Header.DisplayParametersButton(false);
		}

        public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");	
		}
    }
}