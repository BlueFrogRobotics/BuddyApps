using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class RecurrenceInterpreterFrench : RecurrenceInterpreter
    {
        public override bool ExtractRecurrenceFromSpeech(string iSpeechRule, string iSpeechUtterance, ref RepetitionTime oRepetitionTime, ref List<DayOfWeek> oRepetitionDays)
        {
            string lStrMinutes = iSpeechUtterance;
            string[] lStrSplits = iSpeechUtterance.Replace("tous ", "")
                                                .Replace("toutes ", "")
                                                .Replace("les", "")
                                                .Replace(" années", "")
                                                .Replace(" année", "")
                                                .Replace(" ans", "")
                                                .Replace(" an", "")
                                                .Replace(" mois", "")
                                                .Replace(" semaines", "")
                                                .Replace(" semaine", "")
                                                .Replace(" jours", "")
                                                .Replace(" jour", "")
                                                .Replace(" heures", "")
                                                .Replace(" heure", "")
                                                .Replace(" minutes", "")
                                                .Replace(" minute", "")
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
                        if (StringToInt(lStrSplits[0]) == 2)
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
        
        private int StringToInt(string iStringInput)
        {
            switch(iStringInput)
            {
                case "premier":
                case "une":
                    return 1;

                case "vingt-et-une":
                    return 21;

                case "trente-et-une":
                    return 31;

                case "quarante-et-une":
                    return 41;

                case "cinquante-et-une":
                    return 51;

                default:
                    return int.Parse(iStringInput.Trim());
            }
        }

        public override string RecurrenceToString(RepetitionTime iRepetitionTime, List<DayOfWeek> iRepetitionDays)
        {
            string lStrTmp = iRepetitionTime.ToString();
            for (int i = 0; i < iRepetitionDays.Count; ++i)
            {
                lStrTmp += " " + CultureInfo.CreateSpecificCulture("fr-FR").DateTimeFormat.GetDayName(iRepetitionDays[i]);
            }

            return lStrTmp;
        }
    }
}