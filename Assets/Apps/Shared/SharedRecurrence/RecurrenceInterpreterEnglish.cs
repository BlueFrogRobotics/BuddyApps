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
                                                .Trim(' ').Split(' ');

            switch (iSpeechRule)
            {
                case "every_years":
                    if (null == lStrSplits)
                    {
                        oRepetitionTime = RepetitionTime.ANNUAL;
                        return true;
                    }
                    else
                    {
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_months":
                    if (null == lStrSplits)
                    {
                        oRepetitionTime = RepetitionTime.MONTHLY;
                        return true;
                    }
                    else
                    {
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_weeks":
                    if (null == lStrSplits)
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

                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_days":

                    if (null == lStrSplits)
                    {
                        oRepetitionTime = RepetitionTime.DAYLY;
                        return true;
                    }
                    else
                    {
                        // Not yet lStrSplits[0] == number between 2 reminders.
                        return false;
                    }

                case "every_hours":
                    // Not yet
                    return false;

                case "every_minutes":
                    // Not yet
                    return false;

                case "every_hoursandminutes":
                    // Not yet
                    return false;

                case "every_daysofweeks":
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