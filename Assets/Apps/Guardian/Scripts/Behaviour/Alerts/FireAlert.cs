using BlueQuark;

using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Define the fire detection alert
    /// </summary>
    public sealed class FireAlert : AAlert
    {

        public FireAlert() : base()
        {
        }

        /// <summary>
        /// Return the text that will be display during a fire detection
        /// </summary>
        /// <returns>the message shown</returns>
        public override string GetDisplayText()
        {
            return Buddy.Resources.GetString("firealert");
        }

        /// <summary>
        /// Return the text that will be said by buddy when a fire alert occurs
        /// </summary>
        /// <returns>the text said by buddy</returns>
        public override string GetSpeechText()
        {
            return Buddy.Resources.GetRandomString("firealert");
        }

        /// <summary>
        /// Return the icon that will be shown when a fire alert occurs
        /// </summary>
        /// <returns>the Sprite that will be shown</returns>
        public override Sprite GetIcon()
        {
            return Buddy.Resources.Get<Sprite>("Fire_Alert", Context.APP);
        }

        /// <summary>
        /// Return the log that explain the fire alert
        /// </summary>
        /// <returns>the log</returns>
        public override string GetLog()
        {
            return FormatLog("firealertmessage");
        }

        /// <summary>
        /// Return the email that will be sent when a fire alert occurs
        /// </summary>
        /// <returns>the email that will be sent</returns>
        public override EMail GetMail()
        {
            EMail lMail = new EMail("Fire alert", FormatMessage("firealertmessage"));

            return lMail;
        }
    }
}
