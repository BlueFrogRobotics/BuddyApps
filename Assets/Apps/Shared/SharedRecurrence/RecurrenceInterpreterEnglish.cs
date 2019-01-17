using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class RecurrenceInterpreterEnglish : RecurrenceInterpreter
    {
        public override bool ExtractRecurrenceFromSpeech(string iSpeechRule, string iSpeechUtterance, ref RepetitionTime oRepetitionTime, ref List<DayOfWeek> oRepetitionDays)
        {
            string lStrMinutes = iSpeechUtterance;
            string[] lStrSplits = iSpeechUtterance.Replace("every", "")
                                                .Replace(" years", "")
                                                .Replace(" year", "")
                                                .Replace(" months", "")
                                                .Replace(" month", "")
                                                .Replace(" weeks", "")
                                                .Replace(" week", "")
                                                .Replace(" days", "")
                                                .Replace(" day", "")
                                                .Replace(" hours", "")
                                                .Replace(" hour", "")
                                                .Replace(" minutes", "")
                                                .Replace(" minute", "")
                                                .Replace(" and", "")
                                                .Trim(' ').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            switch (iSpeechRule)
            {
                case "every_years":
                    if (lStrSplits.Length == 0)
                    {
                        oRepetitionTime = RepetitionTime.ANNUAL;
                        return true;
                    }
                    else
                    {
                        Debug.Log("<color=red>----" + iSpeechRule + " Whith numbers, Not yet implemented. ----</color>");
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_months":
                    if (lStrSplits.Length == 0)
                    {
                        oRepetitionTime = RepetitionTime.MONTHLY;
                        return true;
                    }
                    else
                    {
                        Debug.Log("<color=red>----" + iSpeechRule + " With numbers, Not yet implemented. ----</color>");
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_weeks":
                    if (lStrSplits.Length == 0)
                    {
                        oRepetitionTime = RepetitionTime.WEEKLY;
                        return true;
                    }
                    else
                    {
                        if (int.Parse(lStrSplits[0]) == 2)
                        {
                            oRepetitionTime = RepetitionTime.EVERY_TWO_WEEKS;
                            return true;
                        }
                        Debug.Log("<color=red>----" + iSpeechRule + " With numbers other than 2, Not yet implemented. ----</color>");
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_days":

                    if (lStrSplits.Length == 0)
                    {
                        oRepetitionTime = RepetitionTime.DAYLY;
                        return true;
                    }
                    else
                    {
                        Debug.Log("<color=red>----" + iSpeechRule + " With numbers, Not yet implemented. ----</color>");
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_hours":
                    // Not yet
                    Debug.Log("<color=red>----" + iSpeechRule + " Not yet implemented. ----</color>");
                    return false;

                case "every_minutes":
                    Debug.Log("<color=red>----" + iSpeechRule + " Not yet implemented. ----</color>");
                    // Not yet
                    return false;

                case "every_hoursandminutes":
                    Debug.Log("<color=red>----" + iSpeechRule + " Not yet implemented. ----</color>");
                    // Not yet
                    return false;

                case "every_daysofweeks":
                    Debug.Log("<color=red>----" + iSpeechRule + " Not yet implemented. ----</color>");
                    return false;

                case "just_once":
                case "no":
                    oRepetitionTime = RepetitionTime.ONCE;
                    return true;

                default:
                    return false;
            }
        }

        public override string RecurrenceToString(RepetitionTime iRepetitionTime, List<DayOfWeek> iRepetitionDays)
        {
            string lStrTmp = iRepetitionTime.ToString();
            for (int i = 0; i < iRepetitionDays.Count; ++i)
            {
                lStrTmp += " " + CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat.GetDayName(iRepetitionDays[i]);
            }

            return lStrTmp;
        }
    }
}