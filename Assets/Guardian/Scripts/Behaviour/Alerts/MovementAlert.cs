using Buddy;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
	public class MovementAlert : AAlert
	{
		private SaveVideo mSaveVideo;
		private RGBCam mWebcam;

		public MovementAlert(SaveVideo iSaveVideo) : base()
		{
			mSaveVideo = iSaveVideo;
			mWebcam = BYOS.Instance.Primitive.RGBCam;
		}

		public override string GetDisplayText()
		{
			return BYOS.Instance.Dictionary.GetString("movementalert");
		}

		public override string GetSpeechText()
		{
			return BYOS.Instance.Dictionary.GetRandomString("movementalert");
		}

		public override Sprite GetIcon()
		{
			return BYOS.Instance.Resources.GetSprite("Movement_Alert", "GuardianAtlas");
		}

		public override string GetLog()
		{
			return FormatLog("movementalertmessage");
		}

		public override EMail GetMail()
		{
			if (mWebcam != null && !mWebcam.IsOpen)
				mWebcam.Open(RGBCamResolution.W_176_H_144);

			//TODO solve Writing rights issues with PathToRaw?
			//mSaveVideo.Save("monitoring.avi");


			//using (ZipFile zip = new ZipFile()) {
			//string filepath = BYOS.Instance.Resources.PathToRaw("monitoring.avi");
			//zip.AddFile(filepath, "video");
			//zip.Save(BYOS.Instance.Resources.PathToRaw("video.zip"));
			//}

			EMail lMail = new EMail("Movement alert", FormatMessage("movementalertmessage"));
			//lMail.AddFile(BYOS.Instance.Resources.PathToRaw("video.zip"));

			return lMail;
		}
	}
}
