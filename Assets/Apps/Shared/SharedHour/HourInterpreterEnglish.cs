using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class HourInterpreterEnglish : HourInterpreter
    {
        public override bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
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
                    SetHour(12, 0, ref ioDateTime);
                    return true;

                case "atprenoon":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" noon", "");

                    SetHour(12, GetMinutesFromPreString(lStrMinutes), ref ioDateTime);
                    return true;

                case "atpostnoon":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace("noon ", "");

                    SetHour(12, GetMinutesFromPostString(lStrMinutes), ref ioDateTime);
                    return true;

                case "atmidnight":
                    SetHour(0, 0, ref ioDateTime);
                    return true;

                case "atpremidnight":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" midnight", "");

                    SetHour(0, GetMinutesFromPreString(lStrMinutes), ref ioDateTime);
                    return true;

                case "atpostmidnight":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace("midnight ", "");

                    SetHour(0, GetMinutesFromPostString(lStrMinutes), ref ioDateTime);
                    return true;

                case "athours":
                    SetHour(int.Parse(lStrSplits[0]), 0, ref ioDateTime);
                    return true;

                case "atprehours":
                    lStrMinutes = iSpeechUtterance.Replace("at", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length - 1]), GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))), ref ioDateTime);
                    return true;

                case "atposthours":
                    lStrMinutes = iSpeechUtterance.Replace("at", "");

                    SetHour(int.Parse(lStrSplits[0]), GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;

                case "atamhours":
                    SetHour(int.Parse(lStrSplits[0]), 0, ref ioDateTime);
                    return true;
            
                case "atpreamhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" am", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length - 1]), GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))), ref ioDateTime);
                    return true;

                case "atpostamhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" am", "");

                    SetHour(int.Parse(lStrSplits[0]), GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;

                case "atpmhours":
                    SetHour(int.Parse(lStrSplits[0]) + 12, 0, ref ioDateTime);
                    return true;

                case "atprepmhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" pm", "");

                    SetHour(int.Parse(lStrSplits[lStrSplits.Length-1]) + 12, GetMinutesFromPreString(lStrMinutes.Substring(0, lStrMinutes.LastIndexOf(lStrSplits[lStrSplits.Length - 1]))), ref ioDateTime);
                    return true;

                case "atpostpmhours":
                    lStrMinutes = iSpeechUtterance.Replace("at ", "")
                                                    .Replace(" pm", "");
                    
                    SetHour(int.Parse(lStrSplits[0]) + 12, GetMinutesFromPostString(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;
                    
                case "inhours":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddHours(int.Parse(lStrSplits[0]));
                    return true;

                case "inminutes":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddMinutes(int.Parse(lStrSplits[0]));
                    return true;

                case "inhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("in ", "")
                                                    .Replace(" and", "")
                                                    .Replace(" hours", "")
                                                    .Replace(" hour", "")
                                                    .Replace(" minutes", "")
                                                    .Replace(" minute", "");

                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);

                    ioDateTime = ioDateTime.AddHours(int.Parse(lStrSplits[0])).AddMinutes(int.Parse(lStrSplits[1]));

                    return true;
                    
                default:
                    return false;
            }
        }
        
        private int GetMinutesFromPreString(string iStrMinutes)
        {
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

        public override string HourToString(DateTime iDate)
        {
            return iDate.ToString("t", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}