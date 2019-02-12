using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class DateInterpreterFrench : DateInterpreter
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

                case "in_days":
                    SetInDays(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "in_weeks":
                    SetInWeeks(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "in_months":
                    SetInMonths(iSpeechUtterance, ref ioDateTime);
                    return true;

                case "in_years":
                    SetInYears(iSpeechUtterance, ref ioDateTime);
                    return true;

                default:
                    return false;
            }
        }
        
        private void SetNextDayOfWeek(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Extract day of week
            string lStrDayOfWeek = iSpeechUtterance.Trim(' ').Split(' ')[0];

            int lIDaysToAdd = (7 + (int)Enum.Parse(typeof(EDayOfWeek), lStrDayOfWeek) - (int)DateTime.Today.DayOfWeek) % 7 + 1;

            if (0 == lIDaysToAdd) // Same day of the week
            {
                lIDaysToAdd += 7;
            }

            SetDate(DateTime.Today.AddDays(lIDaysToAdd), ref ioDateTime);
        }

        private void SetNextDay(string iSpeechUtterance, ref DateTime ioDateTime)
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
            SetDate(lDate, ref ioDateTime);
        }

        private bool SetNextDate(string iSpeechUtterance, ref DateTime ioDateTime)
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
                SetDate(lTmpDate, ref ioDateTime);
                return true;
            }

            Debug.LogError("Incorrect date!!!");
            return false;
        }

        private bool SetFullDate(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            DateTime lTmpDate;
            string lStrYear = iSpeechUtterance.Trim(' ').Substring(iSpeechUtterance.LastIndexOf(" "));
            string[] lNextDate = iSpeechUtterance.Trim(' ').Split(' ');
			
            // Store day to control existence before changing
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

            SetDate(lTmpDate.Year, lTmpDate.Month, lTmpDate.Day, ref ioDateTime);

            return true;
        }

        private void SetInDays(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIDay = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(lIDay), ref ioDateTime);
        }

        private void SetInWeeks(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIWeeks = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddDays(7 * lIWeeks), ref ioDateTime);
        }

        private void SetInMonths(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIMonths = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddMonths(lIMonths), ref ioDateTime);
        }

        private void SetInYears(string iSpeechUtterance, ref DateTime ioDateTime)
        {
            // Get the index from the speech
            int lIYears = StringToInt(iSpeechUtterance.Trim(' ').Split(' ')[1]);
            SetDate(DateTime.Today.AddYears(lIYears), ref ioDateTime);
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
        
        public override string DateToString(DateTime iDate)
        {
            return iDate.ToString("d", CultureInfo.CreateSpecificCulture("fr-FR"));
        }
    }
}