using Buddy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Abstract class that define the specificities of an alert
    /// </summary>
    public abstract class AAlert
    {
        /// <summary>
        /// The date at which the alert occurs
        /// </summary>
        public string AlertDate { get; private set; }
        
        public AAlert()
        {
            CultureInfo lCurrentCulture = new CultureInfo(BYOS.Instance.Language.CurrentFormat);
            AlertDate = DateTime.Now.ToString(lCurrentCulture);
        }

        /// <summary>
        /// Return the text that will be display during a detection
        /// </summary>
        /// <returns>the message shown</returns>
        public abstract string GetDisplayText();

        /// <summary>
        /// Return the text that will be said by buddy when the alert occurs
        /// </summary>
        /// <returns>the text said by buddy</returns>
        public abstract string GetSpeechText();

        /// <summary>
        /// Return the icon that will be shown when the alert occurs
        /// </summary>
        /// <returns>the Sprite that will be shown</returns>
        public abstract Sprite GetIcon();

        /// <summary>
        /// Return the log that explain the alert
        /// </summary>
        /// <returns>the log</returns>
        public abstract string GetLog();


        /// <summary>
        /// Return the email that will be sent when the alert occurs
        /// </summary>
        /// <returns>the email that will be sent</returns>
        public virtual EMail GetMail()
        {
            return null;
        }

        /// <summary>
        /// Format an alert to add the date at which the alert occurs in it
        /// </summary>
        /// <param name="iAlertKey">the dictionnary key of the message</param>
        /// <returns>the message formatted</returns>
        protected string FormatMessage(string iAlertKey)
        {
            return BYOS.Instance.Dictionary.GetString("alertdate").Replace("XXX", AlertDate) +
                    " " + BYOS.Instance.Dictionary.GetString(iAlertKey);
        }

        /// <summary>
        /// Add the date without message to a log
        /// </summary>
        /// <param name="iAlertKey">the dictionnary key of the message</param>
        /// <returns>the log formatted</returns>
        protected string FormatLog(string iAlertKey)
        {
            return AlertDate + " : " + BYOS.Instance.Dictionary.GetString(iAlertKey);
        }
    }
}
