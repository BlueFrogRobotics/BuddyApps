using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class RecurrenceInterpreter
    {
        virtual public bool ExtractRecurrenceFromSpeech(string iSpeechRule, string iSpeechUtterance, ref RepetitionTime oRepetitionTime, ref List<DayOfWeek> oRepetitionDays)
        {
            return false;
        }

        virtual public void SetDate(DateTime iNewDate, ref DateTime ioSetDate)
        {
            ioSetDate = new DateTime(
                iNewDate.Year,
                iNewDate.Month,
                iNewDate.Day,
                ioSetDate.Hour,
                ioSetDate.Minute,
                0);
        }

        virtual public void SetHour(int iHour, int iMinute, ref DateTime ioSetDate)
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

            ioSetDate = new DateTime(
                ioSetDate.Year,
                ioSetDate.Month,
                ioSetDate.Day,
                iHour,
                iMinute,
                0);
        }

        virtual public string RecurrenceToString(RepetitionTime iRepetitionTime, List<DayOfWeek> iRepetitionDays)
        {
            return "";
        }
    }
}