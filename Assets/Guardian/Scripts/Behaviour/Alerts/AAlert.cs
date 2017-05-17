using Buddy;
using Buddy.Features.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BuddyApp.Guardian
{
    public abstract class AAlert
    {
        public string AlertDate { get; private set; }
        
        public AAlert()
        {
            CultureInfo lCurrentCulture = new CultureInfo(BYOS.Instance.LanguageManager.CurrentFormat);
            AlertDate = DateTime.Now.ToString(lCurrentCulture);
        }

        public abstract string GetDisplayText();
        public abstract string GetSpeechText();
        public abstract Sprite GetIcon();
        public abstract string GetLog();

        public virtual Mail GetMail()
        {
            return null;
        }

        protected string FormatMessage(string iAlertKey)
        {
            return BYOS.Instance.Dictionary.GetString("alertdate").Replace("XXX", AlertDate) +
                    " " + BYOS.Instance.Dictionary.GetString(iAlertKey);
        }

        protected string FormatLog(string iAlertKey)
        {
            return AlertDate + " : " + BYOS.Instance.Dictionary.GetString(iAlertKey);
        }
    }
}
