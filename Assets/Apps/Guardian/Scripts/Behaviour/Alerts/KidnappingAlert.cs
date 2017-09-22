using Buddy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
			return BYOS.Instance.Resources.GetSprite("Kidnapping_Alert", "GuardianAtlas");
		}

		public override string GetLog()
		{
			return FormatLog("kidnappingalert");
		}

	}
}
