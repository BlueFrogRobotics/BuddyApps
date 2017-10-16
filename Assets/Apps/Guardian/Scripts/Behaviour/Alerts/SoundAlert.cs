using Buddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Defines a sound detection alert
    /// </summary>
    public class SoundAlert : AAlert
    {

        public SoundAlert() : base()
        {
        }

        /// <summary>
        /// Return the text that will be display during a sound detection
        /// </summary>
        /// <returns>the message shown</returns>
        public override string GetDisplayText()
        {
            return BYOS.Instance.Dictionary.GetString("soundalert");
        }

        /// <summary>
        /// Return the text that will be said by buddy when a sound alert occurs
        /// </summary>
        /// <returns>the text said by buddy</returns>
        public override string GetSpeechText()
        {
            return BYOS.Instance.Dictionary.GetRandomString("soundalert");
        }

        /// <summary>
        /// Return the icon that will be shown when a sound alert occurs
        /// </summary>
        /// <returns>the Sprite that will be shown</returns>
        public override Sprite GetIcon()
        {
            return BYOS.Instance.Resources.GetSprite("Sound_Alert", "GuardianAtlas");
        }

        /// <summary>
        /// Return the log that explain the sound alert
        /// </summary>
        /// <returns>the log</returns>
        public override string GetLog()
        {
            return FormatLog("soundalertmessage");
        }

        /// <summary>
        /// Return the email that will be sent when a sound alert occurs
        /// </summary>
        /// <returns>the email that will be sent</returns>
        public override EMail GetMail()
        {
            EMail lMail = new EMail("Noise alert", FormatMessage("soundalertmessage"));

            return lMail;
        }
    }
}
