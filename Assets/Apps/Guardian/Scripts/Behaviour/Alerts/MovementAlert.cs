﻿using BlueQuark;

using UnityEngine;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// Defines a mouvement detection alert
    /// </summary>
	public sealed class MovementAlert : AAlert
	{

		public MovementAlert() : base()
		{
		}

        /// <summary>
        /// Return the text that will be display during a movement detection
        /// </summary>
        /// <returns>the message shown</returns>
		public override string GetDisplayText()
		{
			return Buddy.Resources.GetString("movementalert");
		}

        /// <summary>
        /// Return the text that will be said by buddy when a movement alert occurs
        /// </summary>
        /// <returns>the text said by buddy</returns>
		public override string GetSpeechText()
		{
			return Buddy.Resources.GetRandomString("movementalert");
		}

        /// <summary>
        /// Return the icon that will be shown when a movement alert occurs
        /// </summary>
        /// <returns>the Sprite that will be shown</returns>
		public override Sprite GetIcon()
		{
			return Buddy.Resources.Get<Sprite>("Movement_Alert", Context.APP);
		}

        /// <summary>
        /// Return the log that explain the movement alert
        /// </summary>
        /// <returns>the log</returns>
		public override string GetLog()
		{
			return FormatLog("movementalertmessage");
		}

        /// <summary>
        /// Return the email that will be sent when a movement alert occurs
        /// </summary>
        /// <returns>the email that will be sent</returns>
		public override EMail GetMail()
		{
            EMail lMail = new EMail("Movement alert", FormatMessage("movementalertmessage"));

			return lMail;
		}
	}
}
