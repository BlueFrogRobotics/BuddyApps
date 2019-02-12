using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class HourInterpreterFrench : HourInterpreter
    {
        public override bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
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
                    SetHour(12, 0, ref ioDateTime);
                    return true;

                case "atnoonandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("midi", "");

                    SetHour(12, MinutesToInt(lStrMinutes.Substring(lStrMinutes.IndexOf(lStrSplits[0]))), ref ioDateTime);
                    return true;

                case "atmidnight":
                    SetHour(0, 0, ref ioDateTime);
                    return true;

                case "atmidnightandminutes":
                    SetHour(0, MinutesToInt(lStrSplits[lStrSplits.Length - 1]), ref ioDateTime);

                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("minuit", "");

                    SetHour(0, MinutesToInt(lStrMinutes.Substring(lStrMinutes.IndexOf(lStrSplits[0]))), ref ioDateTime);
                    return true;

                case "athours":
                    SetHour(StringToInt(lStrSplits[0]), 0, ref ioDateTime);
                    return true;
                    
                case "athoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("heures", "")
                                                    .Replace("heure", "");

                    SetHour(StringToInt(lStrSplits[0]), MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;

                case "atamhours":
                    SetHour(StringToInt(lStrSplits[0]), 0, ref ioDateTime);
                    return true;

                case "atamhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à", "")
                                                    .Replace("heures", "")
                                                    .Replace("heure", "")
                                                    .Replace("du matin", "");

                    SetHour(StringToInt(lStrSplits[0]), MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;
                    
                case "atpmhours":
                    lStrMinutes = iSpeechUtterance.Replace("à ", "")
                                                    .Replace("heures ", "")
                                                    .Replace("heure ", "")
                                                    .Replace("de l'après-midi", "")
                                                    .Replace("du soir", "");
                    
                    SetHour(StringToInt(lStrSplits[0]) + 12, 0, ref ioDateTime);
                    return true;

                case "atpmhoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("à ", "")
                                                    .Replace(" heures", "")
                                                    .Replace(" heure", "")
                                                    .Replace(" de l'après-midi", "")
                                                    .Replace(" du soir", "");
                    
                    SetHour(StringToInt(lStrSplits[0]) + 12, MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))), ref ioDateTime);
                    return true;

                case "in_hours":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddHours(StringToInt(lStrSplits[0]));
                    return true;

                case "in_minutes":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddMinutes(StringToInt(lStrSplits[0]));
                    return true;

                case "in_hoursandminutes":
                    lStrMinutes = iSpeechUtterance.Replace("dans ", "")
                                                    .Replace(" heures", "")
                                                    .Replace(" heure", "");

                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);

                    ioDateTime = ioDateTime.AddHours(StringToInt(lStrSplits[0])).AddMinutes(MinutesToInt(lStrMinutes.Substring(lStrMinutes.LastIndexOf(lStrSplits[1]))));
                    return true;

                default:
                    return false;
            }
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

        public override string HourToString(DateTime iDate)
        {
            return iDate.ToString("t", CultureInfo.CreateSpecificCulture("fr-FR"));
        }
    }
}