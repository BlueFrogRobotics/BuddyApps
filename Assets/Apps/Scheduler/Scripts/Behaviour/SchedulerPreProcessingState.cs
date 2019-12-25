using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Scheduler
{
    public sealed class SchedulerPreProcessingState : AStateMachineBehaviour
    {
        // Tags for date rules. (EN FR)
        private readonly string[] DATE_TAGS =
        {
            "TODAY",
            "TOMORROW",
            "DAYAFTERTOMORROW",
            "NEXTDAYOFWEEK",
            "NEXTWEEK",
            "NEXTMONTH",
            "NEXTYEAR",
            "NEXTDAY",
            "NEXTDATE",
            "FULLDATE",
            "IN_YEARS",
            "IN_MONTHS",
            "IN_WEEKS",
            "IN_DAYS"
        };

        // Tags for hour rules. (EN FR)
        private readonly string[] HOUR_TAGS =
        {
            "ATNOON",
			"ATNOONANDMINUTES",
            "ATPRENOON",
            "ATPOSTNOON",
            "ATMIDNIGHT",
			"ATMIDNIGHTANDMINUTES",
            "ATPREMIDNIGHT",
            "ATPOSTMIDNIGHT",
            "ATHOURS",
			"ATHOURSANDMINUTES",
            "ATPREHOURS",
            "ATPOSTHOURS",
            "ATAMHOURS",
			"ATAMHOURSANDMINUTES",
            "ATAMMINUTESHOURS",
            "ATAMHALFHOURS",
            "ATPREAMHOURS",
            "ATPOSTAMHOURS",
            "ATPMHOURS",
			"ATPMHOURSANDMINUTES",
            "ATPMMINUTESHOURS",
            "ATPMHALFHOURS",
            "ATPREPMHOURS",
            "ATPOSTPMHOURS",
            "IN_HOURS",
			"IN_MINUTES",
			"IN_HOURSANDMINUTES",
        };

        private int mSchedulerInfo;
        
        // TMP
        private Dictionary<string, string> mCompanionInput;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			mSchedulerInfo = 0;

            /*
            ** TMP Code to test - Waiting for the tags to be added to the sdk
            ** Replace mCompanionInput by 'ReminderDateManager.GetInstance().CompanionInput.tag' when available.
            ** Simulate here a Companion Input, where Key = tag, value = Utterance
            */
            //mCompanionInput = new Dictionary<string, string>
            //{
            //    {"FULLDATE", "Dinsdag 8 augustus 2019" },
            //    {"ATAMMINUTESHOURS", "3 over half 5" }
            //};
            /*
            **  END TMP Code
            */

            string lKey = null;
            lKey = FindAKeyInDico(HOUR_TAGS, mCompanionInput);
            if (!string.IsNullOrEmpty(lKey))
            {
                // Extract the hour from speech
                DateTime lDate = SchedulerDateManager.GetInstance().SchedulerDate;
				SchedulerLanguageManager.GetInstance().GetHourLanguage().ExtractHourFromSpeech(
                                lKey.ToLower().Trim(' '),
                                mCompanionInput[lKey].Trim(' '),
                                ref lDate);
				SchedulerDateManager.GetInstance().SchedulerDate = lDate;
				mSchedulerInfo += 1;
            }
            lKey = FindAKeyInDico(DATE_TAGS, mCompanionInput);
            if (!string.IsNullOrEmpty(lKey))
            {
                // Extract the date from speech
                DateTime lDate = SchedulerDateManager.GetInstance().SchedulerDate;
				SchedulerLanguageManager.GetInstance().GetDateLanguage().ExtractDateFromSpeech(
                                lKey.ToLower().Trim(' '),
                                mCompanionInput[lKey].Trim(' '),
                                ref lDate);
				SchedulerDateManager.GetInstance().SchedulerDate = lDate;
				mSchedulerInfo += 2;
            }

            switch (mSchedulerInfo)
            {
                case 0: // No information was found.
                    Trigger("DateChoiceState");
                    break;
                case 1: // Just an hour was found, the date used is today by default.
                    Trigger("GetMessageState");
                    break;
                case 2: // Just a date was found, go to HourChoiceState.
                    Trigger("HourChoiceState");
                    break;
                case 3: // An hour and a date was found, go to GetMessageState.
                    Trigger("GetMessageState");
                    break;
            }
        }

        private string FindAKeyInDico(string[] iKeys, Dictionary<string, string> iDico)
        {
            foreach (string lKey in iKeys)
            {
                if (mCompanionInput.ContainsKey(lKey))
                    return lKey;
            }
            return null;
        }
    }
}
