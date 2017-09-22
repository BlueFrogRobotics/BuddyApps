using Buddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public class SoundAlert : AAlert
    {
        private SaveAudio mSaveAudio;

        public SoundAlert(SaveAudio iSaveAudio) : base()
        {
            mSaveAudio = iSaveAudio;
        }

        public override string GetDisplayText()
        {
            return BYOS.Instance.Dictionary.GetString("soundalert");
        }

        public override string GetSpeechText()
        {
            return BYOS.Instance.Dictionary.GetRandomString("soundalert");
        }

        public override Sprite GetIcon()
        {
            return BYOS.Instance.Resources.GetSprite("Sound_Alert", "GuardianAtlas");
        }

        public override string GetLog()
        {
            return FormatLog("soundalertmessage");
        }

        public override EMail GetMail()
        {
            mSaveAudio.Save();

            EMail lMail = new EMail("Noise alert", FormatMessage("soundalertmessage"));
			// TODO find the path to write
            //lMail.AddFile(BYOS.Instance.Resources.PathToRaw("noise.wav"));

            return lMail;
        }
    }
}
