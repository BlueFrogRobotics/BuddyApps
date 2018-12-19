using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    public class ReminderLanguageFrench : ReminderLanguage
    {
        private enum EDayOfWeek
        {
            lundi = 0,
            mardi = 1,
            mercredi = 2,
            jeudi = 3,
            vendredi = 4,
            samedi = 5,
            dimanche = 6
        }

        private enum EMonths
        {
            janvier = 1,
            février = 2,
            mars = 3,
            avril = 4,
            mai = 5,
            juin = 6,
            juillet = 7,
            août = 8,
            septembre = 9,
            octobre = 10,
            novembre = 11,
            décembre = 12
        }

        public override bool ExtractDateFromSpeech(string iSpeechRule, string iSpeechUtterance)
        {
            switch (iSpeechRule)
            {
                case "today":
                    SetDate(DateTime.Today.Date);
                    return true;

                case "tomorrow":
                    SetDate(DateTime.Today.AddDays(1D).Date);
                    return true;

                case "dayaftertomorrow":
                    SetDate(DateTime.Today.AddDays(2D).Date);
                    return true;

                case "nextdayofweek":
                    SetNextDayOfWeek(iSpeechUtterance);
                    return true;

                case "nextweek":
                    SetDate(DateTime.Today.AddDays(7D).Date);
                    return true;

                case "nextmonth":
                    SetDate(DateTime.Today.AddMonths(1).Date);
                    return true;

                case "nextyear":
                    SetDate(DateTime.Today.AddYears(1).Date);
                    return true;

                case "nextday":
                    SetNextDay(iSpeechUtterance);
                    return true;

                case "nextdate":
                    return SetNextDate(iSpeechUtterance);

                case "fulldate":
                    return SetFullDate(iSpeechUtterance);

                case "indays":
                    SetInDays(iSpeechUtterance);
                    return true;

                case "inweeks":
                    SetInWeeks(iSpeechUtterance);
                    return true;

                case "inmonths":
                    SetInMonths(iSpeechUtterance);
                    return true;

                case "inyears":
                    SetInYears(iSpeechUtterance);
                    return true;

                default:
                    return false;
            }
        }

        public override bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance)
        {
            string lStrMinutes = iSpeechUtterance;
            string[] lStrSplits = iSpeechUtterance.Replace("à ", "")
                                                .Replace("midi ", "")
                                                .Replace("minuit ", "")
                                                .Replace("dans ", "")
                                                .Replace(" heures", "")
                                                .Replace(" heure", "")
                                                .Replace("minutes", "")
                                                .Replace("minute", "")
                                                .Trim(' ').Split(' ');
            
            switch (iSpeechRule)
            {
                case "atnoon":
                    SetHour(12, 0);
                    return true;

                case "atnoonandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("midi", "");

                    SetHour(12, MinutesToInt(lStrMinutes.Substring(lStrMinutes.IndexOf(lStrSplits[0]))));
                    return true;

                case "atmidnight":
                    SetHour(0, 0);
                    return true;

                case "atmidnightandminutes":
                    SetHour(0, MinutesToInt(lStrSplits[lStrSplits.Length - 1]));

                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("minuit", "");

                    SetHour(0, MinutesToInt(lStrMinutes.Substring(lStrMinutes.IndexOf(lStrSplits[0]))));
                    return true;

                case "athours":
                    SetHour(StringToInt(lStrSplits[0]), 0);
                    return true;
                    
                case "athoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("heures", "")
                                                    .Replace("heure", "");

                    SetHour(StringToInt(lStrSplits[0]), MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                case "atamhours":
                    SetHour(StringToInt(lStrSplits[0]), 0);
                    return true;

                case "atamhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("heures", "")
                                                    .Replace("heure", "");

                    SetHour(StringToInt(lStrSplits[0]), MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;
                    
                case "atpmhours":
                    lStrMinutes = iSpeechUtterance.Replace("à ", "")
                                                    .Replace("heures ", "")
                                                    .Replace("heure ", "")
                                                    .Replace("de l'après-midi", "")
                                                    .Replace("du soir", "");
                    
                    return true;

                case "atpmhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à ", "")
                                                    .Replace(" heures", "")
                                                    .Replace(" heure", "")
                                                    .Replace(" de l'après-midi", "")
                                                    .Replace(" du soir", "");
                    
                    SetHour(StringToInt(lStrSplits[0]) + 12, MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                case "inhours":
                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddHours(StringToInt(lStrSplits[0]));
                    return true;

                case "inminutes":
                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddMinutes(StringToInt(lStrSplits[0]));
                    return true;

                case "inhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("dans ", "")
                                                    .Replace(" heures", "")
                                                    .Replace(" heure", "");

                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);

                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddHours(StringToInt(lStrSplits[0]))
                                                                                .AddMinutes(MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                default:
                    return false;
            }
        }
        
        private void SetNextDayOfWeek(string iSpeechUtterance)
        {
            // Extract day of week
            string lStrDayOfWeek = iSpeechUtterance.Trim(' ').Split(' ')[0];

            int lIDaysToAdd = (7 + (int)Enum.Parse(typeof(EDayOfWeek), lStrDayOfWeek) - (int)DateTime.Today.DayOfWeek) % 7 + 1;

            if (0 == lIDaysToAdd) // Same day of the week
            {
                lIDaysToAdd += 7;
            }

            SetDate(DateTime.Today.AddDays(lIDaysToAdd));
        }

        private void SetNextDay(string iSpeechUtterance)
        {
            // Get the word for the targeted day and remove useless characters.
            string lStrDay = iSpeechUtterance.Trim(' ').Split(' ')[iSpeechUtterance.Trim(' ').Split(' ').Length - 1].Replace("-", "");
            int lDayNumber = StringToInt(lStrDay);

            // Initialize date to today
            DateTime lDate = DateTime.Today;

            // Skip current month if targeted day in past
            if (lDayNumber <= lDate.Day)
            {
                lDate = lDate.AddMonths(1);
            }

            // Find a month with enough days
            while (DateTime.DaysInMonth(lDate.Year, lDate.Month) < lDayNumber)
            {
                lDate = lDate.AddMonths(1);
            }

            // Set the date to the targeted day
            lDate = lDate.AddDays(lDayNumber - lDate.Day);
            SetDate(lDate);
        }

        private bool SetNextDate(string iSpeechUtterance)
        {
            // Get the word for the targeted day and remove useless characters.
            string[] lNextDate = iSpeechUtterance.Trim(' ').Split(' ');

            string lStrMonth = lNextDate[lNextDate.Length - 1];
            string lStrDay = lNextDate[lNextDate.Length - 2];

            // Store day to control existence before changing
            int lIDay = StringToInt(lStrDay);
            int lIMonth = 0;
            
            // Set month
            EMonths lMonthResult;
            if (Enum.TryParse<EMonths>(lStrMonth, true, out lMonthResult))
            {
                lIMonth = (int)lMonthResult;
            }
            
            DateTime lTmpDate = DateTime.Today;
            lTmpDate = lTmpDate.AddMonths(lIMonth - lTmpDate.Month).AddDays(lIDay - lTmpDate.Day);

            if (lTmpDate.CompareTo(DateTime.Today) <= 0)
            {
                lTmpDate = lTmpDate.AddYears(1);
            }

            if (lTmpDate.Day == lIDay
                && lTmpDate.Month == lIMonth)
            {
                SetDate(lTmpDate);
                return true;
            }

            Debug.LogError("Incorrect date!!!");
            return false;
        }

        private bool SetFullDate(string iSpeechUtterance)
        {
            DateTime lTmpDate;
            string lStrYear = iSpeechUtterance.Trim(' ').Substring(iSpeechUtterance.LastIndexOf(" "));
            string[] lNextDate = iSpeechUtterance.Trim(' ').Split(' ');

            // Store day to control existence before changing
            Debug.LogError("test1:" + lStrYear);

            int lIDay = StringToInt(lNextDate[lNextDate.Length - 3]);
            int lIMonth = (int)Enum.Parse(typeof(EMonths), lNextDate[lNextDate.Length - 2]);
            int lIYear = int.Parse(lStrYear);

            try
            {
                lTmpDate = new DateTime(lIYear, lIMonth, lIDay);
            }
            catch
            {
                return false;
            }

            if (lTmpDate.Year != lIYear || lTmpDate.Month != lIMonth || lTmpDate.Day != lIDay)
            {
                Debug.LogError("Date does not exist!!!");
                return false;
            }

            SetDate(lTmpDate.Year, lTmpDate.Month, lTmpDate.Day);

            return true;
        }

        private void SetInDays(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIDay = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(lIDay));
        }

        private void SetInWeeks(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIWeeks = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(7 * lIWeeks));
        }

        private void SetInMonths(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIMonths = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddMonths(lIMonths));
        }

        private void SetInYears(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIYears = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddYears(lIYears));
        }

        private int MinutesToInt(string iStrMinutes)
        {
            string[] lStrMinutes = iStrMinutes.Trim(' ').Split(' ');

            // Quarter
            if (lStrMinutes[lStrMinutes.Length - 1].Equals("quart"))
            {
                if (lStrMinutes[0].Equals("moins"))
                {
                    return -15;
                }
                else
                {
                    return 15;
                }
            }

            // Half
            if (lStrMinutes[lStrMinutes.Length - 1].Equals("demi"))
            {
                return 30;
            }

            // Minus minutes
            if (lStrMinutes[0].Equals("moins"))
            {
                return -StringToInt(lStrMinutes[1]);
            }

            // And minutes
            if (lStrMinutes[0].Equals("et"))
            {
                return StringToInt(lStrMinutes[1]);
            }

            return StringToInt(lStrMinutes[0]);
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






        public override bool ExtractHourFromSpeech(string iSpeech)
        {
            DayMoment lDayMoment = DayMoment.UNKNOW;
            List<int> lNumbers;

            // Check if the user asked AM or PM
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Try to get two numbers in speech
            lNumbers = GetNumbersInString(iSpeech, 2);

            // No numbers in the speech
            if (lNumbers == null)
            {
                if (ContainsOneOf(iSpeech, "midnight"))
                    return HourSet(0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "noon"))
                    return HourSet(12, 0, lDayMoment);
            }
            // One numbers in the speech
            else if (lNumbers.Count == 1 && lNumbers[0] >= 0)
            {
                if (ContainsOneOf(iSpeech, "intime"))
                {
                    if (ContainsOneOf(iSpeech, "hour"))
                    {
                        ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Date + DateTime.Now.AddHours(lNumbers[0]).TimeOfDay;
                        return true;
                    }
                    else if (ContainsOneOf(iSpeech, "minute"))
                    {
                        ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Date + DateTime.Now.AddMinutes(lNumbers[0]).TimeOfDay;
                        return true;
                    }
                    return false;
                }
                else if (ContainsOneOf(iSpeech, "hour"))
                {
                    if (ContainsOneOf(iSpeech, "half"))
                        return HourSet(lNumbers[0], 30, lDayMoment);
                    else if (ContainsOneOf(iSpeech, "quarterto"))
                        return HourSet((lNumbers[0] - 1), 45, lDayMoment);
                    else if (ContainsOneOf(iSpeech, "quarter"))
                        return HourSet(lNumbers[0], 15, lDayMoment);
                    else
                        return HourSet(lNumbers[0], 0, lDayMoment);
                }
            }
            // Two numbers in the speech
            else if (lNumbers.Count >= 2 && lNumbers[0] >= 0 && lNumbers[1] >= 0)
            {
                // Ex: in 2 hours and 10 minutes
                if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour"))
                {
                    int lMinutesToAdd = (lNumbers[0] * 60) + lNumbers[1];
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Date + DateTime.Now.AddMinutes(lMinutesToAdd).TimeOfDay;
                    return true;
                }
                else if (ContainsOneOf(iSpeech, "hour"))
                {
                    if (ContainsOneOf(iSpeech, "to"))
                        return HourSet(lNumbers[0] - 1, 60 - lNumbers[1], lDayMoment);
                    else
                        return HourSet(lNumbers[0], lNumbers[1], lDayMoment);
                }
            }
            return false;
        }

        public override string DateToString()
        {
            return ReminderDateManager.GetInstance().ReminderDate.ToString("dd/MM/yyyy");
        }
    }
}