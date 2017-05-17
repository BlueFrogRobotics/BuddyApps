using Buddy;
using Buddy.Features.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public class FireAlert : AAlert
    {
        private RGBCam mWebcam;

        public FireAlert() : base()
        {
            mWebcam = BYOS.Instance.RGBCam;
        }

        public override string GetDisplayText()
        {
            return BYOS.Instance.Dictionary.GetString("firealert");
        }

        public override string GetSpeechText()
        {
            return BYOS.Instance.Dictionary.GetRandomString("firealert");
        }

        public override Sprite GetIcon()
        {
            return BYOS.Instance.ResourceManager.GetSprite("Fire_Alert", "GuardianAtlas");
        }

        public override string GetLog()
        {
            return FormatLog("firealertmessage");
        }

        public override Mail GetMail()
        {
            if (mWebcam != null && !mWebcam.IsOpen)
                mWebcam.Open(RGBCamResolution.W_176_H_144);

            if (mWebcam == null || !mWebcam.IsOpen || mWebcam.FrameTexture2D == null)
                return null;

            Mail lMail = new Mail("Fire alert", FormatMessage("firealertmessage"));
            lMail.AddTexture2D(mWebcam.FrameTexture2D, "photocam.png");

            return lMail;
        }
    }
}
