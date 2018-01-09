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
			//Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
			//CompanionData.Instance.LastApp = "";
			//BYOS.Instance.Interaction.TextToSpeech.SetPitch(1.1F);
			//BYOS.Instance.Interaction.TextToSpeech.SetSpeechRate(0.7F);
		}

		public override void OnStart()
		{
			Utils.LogI(LogContext.APP, "On  start...");
			CompanionLayout lLayout = (CompanionLayout)Layout;
			lLayout.mState = (Text)Objects[0];
		}

		public override void OnQuit()
		{
			Utils.LogI(LogContext.APP, "On quit...");
		}
	}
}