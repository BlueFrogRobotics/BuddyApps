using Buddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Buddy.Features.Web;

namespace BuddyApp.Guardian
{
    public class KidnappingAlert : AAlert
    {
        public override string GetDisplayText()
        {
            return BYOS.Instance.Dictionary.GetString("kidnappingalert");
        }

        public override string GetSpeechText()
        {
            return BYOS.Instance.Dictionary.GetRandomString("kidnappingalert");
        }

        public override Sprite GetIcon()
        {
            return BYOS.Instance.ResourceManager.GetSprite("Kidnapping_Alert", "GuardianAtlas");
        }

        public override string GetLog()
        {
            return FormatLog("kidnappingalertmessage");
        }

    }
}
