using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.Reminder
{
    /* This class contains useful callback during your app process */
    public class ReminderActivity : AAppActivity
    {

        private ReminderManager mReminderManager;

		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{ 
			Utils.LogI(LogContext.APP, "On loading...");
		}

        public override void OnStart(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{
			Utils.LogI(LogContext.APP, "On start...");
            mReminderManager = (ReminderManager)Objects[0];
		}

        public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");
            mReminderManager.SerializeList();
		}
    }
}