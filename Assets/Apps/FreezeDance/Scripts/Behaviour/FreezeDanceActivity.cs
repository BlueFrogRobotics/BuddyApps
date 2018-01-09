using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.FreezeDance
{
    /* This class contains useful callback during your app process */
    public class FreezeDanceActivity : AAppActivity
    {
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{
            BYOS.Instance.Resources.LoadAtlas("os_AtlasUI");
			Utils.LogI(LogContext.APP, "On loading...");
		}

        public override void OnStart()
		{
			Utils.LogI(LogContext.APP, "On start...");
			BYOS.Instance.Header.DisplayParametersButton = false;
		}

        public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");	
		}
    }
}