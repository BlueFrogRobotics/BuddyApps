using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.Shared
{
    public class DateInterpreter
    {
        virtual public bool ExtractDateFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
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

        virtual public void SetDate(int iYear, int iMonth, int iDay, ref DateTime ioSetDate)
        {
            ioSetDate = new DateTime(
                iYear,
                iMonth,
                iDay,
                ioSetDate.Hour,
                ioSetDate.Minute,
                0);
        }

        virtual public string DateToString(DateTime iDate)
        {
            return "";
        }
    }
}