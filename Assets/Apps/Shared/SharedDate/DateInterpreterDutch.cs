using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class DateInterpreterDutch : DateInterpreter
    {
        private enum EDayNumber
        {
            eerste = 1,
            tweede = 2,
            derde = 3,
            vierde = 4,
            vijfde = 5,
            zesde = 6,
            zevende = 7,
            achtste = 8,
            negende = 9,
            tiende = 10,
            elfde = 11,
            twaalfde = 12,
            dertiende = 13,
            veertiende = 14,
            vijftiende = 15,
            zestiende = 16,
            zeventiende = 17,
            achttiende = 18,
            negentiende = 19,
            twintigste = 20,
            eenentwintigste = 21,
            tweeentwintigste = 22,
            drieentwintigste = 23,
            vierentwintigste = 24,
            vijfentwintigste = 25,
            zesentwintigste = 26,
            zevenentwintigste = 27,
            achtentwintigste = 28,
            negenentwintigste = 29,
            dertigste = 30,
            eenendertigste = 31
        }

        private enum EDayOfWeek
        {
            maandag = 0,
            dinsdag = 1,
            woensdag = 2,
            donderdag = 3,
            vrijdag = 4,
            zaterdag = 5,
            zondag = 6
        }
        
        private enum EMonths
        {
            januari = 1,
            februari = 2,
            maart = 3,
            april = 4,
            mei = 5,
            juni = 6,
            juli = 7,
            augustus = 8,
            september = 9,
            oktober = 10,
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
            // The day is the last word of the utterance
            string[] iSpeechList = iSpeechUtterance.ToLower().Trim(' ').Split(' ');
            string lStrDayOfWeek = iSpeechList.Length > 0 ? iSpeechList[iSpeechList.Length - 1] : iSpeechUtterance;

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

            string lStrMonth = lNextDate[lNextDate.Length - 1];
            string lStrDay = lNextDate[lNextDate.Length - 2];

            // Store day to control existence before changing
            int lIDay = 0;
            EDayNumber lDayResult;
            if (Enum.TryParse<EDayNumber>(lStrDay, true, out lDayResult))
            {
                lIDay = (int)lDayResult;
            }
            else
            {
                int.TryParse(lStrDay, out lIDay);
            }

            // Set month
            int lIMonth = 0;
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
            string lStrDay = lNextDate[lNextDate.Length - 3];

            // Store day to control existence before changing
            int lIDay = 0;
            EDayNumber lDayResult;
            if (Enum.TryParse<EDayNumber>(lStrDay, true, out lDayResult))
            {
                lIDay = (int)lDayResult;
            }
            else
            {
                int.TryParse(lStrDay, out lIDay);
            }

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
        
        public override string DateToString(DateTime iDate)
        {
            return iDate.ToString("d", CultureInfo.CreateSpecificCulture("nl-NL"));
        }
    }
}