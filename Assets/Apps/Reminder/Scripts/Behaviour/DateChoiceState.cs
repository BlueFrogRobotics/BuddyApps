using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class DateChoiceState : AStateMachineBehaviour
    {
        private const int JANUARY = 1;
        private const int DECEMBER = 12;
        private const int TRY_NUMBER = 2;
        private const string DATE_FORMAT = "dd/MM/yyyy";
        private int mListen;
        private SpeechInputParameters mGrammar;

        // TMP
        private const string DC_GREEN = "<color=green>-----";
        private const string DC_RED = "<color=red>-----";
        private const string DC_BLUE = "<color=blue>-----";
        private const string DC_END = "-----</color>";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListen = 0;

            // Setting of the grammar for STT
            ReminderData.Instance.DateChoice = "";
            mGrammar = new SpeechInputParameters();
            mGrammar.Grammars = new string[] { "reminder" };

            // Setting of the callback
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => { VoconGetResult(iSpeechInput); });
        }

        private void VoconGetResult(SpeechInput iSpeechInput)
        {
            Debug.Log(DC_BLUE + "SPEECH.ToString: " + iSpeechInput.ToString() + DC_END);
            Debug.Log(DC_BLUE + "SPEECH.Utterance: " + iSpeechInput.Utterance + DC_END);
            ReminderData.Instance.DateChoice = iSpeechInput.Utterance;
            mListen++;
            if (iSpeechInput.Utterance.Length > 0)
            {
                ReminderData.Instance.DateChoice = ExtractDateFromSpeech(iSpeechInput.Utterance);
                Debug.Log(DC_GREEN + "DATE IS: " + ReminderData.Instance.DateChoice + DC_END);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mListen < TRY_NUMBER && string.IsNullOrEmpty(ReminderData.Instance.DateChoice))
            {
                if (!Buddy.Vocal.IsBusy)
                {
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("when"), mGrammar.Grammars);
                }
            }
            else if (mListen >= TRY_NUMBER || Input.GetKeyDown(KeyCode.RightArrow))
                Trigger("DateEntryState");

            // Temporary reset to quick test
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ReminderData.Instance.DateChoice = "";
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Trigger("DateEntryState");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("------ NEXT STEP ------");
            Buddy.Vocal.Stop();
        }

        //  ---- PARSING FUNCTION ---- 

        private string ExtractDateFromSpeech(string iSpeech)
        {
            if (ContainsOneOf(iSpeech, "today"))
                return DateTime.Today.ToString(DATE_FORMAT);
            else if (ContainsOneOf(iSpeech, "tomorrow"))
                return DateTime.Today.AddDays(1F).ToString(DATE_FORMAT);
            else if (ContainsOneOf(iSpeech, "dayaftertomorrow"))
                return DateTime.Today.AddDays(2F).ToString(DATE_FORMAT);
            else if (ContainsOneOf(iSpeech, "weekdays"))
                return GetNextDay(DateTime.Today, iSpeech.Split(' '));
            else if (ContainsOneOf(iSpeech, "weekdays") || ContainsOneOf(iSpeech, "the") || ContainsOneOf(iSpeech, "allmonth"))
                return TryToBuildDate(iSpeech);
            else if (ContainsOneOf(iSpeech, "intime"))
            {
                UInt16 lNb = 0;
                string[] lWords = iSpeech.Split(' ');
                // If the speech contain less than 3 word, the order is inconsistent
                if (lWords.Length < 3)
                    return "";
                for (int lIndex = 0; lIndex < lWords.Length; ++lIndex)
                {
                    if (lWords[lIndex].ToLower() == Buddy.Resources.GetString("intime") && lIndex + 2 < lWords.Length)
                    {
                        // Check if is't necessary to test the presence of the word "day" / "month" / "year", maybe the grammar of vocon is enough
                        if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "day"))
                            return DateTime.Today.AddDays(lNb).ToString(DATE_FORMAT);
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "month"))
                            return DateTime.Today.AddMonths(lNb).ToString(DATE_FORMAT);
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "year"))
                            return DateTime.Today.AddYears(lNb).ToString(DATE_FORMAT);
                    }
                }
            }
            return null;
        }

        // Try to build a date - Find day / number, month,
        // If number or month is absent - check if "next" is present -> GetNextDay
        // Find a day number
        private string TryToBuildDate(string iSpeech)
        {
            DateTime lDate;
            UInt16 lDayNum = 0;
            int lMonth = DateTime.Today.Month;
            int lYear = DateTime.Today.Year;
            string lResult = null;
            string[] lSpeechWords = iSpeech.Split(' ');

            // Get a number in speech
            foreach (string lWord in lSpeechWords)
            {
                if (UInt16.TryParse(lWord, out lDayNum))
                    break;
            }
            // If the day number is valid and a month is choose, create a date
            if (lDayNum >= 1 && lDayNum <= 31 && ContainsOneOf(iSpeech, "allmonth"))
            {
                // Find the choosen month - If an error occured, the date extraction is stopped.
                if ((lMonth = FindMonthNumber(lSpeechWords)) < 0)
                    return "";
                // If the month number is passed OR, if the day is passed and the choosen month is the current month, increment the year.
                if (lMonth < DateTime.Today.Month || (lMonth == DateTime.Today.Month && lDayNum < DateTime.Today.Day))
                    lYear++;
                lDate = new DateTime(lYear, lMonth, lDayNum);
                lResult = lDate.ToString(DATE_FORMAT);
            }
            // If the day number is valid but without month, we create a date to the next day number.
            else if (lDayNum >= 1 && lDayNum <= 31)
            {
                // If the day number is passed, use the next month
                if (lDayNum < DateTime.Today.Day)
                    lMonth++;
                // Set to january if we were in december - Happy new year !
                if (lMonth > DECEMBER)
                {
                    lMonth = JANUARY;
                    lYear++;
                }
                lDate = new DateTime(lYear, lMonth, lDayNum);
                lResult = lDate.ToString(DATE_FORMAT);
            }
            // If no number is choose => speech = "the" + "next" ([TODO] Make this impossible with the grammar) || "weekdays" + "next"
            else if (lDayNum == 0 && ContainsOneOf(iSpeech, "next"))
                return GetNextDay(DateTime.Today, lSpeechWords);
            return lResult;
        }

        /*
         *  TMP - waiting for bug fix in utils - bug with "intime"
         */

        private bool ContainsOneOf(string iSpeech, string iKey)
        {
            return ContainsOneOfFromWeatherApp(iSpeech, iKey);
         //   return Utils.ContainsOneOf(iSpeech, Buddy.Resources.GetPhoneticStrings(iKey));
        }

        private bool ContainsOneOfFromWeatherApp(string iSpeech, string iKey)
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

        /*
         *  Return the number of the first month found in iSpeech.
         *  If no month are found, negative value is return.
         */
        private int FindMonthNumber(string[] iSpeech)
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
        private string GetNextDay(DateTime iStartDay, string[] iSpeech)
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
                return null;
            // Substract the choosen day number with the current day number
            // The calcul assure the range is between [1-7]
            lDaysToAdd = ((lNumDay - (int)iStartDay.DayOfWeek + 7) % 7) + 1;
            return iStartDay.AddDays(lDaysToAdd).ToString(DATE_FORMAT);
        }
    }
}
