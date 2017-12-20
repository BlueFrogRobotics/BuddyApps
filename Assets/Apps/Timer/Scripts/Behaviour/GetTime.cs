using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Buddy;
using Buddy.UI;

namespace BuddyApp.Timer
{
	public class GetTime : AStateMachineBehaviour
	{
		private int mFinalsec;
		private int mSec;
		private int mMin;
		private int mHour;

		private string mVoice;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mFinalsec = -1;
			if (!string.IsNullOrEmpty(TimerData.Instance.VocalRequest)) {
				mVoice = TimerData.Instance.VocalRequest.ToLower();
				ParseTime(mVoice);
				if (mFinalsec == -1) {
					Interaction.TextToSpeech.SayKey("whattime");
					Trigger("TimeNeeded");
				}
			} else {
				Interaction.TextToSpeech.SayKey("whattime");
				Trigger("TimeNeeded");
			}
		}

		private void ParseTime(string iVoice)
		{
			double num;
			string lCnumber;
			string[] lWord = iVoice.Split(' ');
			lCnumber = null;

			for (int i = 0; i <= lWord.Length; i++) {
				if (double.TryParse(lWord[i], out num)) {
					lCnumber = lWord[i];
					i++;
				}


				//if ((lWord[i].Equals(Dictionary.GetPhoneticStrings("sec")) || lWord[i].Equals(Dictionary.GetPhoneticStrings("secs"))))
				if (lWord[i].Contains("second") && lCnumber != null) {
					mFinalsec += Int32.Parse(lCnumber);
					lCnumber = null;
					i++;
				} else if (lWord[i].Contains("minute") && lCnumber != null) {
					mFinalsec += Int32.Parse(lCnumber) * 60;
					lCnumber = null;
					i++;
				} else if (lWord[i].Contains(Dictionary.GetString("hour")) && lCnumber != null) {
					mFinalsec += Int32.Parse(lCnumber) * 3600;
					lCnumber = null;
					i++;
				}
				//else if (lWord[i].Equals(Dictionary.GetPhoneticStrings("hour")) || lWord[i].Equals(Dictionary.GetPhoneticStrings("hours")))
			}

			CommonIntegers["finalcountdown"] = mFinalsec;

			Trigger("Countdown");

		}

	}
}
