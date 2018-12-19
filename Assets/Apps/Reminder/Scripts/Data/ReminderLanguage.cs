using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Reminder
{
    public class ReminderLanguage
    {
        public enum DayMoment
        {
            UNKNOW,
            AM,
            PM,
        }

        protected const int JANUARY = 1;
        protected const int DECEMBER = 12;

        virtual public bool ExtractDateFromSpeech(string iSpeechRule, string iSpeechUtterance)
        {
            return false;
        }

        virtual public bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance)
        {
            return false;
        }
        
        virtual public bool ExtractDateFromSpeech(string iSpeech)
        {
            string[] lWords = iSpeech.Split(' ');

            if (ContainsOneOf(iSpeech, "today") && lWords.Length == 1)
            {
                ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.Date;
                return true;
            }
            else if (ContainsOneOf(iSpeech, "tomorrow") && lWords.Length == 1)
            {
                ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.AddDays(1F).Date;
                return true;
            }
            else if (ContainsOneOf(iSpeech, "weekdays") && lWords.Length == 1)
            {
                ReminderDateManager.GetInstance().ReminderDate = GetNextDay(DateTime.Today, iSpeech.Split(' '));
                return true;
            }
            else if (ContainsOneOf(iSpeech, "dayaftertomorrow"))
            {
                ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.AddDays(2F).Date;
                return true;
            }
            else if (ContainsOneOf(iSpeech, "weekdays") || ContainsOneOf(iSpeech, "the") || ContainsOneOf(iSpeech, "allmonth"))
            {
                return TryToBuildDate(iSpeech);
            }
            else if (ContainsOneOf(iSpeech, "intime"))
            {
                // If the speech contain less than 3 words, the order is inconsistent
                if (lWords.Length < 3)
                    return false;

                UInt16 lNb = 0;
                for (int lIndex = 0; lIndex < lWords.Length; ++lIndex)
                {
                    if (lWords[lIndex].ToLower() == Buddy.Resources.GetString("intime") && lIndex + 2 < lWords.Length)
                    {
                        if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "day"))
                        {
                            ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.AddDays(lNb).Date;
                            return true;
                        }
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "month"))
                        {
                            ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.AddMonths(lNb).Date;
                            return true;
                        }
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "year"))
                        {
                            ReminderDateManager.GetInstance().ReminderDate = DateTime.Today.AddYears(lNb).Date;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        virtual public bool TryToBuildDate(string iSpeech)
        {
            int lMonth = DateTime.Today.Month;
            int lYear = DateTime.Today.Year;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            // Get two numbers in the speech
            lNumbers = GetNumbersInString(iSpeech, 2);

            if (lNumbers == null)
            {
                if (ContainsOneOf(iSpeech, "next"))
                {
                    ReminderDateManager.GetInstance().ReminderDate = GetNextDay(DateTime.Today, lSpeechWords);
                    return true;
                }
            }
            // If the first number is a valid day number - and a month is specidied
            else if (lNumbers[0] >= 1 && lNumbers[0] <= 31 && ContainsOneOf(iSpeech, "allmonth"))
            {
                // Find the choosen month - If an error occured, the date extraction is stopped.
                if ((lMonth = FindMonthNumber(lSpeechWords)) < 0)
                    return false;
                // If a second number is specified, it's the year
                if (lNumbers.Count > 1 && lNumbers[1] > 0 && lNumbers[1] < 9999)
                    lYear = lNumbers[1];
                // If the month is passed, OR If the month is the current month AND the day is passed, Set to the next year
                else if (lMonth < DateTime.Today.Month || (lMonth == DateTime.Today.Month && lNumbers[0] < DateTime.Today.Day))
                    lYear++;

                ReminderDateManager.GetInstance().ReminderDate = new DateTime(lYear, lMonth, lNumbers[0]);
                return true;
            }
            // If the day number is valid but without month, we create a date to the next same day number.
            else if (lNumbers[0] >= 1 && lNumbers[0] <= 31)
            {
                // If the day number is passed, use the next month
                if (lNumbers[0] < DateTime.Today.Day)
                    lMonth++;
                // Set to january if we were in december - Happy new year !
                if (lMonth > DECEMBER)
                {
                    lMonth = JANUARY;
                    lYear++;
                }
                ReminderDateManager.GetInstance().ReminderDate = new DateTime(lYear, lMonth, lNumbers[0]);
                return true;
            }

            return false;
        }


        // --- Generic Function for Extract info in Speech ---

        /*
         *  Return the number of the first month found in iSpeech.
         *  If no month are found, negative value is return.
         */
        virtual public int FindMonthNumber(string[] iSpeech)
        {
            string[] lMonthArray = Buddy.Resources.GetPhoneticStrings("allmonth");

            foreach (string lWord in iSpeech)
            {
                for (int lIndex = 0; lIndex < lMonthArray.Length; lIndex++)
                {
                    if (lMonthArray[lIndex].ToLower() == lWord.ToLower())
                        return (lIndex + 1);
                }
            }
            return -1;
        }

        /*
         *  Return the date in string format, of the next day, from a start day.
         *  (It takes the first encounter day in the array)
         *  If an error occured, a negative value is return.
         */

        virtual public DateTime GetNextDay(DateTime iStartDay, string[] iSpeech)
        {
            bool lFind = false;
            string[] lWeekDays = Buddy.Resources.GetPhoneticStrings("weekdays");
            int lNumDay = -1;
            int lDaysToAdd = 0;

            // Find the first day in the array
            foreach (string lWord in iSpeech)
            {
                for (int lIndex = 0; lIndex < lWeekDays.Length; lIndex++)
                {
                    if (lWeekDays[lIndex] == lWord)
                    {
                        lNumDay = lIndex;
                        lFind = true;
                        break;
                    }
                }
                if (lFind)
                    break;
            }
            if (lNumDay < 0)
                return new DateTime(0001, 01, 01);
            // Substract the choosen day number with the current day number
            // The calcul assure the range is between [1-7]
            lDaysToAdd = ((lNumDay - (int)iStartDay.DayOfWeek + 7) % 7) + 1;
            return iStartDay.AddDays(lDaysToAdd).Date;
        }

        /*
         *  Get all numbers in iText into a List of int.
         *  Possibility to choose the amount of numbers to extract.
         *  If iHowMany = 0, the function will extract all the number in the string.
         *  Return null on error or if no number was found
         */

        virtual public List<int> GetNumbersInString(string iText, UInt16 iHowMany)
        {
            int lNum;
            int lCount = 0;
            List<int> lNumbers = new List<int>();
            string[] lSpeechWords = iText.Split(' ');

            for (int lIndex = 0; lIndex < lSpeechWords.Length; lIndex++)
            {
                // Add number to the list, on found
                if (iHowMany != 0 && lCount >= iHowMany)
                    break;
                if (int.TryParse(lSpeechWords[lIndex], out lNum))
                {
                    lNumbers.Add(lNum);
                    lCount++;
                }
            }
            if (lNumbers.Count > 0)
                return lNumbers;
            return null;
        }

        //  TMP - Will be in shared
        virtual protected bool ContainsOneOf(string iSpeech, string iKey)
        {
            string[] iListSpeech = Buddy.Resources.GetPhoneticStrings(iKey);
            iSpeech = iSpeech.ToLower();
            for (int i = 0; i < iListSpeech.Length; ++i)
            {
                string[] words = iListSpeech[i].Split(' ');
                if (words.Length < 2)
                {
                    words = iSpeech.Split(' ');
                    foreach (string word in words)
                    {
                        if (word == iListSpeech[i].ToLower())
                        {
                            return true;
                        }
                    }
                }
                else if (iSpeech.ToLower().Contains(iListSpeech[i].ToLower()))
                    return true;
            }
            return false;
        }

        virtual public bool ExtractHourFromSpeech(string iSpeech)
        {
            return false;
        }

        virtual public bool HourSet(int iHour, int iMinute, DayMoment iDayMoment)
        {
            if (!(iHour >= 0 && iHour <= 23))
                return false;
            if (!(iMinute >= 0 && iMinute <= 59))
                return false;

            if (iDayMoment == DayMoment.AM && iHour >= 12)
            {
                iHour -= 12;
            }
            else if (iDayMoment == DayMoment.PM && iHour <= 12)
            {
                iHour += 12;
            }

            ReminderDateManager.GetInstance().ReminderDate = new DateTime(
                ReminderDateManager.GetInstance().ReminderDate.Year,
                ReminderDateManager.GetInstance().ReminderDate.Month,
                ReminderDateManager.GetInstance().ReminderDate.Day,
                iHour,
                iMinute,
                0);

            return true;
        }

        virtual public void SetHour(int iHour, int iMinute)
        {
            if (iMinute < 0)
            {
                iHour -= 1;
                iMinute = 60 + iMinute;
            }

            if (iHour < 0)
            {
                iHour += 24;
            }

            ReminderDateManager.GetInstance().ReminderDate = new DateTime(
                ReminderDateManager.GetInstance().ReminderDate.Year,
                ReminderDateManager.GetInstance().ReminderDate.Month,
                ReminderDateManager.GetInstance().ReminderDate.Day,
                iHour,
                iMinute,
                0);
        }

        virtual public void SetDate(DateTime iDate)
        {
            ReminderDateManager.GetInstance().ReminderDate = new DateTime(
                iDate.Year,
                iDate.Month,
                iDate.Day,
                ReminderDateManager.GetInstance().ReminderDate.Hour,
                ReminderDateManager.GetInstance().ReminderDate.Minute,
                0);
        }

        virtual public void SetDate(int iYear, int iMonth, int iDay)
        {
            ReminderDateManager.GetInstance().ReminderDate = new DateTime(
                iYear,
                iMonth,
                iDay,
                ReminderDateManager.GetInstance().ReminderDate.Hour,
                ReminderDateManager.GetInstance().ReminderDate.Minute,
                0);
        }

        virtual public string DateToString()
        {
            return "";
        }
    }
}