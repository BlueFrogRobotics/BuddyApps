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
        private const int TRY_NUMBER = 20;
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
            DateTime lDate;
            string lResult = null;
            string[] lSpeechWords = iSpeech.Split(' ');
            int lDayNum = 0;
            int lMonth = DateTime.Today.Month;
            int lYear = DateTime.Today.Year;

            // One word in the speech
            if (lSpeechWords.Length == 1)
            {
                if (ContainOneOfPhonetics(lSpeechWords, "today"))
                    return DateTime.Today.ToString(DATE_FORMAT);
                else if (ContainOneOfPhonetics(lSpeechWords, "tomorrow"))
                    return DateTime.Today.AddDays(1F).ToString(DATE_FORMAT);
                else if (ContainOneOfPhonetics(lSpeechWords, "weekdays"))
                    return GetNextDay(DateTime.Today, lSpeechWords);
            }
            if (ContainOneOfPhonetics(lSpeechWords, "dayaftertomorrow"))
                return DateTime.Today.AddDays(2F).ToString(DATE_FORMAT);
            else if (ContainOneOfPhonetics(lSpeechWords, "weekdays") || ContainOneOfPhonetics(lSpeechWords, "the"))
            {
                // Try to build a date - Find day / number, month,
                // If number or month is absent - check if "next" is present -> GetNextDay
                // Find a day number
                foreach (string lWord in lSpeechWords)
                {
                    if (int.TryParse(lWord, out lDayNum))
                        break;
                }
                // If the day number is valid and a month is choose, create a date
                if (lDayNum >= 1 && lDayNum <= 31 && ContainOneOfPhonetics(lSpeechWords, "month"))
                {
                    // Find the choosen month - If an error occured, the date extraction is stopped.
                    if ((lMonth = FindMonthNumber(lSpeechWords)) < 0)
                        return "";
                    // If the month number is passed OR if the day is passed and the choosen month is the current month, increment the year.
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
                else if (lDayNum == 0 && ContainOneOfPhonetics(lSpeechWords, "next"))
                    return GetNextDay(DateTime.Today, lSpeechWords);
            }
            return lResult;
        }

        /*
         *  Return the number of the first month found in iSpeech.
         *  If no month are found, negative value is return.
         */
        private int FindMonthNumber(string[] iSpeech)
        {
            string[] lMonthArray = Buddy.Resources.GetPhoneticStrings("month");

            foreach (string lWord in iSpeech)
            {
                for (int lIndex = 0; lIndex < lMonthArray.Length; lIndex++)
                {
                    if (lMonthArray[lIndex] == lWord)
                        return lIndex;
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

        /*
         *  Return true if, a word in the phonetics string list represent by iKey (Dictionnary),
         *  is present in the array of string iWords. 
         */
        private bool ContainOneOfPhonetics(string[] iWords, string iKey)
        {
            string[] lKeyWords = Buddy.Resources.GetPhoneticStrings(iKey);

            for (int lIndex = 0; lIndex < iWords.Length; lIndex++)
            {
                foreach (string lWord in lKeyWords)
                {
                    Debug.Log(DC_RED + lWord + DC_END);
                    if (iWords[lIndex].ToLower() == lWord.ToLower())
                        return true;
                }
            }
            return false;
        }

    }
}
