using BlueQuark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
	public sealed class KidnappingAlert : AAlert
	{
        /// <summary>
        /// Return the text that will be display during a kidnapping detection
        /// </summary>
        /// <returns>the message shown</returns>
		public override string GetDisplayText()
		{
			return Buddy.Resources.GetString("kidnappingalert");
		}

        /// <summary>
        /// Return the text that will be said by buddy when a kidnapping alert occurs
        /// </summary>
        /// <returns>the text said by buddy</returns>
		public override string GetSpeechText()
		{
			return Buddy.Resources.GetRandomString("kidnappingalert");
		}

        /// <summary>
        /// Return the icon that will be shown when a kidnapping alert occurs
        /// </summary>
        /// <returns>the Sprite that will be shown</returns>
		public override Sprite GetIcon()
		{
			return Buddy.Resources.Get<Sprite>("Kidnapping_Alert", Context.APP);
		}

        /// <summary>
        /// Return the log that explain the kidnapping alert
        /// </summary>
        /// <returns>the log</returns>
		public override string GetLog()
		{
			return FormatLog("kidnappingalert");
		}

        /// <summary>
        /// Return the email that will be sent when a kidnapping alert occurs
        /// </summary>
        /// <returns>the email that will be sent</returns>
        public override EMail GetMail()
        {
            EMail lMail = new EMail("Kidnapping alert", FormatMessage("kidnappingalert"));

            return lMail;
        }

    }
}
