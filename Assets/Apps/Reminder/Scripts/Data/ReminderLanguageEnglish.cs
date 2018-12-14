using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    public class ReminderLanguageEnglish : ReminderLanguage
    {
        private enum EDayNumber
        {
            first = 1,
            second = 2,
            third = 3,
            fourth = 4,
            fifth = 5,
            sixth = 6,
            seventh = 7,
            eighth = 8,
            ninth = 9,
            tenth = 10,
            eleventh = 11,
            twelfth = 12,
            thirteenth = 13,
            fourteenth = 14,
            fifteenth = 15,
            sixteenth = 16,
            seventeenth = 17,
            eighteenth = 18,
            nineteenth = 19,
            twentieth = 20,
            twentyfirst = 21,
            twentysecond = 22,
            twentythird = 23,
            twentyfourth = 24,
            twentyfifth = 25,
            twentysixth = 26,
            twentyseventh = 27,
            twentyeighth = 28,
            twentyninth = 29,
            thirty = 30,
            thirtyfirst = 31
        }

        private enum EDayOfWeek
        {
            monday = 0,
            tuesday = 1,
            wednesday = 2,
            thursday = 3,
            friday = 4,
            saturday = 5,
            sunday = 6
        }

        private enum EMonths
        {
            january = 1,
            february = 2,
            march = 3,
            april = 4,
            may = 5,
            june = 6,
            july = 7,
            august = 8,
            september = 9,
            october = 10,
            november = 11,
            december = 12
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
            string[] lStrSplits = iSpeechUtterance.Replace("at ", "")
                                                .Replace("in ", "")
                                                .Replace(" a ", " ")
                                                .Replace(" pm", "")
                                                .Replace(" am", "")
                                                .Replace(" and", "")
                                                .Replace(" minutes", "")
                                                .Replace(" minute", "")
                                                .Replace(" hours", "")
                                                .Replace(" hour", "")
                                                .Replace(" o'clock", "")
                                                .Trim(' ').Split(' ');

            switch (iSpeechRule)
            {
                case "atnoon":
                    SetHour(12, 0);
                    return true;

                case "atprenoon":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" noon", "");

                    SetHour(12, GetMinutesFromPreString(lStrMinutes));
                    return true;

                case "atpostnoon":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace("noon ", "");

                    SetHour(12, GetMinutesFromPostString(lStrMinutes));
                    return true;

                case "atmidnight":
                    SetHour(0, 0);
                    return true;

                case "atpremidnight":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" midnight", "");

                    SetHour(0, GetMinutesFromPreString(lStrMinutes));
                    return true;

                case "atpostmidnight":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace("midnight ", "");

                    SetHour(0, GetMinutesFromPostString(lStrMinutes));
                    return true;

                case "athours":
                    SetHour(int.Parse(lStrSplits[0]), 0);
                    return true;

                case "atprehours":
                    lStrMinutes = iSpeechUtterance.Replace("at", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length - 1]), GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))));
                    return true;

                case "atposthours":
                    lStrMinutes = iSpeechUtterance.Replace("at", "");

                    SetHour(int.Parse(lStrSplits[0]), GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                case "atamhours":
                    SetHour(int.Parse(lStrSplits[0]), 0);
                    return true;
            
                case "atpreamhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" am", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length - 1]), GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))));
                    return true;

                case "atpostamhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" am", "");

                    SetHour(int.Parse(lStrSplits[0]), GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                case "atpmhours":
                    SetHour(int.Parse(lStrSplits[0]) + 12, 0);
                    return true;

                case "atprepmhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" pm", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length-1]) + 12, GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))));
                    return true;

                case "atpostpmhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" pm", "");
                    
                    SetHour(int.Parse(lStrSplits[0]) + 12, GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;
                    
                case "inhours":
                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddHours(int.Parse(lStrSplits[0]));
                    return true;

                case "inminutes":
                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddMinutes(int.Parse(lStrSplits[0]));
                    return true;

                case "inhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("in ", "")
                                                    .Replace(" and", "")
                                                    .Replace(" hours", "")
                                                    .Replace(" hour", "")
                                                    .Replace(" minutes", "")
                                                    .Replace(" minute", "");

                    SetDate(DateTime.Today);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute);
                    
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddHours(int.Parse(lStrSplits[0]))
                                                                                .AddMinutes(int.Parse(lStrSplits[1]));

                    return true;
                    
                default:
                    return false;
            }
        }
        
        private void SetNextDayOfWeek(string iSpeechUtterance)
        {
            // Extract day of week
            string lStrDayOfWeek = iSpeechUtterance.Trim(' ').Split(' ').Length > 1 ? iSpeechUtterance.Trim(' ').Split(' ')[1] : iSpeechUtterance;

            int lIDaysToAdd = (7 + (int)Enum.Parse(typeof(DayOfWeek), lStrDayOfWeek) - (int)DateTime.Today.DayOfWeek) % 7;

            if (0 == lIDaysToAdd) // Same day of the week
            {
                lIDaysToAdd += 7;
            }

            SetDate(DateTime.Today.AddDays(lIDaysToAdd));
        }

        private void SetNextDay(string iSpeechUtterance)
        {
            // Get the word for the targeted day and remove useless characters.
            string lStrDay = iSpeechUtterance.Trim(' ').Split(' ')[1].Replace("-", "");
            
            // EDayNumber enum allows to switch from string writen numbers and int.
            int lDayNumber = (int)Enum.Parse(typeof(EDayNumber), lStrDay);

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
            
            // Store day to control existence before changing
            int lIDay = 0;
            int lIMonth = 0;

            // Find Month and Day
            foreach (string str in lNextDate)
            {
                // Ignore if not a day or month
                if (str.Equals("the") || str.Equals("on") || str.Equals("of"))
                {
                    continue;
                }

                //EMonths
                EMonths lMonthResult;
                if (Enum.TryParse<EMonths>(str, true, out lMonthResult))
                {
                    lIMonth = (int)lMonthResult;
                    continue;
                }

                //EDayNumber
                EDayNumber lDayResult;
                if (Enum.TryParse<EDayNumber>(str, true, out lDayResult))
                {
                    lIDay = (int)lDayResult;
                    continue;
                }
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
            string lStrDate = iSpeechUtterance.Substring(0, iSpeechUtterance.LastIndexOf(" "));
            string lStrYear = iSpeechUtterance.Substring(iSpeechUtterance.LastIndexOf(" "));
            
            // Store day to control existence before changing
            int lIDay = 0;
            int lIMonth = 0;
            int lIYear = int.Parse(lStrYear);

            // Find Month and Day
            foreach (string str in lStrDate.Trim(' ').Split(' '))
            {
                // Ignore if not a day or month
                if (str.Equals("the") || str.Equals("on") || str.Equals("of"))
                {
                    continue;
                }

                //EMonths
                EMonths lMonthResult;
                if (Enum.TryParse<EMonths>(str, true, out lMonthResult))
                {
                    lIMonth = (int)lMonthResult;
                    continue;
                }

                //EDayNumber
                EDayNumber lDayResult;
                if (Enum.TryParse<EDayNumber>(str, true, out lDayResult))
                {
                    lIDay = (int)lDayResult;
                    continue;
                }
            }

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
        
        private void SetInDays (string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIDay = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(lIDay));
        }

        private void SetInWeeks(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIWeeks = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(7 * lIWeeks));
        }

        private void SetInMonths(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIMonths = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddMonths(lIMonths));
        }

        private void SetInYears(string iSpeechUtterance)
        {
            // Get the index from the speech
            int lIYears = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddYears(lIYears));
        }

        private int GetMinutesFromPreString(string iStrMinutes)
        {
            Debug.LogError("test: " + iStrMinutes);
            string[] lStrSplits = iStrMinutes.Replace("a ", "").Trim(' ').Split(' ');
            
            // Quarter
            if (lStrSplits[0].Equals("quarter"))
            {
                if (lStrSplits[1].Equals("past"))
                {
                    return 15;
                }
                else
                {
                    return -15;
                }
            }

            // Half
            if (lStrSplits[0].Equals("half"))
            {
                return 30;
            }

            // Minutes past
            if (lStrSplits[1].Equals("past"))
            {
                return int.Parse(lStrSplits[0]);
            }

            // Minutes to
            if (lStrSplits[1].Equals("to"))
            {
                return -int.Parse(lStrSplits[0]);
            }

            return 0;
        }

        private int GetMinutesFromPostString(string iStrMinutes)
        {
            Debug.LogError("test: " + iStrMinutes);
            string[] lStrSplits = iStrMinutes.Replace("a ", "")
                                                .Replace("and ", "")
                                                .Replace("past ", "")
                                                .Replace(" minutes", "")
                                                .Replace(" minute", "")
                                                .Trim(' ').Split(' ');

            // Quarter
            if (lStrSplits[0].Equals("quarter"))
            {
                return 15;
            }

            // Half
            if (lStrSplits[0].Equals("half"))
            {
                return 30;
            }

            // Minutes
            return int.Parse(lStrSplits[0]);
        }

        public override bool ExtractHourFromSpeech(string iSpeech)
        {
            DayMoment lDayMoment = DayMoment.UNKNOW;
            List<int> lNumbers;

            // Check if the user asked AM or PM - Check if it's useful or not
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
                {
                    return HourSet(0, 0, lDayMoment);
                }
                else if (ContainsOneOf(iSpeech, "noon"))
                    return HourSet(12, 0, lDayMoment);
            }
            // One numbers in the speech
            else if (lNumbers.Count == 1)
            {
                if (ContainsOneOf(iSpeech, "oclock"))
                    return HourSet(lNumbers[0], 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "half"))
                    return HourSet(lNumbers[0], 30, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarterto"))
                    return HourSet((lNumbers[0] - 1), 45, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarter"))
                    return HourSet(lNumbers[0], 15, lDayMoment);
                else if (ContainsOneOf(iSpeech, "intime"))
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
            }
            // Two numbers in the speech
            else if (lNumbers.Count >= 2)
            {
                // Ex: 10 past 8 => 8h10 - So use lSeconNum to hours and lMinuteNum to minutes
                if (ContainsOneOf(iSpeech, "pasthour"))
                    return HourSet(lNumbers[1], lNumbers[0], lDayMoment);
                // Ex: 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
                else if (ContainsOneOf(iSpeech, "to"))
                    return HourSet(lNumbers[1] - 1, 60 - lNumbers[0], lDayMoment);
                // Ex: in 2 hours and 10 minutes
                else if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour") && ContainsOneOf(iSpeech, "minute"))
                {
                    int lMinutesToAdd = (lNumbers[0] * 60) + lNumbers[1];
                    ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Date + DateTime.Now.AddMinutes(lMinutesToAdd).TimeOfDay;
                    return true;
                }
            }
            return false;
        }

        public override string DateToString()
        {
            return ReminderDateManager.GetInstance().ReminderDate.ToString("MM/dd/yyyy");
        }
    }
}