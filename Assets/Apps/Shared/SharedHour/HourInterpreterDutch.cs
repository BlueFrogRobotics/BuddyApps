using BlueQuark;

using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace BuddyApp.Shared
{
    public class HourInterpreterDutch : HourInterpreter
    {
        public override bool ExtractHourFromSpeech(string iSpeechRule, string iSpeechUtterance, ref DateTime ioDateTime)
        {
            string lStrMinutes = iSpeechUtterance;
            string[] lStrSplits = iSpeechUtterance.Replace("om ", "")
                                                .Replace(" en", "")
                                                .Replace(" 's middags", "")
                                                .Replace(" middags", "")
                                                .Replace(" 's avonds", "")
                                                .Replace(" in de avond", "")
                                                .Replace(" 's ochtends", "")
                                                .Replace(" 's nachts", "")
                                                .Replace(" 's morgens", "")
                                                .Replace(" avonds", "")
                                                .Replace(" minuten", "")
                                                .Replace(" minuut", "")
                                                .Replace(" uren", "")
                                                .Replace(" uur", "")
                                                .Trim(' ').Split(' ');

            string[] lStrSplits_over = iSpeechUtterance.Replace("om ", "")
                                        .Replace(" en", "")
                                        .Replace(" 's middags", "")
                                        .Replace(" middags", "")
                                        .Replace(" 's avonds", "")
                                        .Replace(" in de avond", "")
                                        .Replace(" 's ochtends", "")
                                        .Replace(" 's nachts", "")
                                        .Replace(" 's morgens", "")
                                        .Replace(" avonds", "")
                                        .Replace(" minuten", "")
                                        .Replace(" minuut", "")
                                        .Replace(" uren", "")
                                        .Replace(" uur", "")
                                        .Replace("over", "")
                                        .Trim(' ').Split(' ');

            int hh = 0;
            int mm = 0;
            bool isPm = false;
            if (iSpeechRule.StartsWith("atpm"))
                isPm = true;

            switch (iSpeechRule)
            {
                case "atnoon":
                    SetHour(12, 0, ref ioDateTime);
                    return true;

                case "atpmhours":
                case "atamhours":
                    
                    if (lStrSplits.Length > 0
                        && int.TryParse(lStrSplits[0], out hh))
                    {
                        SetHourInDay(isPm, hh, 0, ref ioDateTime);
                        return true;
                    }
                    return false;

                case "atpmhalfhours":
                case "atamhalfhours":

                    if (lStrSplits.Length > 1
                        && int.TryParse(lStrSplits[1], out hh))
                    {
                        SetHourInDay(isPm, hh, -30, ref ioDateTime);
                        return true;
                    }
                    return false;

                case "atpmhoursandminutes":
                case "atamhoursandminutes":
                    // <hours> uur <minutes>
                    if (lStrSplits.Length > 1
                        && int.TryParse(lStrSplits[0], out hh)
                        && int.TryParse(lStrSplits[1], out mm))
                    {
                        SetHourInDay(isPm, hh, mm, ref ioDateTime);
                        return true;
                    }
                    return false;

                case "atpmminuteshours":
                case "atamminuteshours":
                    // (<minutes> | kwart) [minuten] (over | voor) [half] <hours>
                    if (lStrSplits.Length > 2
                        && int.TryParse(lStrSplits[lStrSplits.Length-1], out hh))
                    {
                        if (lStrSplits[0] == "kwart")
                            mm = 15;
                        else if (!int.TryParse(lStrSplits[0], out mm))
                            return false;

                        if (lStrSplits[1] == "voor")
                            mm *= -1;

                        if (lStrSplits.Length > 3
                            && lStrSplits[2] == "half")
                            mm += -30;

                        SetHourInDay(isPm, hh, mm, ref ioDateTime);

                        return true;
                    }
                    return false;

                case "in_hours":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddHours(int.Parse(lStrSplits_over[0]));
                    return true;

                case "in_minutes":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);
                    ioDateTime = ioDateTime.AddMinutes(int.Parse(lStrSplits_over[0]));
                    return true;

                case "in_hoursandminutes":
                    SetDate(DateTime.Today, ref ioDateTime);
                    SetHour(DateTime.Now.Hour, DateTime.Now.Minute, ref ioDateTime);

                    ioDateTime = ioDateTime.AddHours(int.Parse(lStrSplits_over[0])).AddMinutes(int.Parse(lStrSplits_over[1]));

                    return true;

                default:
                    return false;
            }
        }

        void SetHourInDay(bool isPm, int iHour, int iMinutes, ref DateTime ioDateTime)
        {
            if (iHour <= 12 && isPm)
                iHour += 12;
            SetHour(iHour, iMinutes, ref ioDateTime);
        }
    }
}