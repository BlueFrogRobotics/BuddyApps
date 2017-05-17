using Buddy;
using Buddy.Features.Web;
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
            mWebcam = BYOS.Instance.RGBCam;
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
            return BYOS.Instance.ResourceManager.GetSprite("Movement_Alert", "GuardianAtlas");
        }

        public override string GetLog()
        {
            return FormatLog("movementalertmessage");
        }

        public override Mail GetMail()
        {
            if (mWebcam != null && !mWebcam.IsOpen)
                mWebcam.Open(RGBCamResolution.W_176_H_144);

            mSaveVideo.Save("monitoring.avi");

            using (ZipFile zip = new ZipFile()) {
                string filepath = Path.Combine(Application.persistentDataPath, "monitoring.avi");
                zip.AddFile(filepath, "video");
                zip.Save(Path.Combine(Application.persistentDataPath, "video.zip"));
            }

            Mail lMail = new Mail("Movement alert", FormatMessage("movementalertmessage"));
            lMail.AddFile("video.zip");

            return lMail;
        }
    }
}
