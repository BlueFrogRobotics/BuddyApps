using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Companion
{
    /* This class contains useful callback during your app process */
    public class CompanionActivity : AAppActivity
    {
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
			BYOS.Instance.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
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