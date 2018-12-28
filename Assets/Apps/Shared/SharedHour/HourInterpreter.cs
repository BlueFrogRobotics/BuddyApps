using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Shared
{
    public class HourInterpreter
    {
        virtual public bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
        {
            return false;
        }
        
        virtual public bool ExtractHourFromSpeech(string iSpeech)
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
    }
}