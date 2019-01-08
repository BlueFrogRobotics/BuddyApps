using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class DateInterpreterEnglish : DateInterpreter
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

        public override bool ExtractDateFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
        {
            switch (iSpeechRule)
            {
                case "today":
                    SetDate(DateTime.Today.Date, ref ioDateTime);
                    return true;

                case "tomorrow":
                    SetDate(DateTime.Today.AddDays(1D).Date, ref ioDateTime);
                    return true;

                case "dayaftertomorrow":
                    SetDate(DateTime.Today.AddDays(2D).Date, ref ioDateTime);
                    return true;

                case "nextdayofweek":
                    SetNextDayOfWeek(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "nextweek":
                    SetDate(DateTime.Today.AddDays(7D).Date, ref ioDateTime);
                    return true;

                case "nextmonth":
                    SetDate(DateTime.Today.AddMonths(1).Date, ref ioDateTime);
                    return true;

                case "nextyear":
                    SetDate(DateTime.Today.AddYears(1).Date, ref ioDateTime);
                    return true;

                case "nextday":
                    SetNextDay(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "nextdate":
                    return SetNextDate(iSpeechUtterance, ref ioDateTime);

                case "fulldate":
                    return SetFullDate(iSpeechUtterance, ref ioDateTime);

                case "indays":
                    SetInDays(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "inweeks":
                    SetInWeeks(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "inmonths":
                    SetInMonths(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "inyears":
                    SetInYears(iSpeechUtterance, ref ioDateTime);
                    return true;

                default:
                    return false;
            }
        }
        
        private void SetNextDayOfWeek(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Extract day of week
            string lStrDayOfWeek = iSpeechUtterance.Trim(' ').Split(' ').Length > 1 ? iSpeechUtterance.Trim(' ').Split(' ')[1] : iSpeechUtterance;

            int lIDaysToAdd = (7 + (int)Enum.Parse(typeof(DayOfWeek), lStrDayOfWeek) - (int)DateTime.Today.DayOfWeek) % 7;

            if (0 == lIDaysToAdd) // Same day of the week
            {
                lIDaysToAdd += 7;
            }

            SetDate(DateTime.Today.AddDays(lIDaysToAdd), ref ioDateTime);
        }

        private void SetNextDay(string iSpeechUtterance, ref DateTime ioDateTime)
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
            SetDate(lDate, ref ioDateTime);
        }

        private bool SetNextDate(string iSpeechUtterance, ref DateTime ioDateTime)
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
                SetDate(lTmpDate, ref ioDateTime);
                return true;
            }

            Debug.LogError("Incorrect date!!!");
            return false;
        }

        private bool SetFullDate(string iSpeechUtterance, ref DateTime ioDateTime)
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
            
            SetDate(lTmpDate.Year, lTmpDate.Month, lTmpDate.Day, ref ioDateTime);

            return true;
        }
        
        private void SetInDays (string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIDay = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(lIDay), ref ioDateTime);
        }

        private void SetInWeeks(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIWeeks = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(7 * lIWeeks), ref ioDateTime);
        }

        private void SetInMonths(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIMonths = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddMonths(lIMonths), ref ioDateTime);
        }

        private void SetInYears(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIYears = int.Parse(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddYears(lIYears), ref ioDateTime);
        }
        
        public override string DateToString (DateTime iDate)
        {
            return iDate.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}