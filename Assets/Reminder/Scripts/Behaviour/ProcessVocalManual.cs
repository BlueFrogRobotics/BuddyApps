using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace BuddyApp.Reminder
{
    public class ProcessVocalManual : IProcessVocal
    {
        public Command ExtractParameters(string iCommand)
        {
            Command lCommand = new Command();

            lCommand.AddDate = DateTime.Now;
            lCommand.RemindDate = ExtractDate(iCommand);

            if(iCommand.ToLower().Contains("rappelle"))
            {
                lCommand.Intent = Intent.ADD;
            }

            else if (iCommand.ToLower().Contains("montre"))
            {
                lCommand.Intent = Intent.PRINT;
            }

            return lCommand;
        }

        private DateTime ExtractDate(string iSpeech)
        {
            string date="";
            string month="";
            string time="";
            DateTime departureDate;
            bool departureDateSet;
            bool departureTimeSet;
            List<string> monthsList;
            List<string> daysList;

            monthsList = new List<string>();
            daysList = new List<string>();
            monthsList.Add("janvier");
            monthsList.Add("février");
            monthsList.Add("mars");
            monthsList.Add("avril");
            monthsList.Add("mai");
            monthsList.Add("juin");
            monthsList.Add("juillet");
            monthsList.Add("août");
            monthsList.Add("septembre");
            monthsList.Add("octobre");
            monthsList.Add("novembre");
            monthsList.Add("décembre");

            daysList.Add("dimanche");
            daysList.Add("lundi");
            daysList.Add("mardi");
            daysList.Add("mercredi");
            daysList.Add("jeudi");
            daysList.Add("vendredi");
            daysList.Add("samedi");
            

            departureDate = DateTime.Now;
            /*} else {
                List<int> days = ContainsDay(iSpeech, daysList);
                if (days.Count == 1) {

                link.departureDateSet = true;
                */
            if (string.IsNullOrEmpty(month))
            {
                List<int> months = ContainsMonth(iSpeech.ToLower(), monthsList);
                 if (months.Count == 1)
                 {
                    //departureDate.Month = months[0];
                     /*month = monthsList[months[0] - 1];
                     int deltaMonth = departureDate.Month - months[0];
                     if (deltaMonth >= 0)
                     {
                         departureDate = departureDate.AddMonths(deltaMonth);
                     }
                     else
                     {
                         departureDate = departureDate.AddMonths(12 + deltaMonth);
                     }*/
                 }

            }

            if (string.IsNullOrEmpty(date))
            {
                if (iSpeech.Contains("aujourd'hui"))
                {
                    date = "aujourd'hui";
                    departureDateSet = true;
                    Debug.Log("date ajd: " + departureDate);
                }
                else if (iSpeech.Contains("après demain"))
                {
                    date = "après demain";
                    departureDate = departureDate.AddDays(2);
                    departureDateSet = true;
                    Debug.Log("date apdm: " + departureDate);
                }
                else if (iSpeech.Contains("demain"))
                {
                    date = "demain";
                    departureDate = departureDate.AddDays(1);
                    departureDateSet = true;
                    Debug.Log("date dm: " + departureDate);
                }
                else
                {
                    List<int> dates = ContainsDate(iSpeech);
                    List<int> months = ContainsMonth(iSpeech.ToLower(), monthsList);
                    List<int> hours = ContainsHour(iSpeech);
                    if (dates.Count == 1)
                    {
                        Debug.Log("nombre de heure: " + hours.Count);
                        if(hours.Count!=1)
                            departureDate = new DateTime(departureDate.Year, months[0]/*departureDate.Month*/, dates[0], departureDate.Hour, 0, 0, 0, System.DateTimeKind.Utc);
                        else
                            departureDate = new DateTime(departureDate.Year, months[0]/*departureDate.Month*/, dates[0], hours[0], 0, 0, 0, System.DateTimeKind.Utc);
                        date = "le " + departureDate.ToString("dddd", new CultureInfo("fr-FR")) + " " + dates[0].ToString();
                        Debug.Log("date le: " + departureDate);
                    }

                }
                // TODO Get the date
                if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(date))
                    departureDateSet = true;
            }

            if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(month))
            {
                List<string> days = ContainsDay(iSpeech, daysList);
                Debug.Log("nb days: " + days.Count);
                if (days.Count == 1)
                {
                    date = days[0];
                    departureDate = GetNextWeekday(departureDate.AddDays(1), daysList.IndexOf(days[0].ToLower()));
                    departureDateSet = true;
                    List<int> hours = ContainsHour(iSpeech);
                    if (hours.Count == 1)
                        departureDate = new DateTime(departureDate.Year, departureDate.Month, departureDate.Day, hours[0], 0, 0, 0, System.DateTimeKind.Utc);
                    Debug.Log("date with day: " + departureDate);
                }
            }

            return departureDate;
        }

        private List<int> ContainsDate(string iSpeech)
        {
            List<int> result = new List<int>();



            string[] words = iSpeech.Split(' ');
            for (int iw = 0; iw < words.Length; ++iw)
            {
                for (int i = 1; i < 32; ++i)
                {
                    //Debug.Log("iMonthList actuel " + iMonthsList[i]);
                    //Debug.Log("iword actuel " + word);
                    if (words[iw].ToLower() == "le" && iw + 1 < words.Length)
                    {
                        if (words[iw + 1] == i.ToString())
                        {

                            result.Add(i);
                            Debug.Log("contains date: " + i);
                        }
                    }
                }

                /*if (iSpeech.Contains("le " + i.ToString() + " ")) {
                    result.Add(i);
                    Debug.Log("contains date: " + i);
                }*/
            }
            return result;
        }

        private List<string> ContainsDay(string iSpeech, List<string> iDaysList)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < iDaysList.Count; ++i)
            {

                //Debug.Log("iSpeech: " + iSpeech);
                //Debug.Log("iStationList: " + iStationsList[i] );
                if (iSpeech.ToLower().Contains(iDaysList[i].ToLower()))
                {
                    result.Add(iDaysList[i]);
                    Debug.Log("contains day: " + iDaysList[i]);
                }
            }

            return result;
        }

        private List<int> ContainsMonth(string iSpeech, List<string> iMonthsList)
        {
            List<int> result = new List<int>();
            string[] words = iSpeech.Split(' ');
            foreach (string word in words)
            {
                for (int i = 0; i < iMonthsList.Count; ++i)
                {
                    //Debug.Log("iMonthList actuel " + iMonthsList[i]);
                    //Debug.Log("iword actuel " + word);
                    if (word.ToLower() == iMonthsList[i])
                    {
                        result.Add(i + 1);
                        Debug.Log("contains Month: " + (i + 1));
                    }
                }
            }
            /*for (int i = 0; i < iMonthsList.Count; ++i) {
                Debug.Log("iMonthList actuel " + iMonthsList[i]);
                Debug.Log("iSpeech actuel " + iSpeech);
                //Debug.Log("iSpeech: " + iSpeech);
                //Debug.Log("iStationList: " + iStationsList[i] );
                if (iSpeech.Contains(" " + iMonthsList[i].ToLower() + " " )) {
                    result.Add(i + 1);
                    Debug.Log("contains Month: " + (i + 1));
                }
            }*/

            return result;
        }

        public static DateTime GetNextWeekday(DateTime start, int dayOfWeek)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = (dayOfWeek - (int)start.DayOfWeek + 7) % 7;
            Debug.Log("week day " + dayOfWeek + " day to add " + daysToAdd);
            return start.AddDays(daysToAdd);
        }

        private List<int> ContainsHour(string iSpeech)
        {
            List<int> result = new List<int>();

            string[] words = iSpeech.Split(' ');
            for (int iw = 1; iw < words.Length; ++iw)
            {
                for (int i = 0; i < 24; ++i)
                {
                    //Debug.Log("iMonthList actuel " + iMonthsList[i]);
                    //Debug.Log("iword actuel " + word);
                    if ((words[iw].ToLower() == "h" || words[iw].ToLower() == "heures" || words[iw].ToLower() == "heure"))
                    {
                        if (words[iw - 1] == i.ToString())
                        {

                            result.Add(i);
                            Debug.Log("contains hour: " + i);
                        }
                    }
                }
            }

            return result;
        }
    }
}