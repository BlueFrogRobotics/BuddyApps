using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Jukebox
{
    /* This class contains useful callback during your app process */
    public class JukeboxActivity : AAppActivity
    {
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
		}

        public override void OnStart()
		{
			Utils.LogI(LogContext.APP, "On start...");
		}

        public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");	
		}
    }
}