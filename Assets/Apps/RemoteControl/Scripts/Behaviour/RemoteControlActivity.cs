using UnityEngine.UI;
using UnityEngine;

using Buddy;

namespace BuddyApp.RemoteControl
{
	/* This class contains useful callback during your app process */
	public class RemoteControlActivity : AAppActivity
	{
		public override void OnLoading(string[] iStrArgs, int[] iIntArgs, float[] iSingleArgs)
		{
			Utils.LogI(LogContext.APP, "On loading...");
			if (iIntArgs.Length > 0)
				if (iIntArgs[0] == 1)
					RemoteControlData.Instance.IsWizardOfOz = true;
				else
					RemoteControlData.Instance.IsWizardOfOz = false;
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