using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Diagnostic
{
    /* This class contains useful callback during your app process */
    public class DiagnosticActivity : AAppActivity
    {
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
		}

        public override void OnStart(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{
			Utils.LogI(LogContext.APP, "On start...");
		}

        public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");	
		}
    }
}