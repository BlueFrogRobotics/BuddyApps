using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.Tutorial
{
    public class TutorialActivity : AAppActivity
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