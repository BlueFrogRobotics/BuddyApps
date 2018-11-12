using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class DateChoiceState : AStateMachineBehaviour
    {
        // TMP - Waiting for caroussel
        private enum DateModify
        {
            DAY,
            MONTH,
            YEAR,
        }

        private const int TRY_NUMBER = 3;
        private const float QUIT_TIMEOUT = 20;
        private const float TITLE_TIMER = 2.500F;
        private const int JANUARY = 1;
        private const int DECEMBER = 12;

        private bool mQuit;
        private int mListen;
        private float mTimer;

        // TMP - Waiting for caroussel
        private DateTime mCarousselDate;
        private TText mDateText;
        private TButton mSwitch;
        private DateModify mDateModify;

        // TMP - Debug function - Add to shared ?
        private void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        /*
         *  This function wait for iTitleLifeTime seconds
         *  and then Hide the header title.
         */
        public IEnumerator TitleLifeTime(float iTitleLifeTime)
        {
            yield return new WaitForSeconds(iTitleLifeTime);
            Buddy.GUI.Header.HideTitle();
        }

        public IEnumerator QuittingTimeout()
        {
            mTimer = 0;
            while (mTimer < QUIT_TIMEOUT)
            {
                yield return null;
                mTimer += Time.deltaTime;
            }
            QuitReminder();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mDateModify = DateModify.DAY;
            mListen = 0;
            mQuit = false;
            mTimer = -1;

            // For now, when we go back to that state the date is reset
            ReminderData.Instance.AppState = 0;
            ReminderData.Instance.ReminderDate = new DateTime(0001, 01, 01);
            mCarousselDate = new DateTime(0001, 01, 01);

            // Header setting
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Callback & Grammar setting & First call to Vocon
            Buddy.Vocal.OnEndListening.Add(VoconGetDateResult);
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("when"), new string[] { "reminder", "common" });
        }

        private void VoconGetDateResult(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted || mQuit)
                return;
            mListen++;
            DebugColor("Date SPEECH" + mListen + ".ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Date SPEECH" + mListen + ".Utterance: " + iSpeechInput.Utterance, "blue");

            // Launch Extraction date
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                ReminderData.Instance.ReminderDate = ExtractDateFromSpeech(iSpeechInput.Utterance);

            // Extraction date success - Show the heard date and go to next state
            if (!DateIsDefault(ReminderData.Instance.ReminderDate))
            {
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderData.Instance.ReminderDate.ToShortDateString());
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState();
                return;
            }

            // Extraction date failed - Relaunch listenning until we make less than 2 listenning
            if (mListen < TRY_NUMBER || mTimer >= 0)
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("when"), new string[] { "reminder", "common" });
            // Listenning count is reached - So display UI & launch the last listenning
            else if (!Buddy.GUI.Toaster.IsBusy)
                DisplayDateEntry();
            // Misunderstood & User didn't click on validate - We request to quit
            else
                QuitReminder();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopCoroutine(QuittingTimeout());
            Buddy.Vocal.OnEndListening.Remove(VoconGetDateResult);
            Buddy.Vocal.Stop();
        }

        private void QuitReminder()
        {
            mQuit = true;
            Buddy.Vocal.SayKey("bye");
            DebugColor("QUITTING", "red");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.StopListening();
            Buddy.Vocal.OnEndListening.Remove(VoconGetDateResult);
            StopAllCoroutines();
            QuitApp();
        }

        /*
         *  Return true if iDate is equal to the default date time.
         *  The default date time is set to 0001, 01, 01 in StateEnter
         */
        private bool DateIsDefault(DateTime iDate)
        {
            if (DateTime.Compare(iDate, new DateTime(0001, 01, 01)) == 0)
                return true;
            return false;
        }


        /*
         *  Displaying UI to choose Date
         *  And launch the last listenning
         */
        private void DisplayDateEntry()
        {
            DebugColor("Display DATE TOASTER", "red");

            StartCoroutine(QuittingTimeout());

            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), new string[] { "reminder", "common" });

            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setupdate"));

            //  Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                //  Starting point is the today's date
                mCarousselDate = DateTime.Today.Date;

                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => { IncrementDate(1); mTimer = 0; });

                // Creating of a text to display the choosen date in UI. (save to update it at each click)
                mDateText = iOnBuild.CreateWidget<TText>();
                mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + mCarousselDate.ToShortDateString());

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => { IncrementDate(-1); mTimer = 0; });

                // Switch button - (day/month/year)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_agenda_check"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("day"));
                mSwitch.OnClick.Add(() => { UpdateTargetIncrement(); mTimer = 0; });
            },
            // Cancel date
            () => { /* Back to recipient when available */}, Buddy.Resources.GetString("cancel"),
            // Validate date
            () => GoToNextState(), Buddy.Resources.GetString("next"));
        }

        private void GoToNextState()
        {
            if (!Buddy.Vocal.IsSpeaking)
            {
                if (Buddy.GUI.Toaster.IsBusy)
                {
                    if (DateIsDefault(ReminderData.Instance.ReminderDate))
                        ReminderData.Instance.ReminderDate = mCarousselDate;
                    Buddy.GUI.Header.HideTitle();
                }
                DebugColor("DATE IS: " + ReminderData.Instance.ReminderDate.ToShortDateString(), "green");
                ReminderData.Instance.AppState++;
                Trigger("HourChoiceState");
            }
        }

        /*
         *  This function increment each part of the date ReminderDate, using iIncrement.
         */
        private void IncrementDate(int iIncrement)
        {
            if (mDateModify == DateModify.DAY)
                mCarousselDate = mCarousselDate.AddDays(iIncrement);
            else if (mDateModify == DateModify.MONTH)
                mCarousselDate = mCarousselDate.AddMonths(iIncrement);
            else if (mDateModify == DateModify.YEAR)
                mCarousselDate = mCarousselDate.AddYears(iIncrement);
            mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + mCarousselDate.ToShortDateString());

        }

        /*
         *  This function switch between day/month/year as a target to the modification on date.
         *  It also update target text in UI.
         */
        private void UpdateTargetIncrement()
        {
            if (mDateModify >= DateModify.YEAR)
                mDateModify = DateModify.DAY;
            else
                mDateModify++;
            if (mDateModify == DateModify.DAY)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("day"));
            else if (mDateModify == DateModify.MONTH)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("month"));
            else if (mDateModify == DateModify.YEAR)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("year"));
        }

        /*  ---- PARSING FUNCTION ---- 
         *  
         *  This function find the way the date is pronounce
         *  and create date according to that.
         */
        private DateTime ExtractDateFromSpeech(string iSpeech)
        {
            string[] lWords = iSpeech.Split(' ');

            if (ContainsOneOf(iSpeech, "cancel"))
                QuitReminder();
            if (ContainsOneOf(iSpeech, "today") && lWords.Length == 1)
                return DateTime.Today.Date;
            else if (ContainsOneOf(iSpeech, "tomorrow") && lWords.Length == 1)
                return DateTime.Today.AddDays(1F).Date;
            else if (ContainsOneOf(iSpeech, "weekdays") && lWords.Length == 1)
                return GetNextDay(DateTime.Today, iSpeech.Split(' '));
            else if (ContainsOneOf(iSpeech, "dayaftertomorrow"))
                return DateTime.Today.AddDays(2F).Date;
            else if (ContainsOneOf(iSpeech, "weekdays") || ContainsOneOf(iSpeech, "the") || ContainsOneOf(iSpeech, "allmonth"))
                return TryToBuildDate(iSpeech);
            else if (ContainsOneOf(iSpeech, "intime"))
            {
                UInt16 lNb = 0;
                // If the speech contain less than 3 word, the order is inconsistent
                if (lWords.Length < 3)
                    new DateTime(0001, 01, 01);
                for (int lIndex = 0; lIndex < lWords.Length; ++lIndex)
                {
                    if (lWords[lIndex].ToLower() == Buddy.Resources.GetString("intime") && lIndex + 2 < lWords.Length)
                    {
                        // Check if is't necessary to test the presence of the word "day" / "month" / "year", maybe the grammar of vocon is enough
                        if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "day"))
                            return DateTime.Today.AddDays(lNb).Date;
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "month"))
                            return DateTime.Today.AddMonths(lNb).Date;
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "year"))
                            return DateTime.Today.AddYears(lNb).Date;
                    }
                }
            }
            return new DateTime(0001, 01, 01);
        }

        /*
         *  This function try to buid an entire date with the speech string.
         */
        private DateTime TryToBuildDate(string iSpeech)
        {
            int lMonth = DateTime.Today.Month;
            int lYear = DateTime.Today.Year;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            lNumbers = GetNumbersInString(iSpeech, 2);

            /*
             *  If no number is choose the speech is:
             *  "the next"              -   error to fix in grammar
             *  "<weekdays> next"       -   error to fix in grammar
             *  "the next <weekdays>"   -   OK
             *  "next <weekdays>"       -   OK
             */
            if (lNumbers == null)
            {
                if (ContainsOneOf(iSpeech, "next"))
                    return GetNextDay(DateTime.Today, lSpeechWords);
            }
            // If the first number is a valid day number - and a month is specidied
            else if (lNumbers[0] >= 1 && lNumbers[0] <= 31 && ContainsOneOf(iSpeech, "allmonth"))
            {
                // Find the choosen month - If an error occured, the date extraction is stopped.
                if ((lMonth = FindMonthNumber(lSpeechWords)) < 0)
                    return new DateTime(0001, 01, 01);
                // If a second number is specified, it's the year
                if (lNumbers.Count > 1 && lNumbers[1] > 0 && lNumbers[1] < 9999)
                    lYear = lNumbers[1];
                // If valid year is not found AND If the month is passed, OR If the month is the current month AND the day is passed, Set to the next year
                else if (lMonth < DateTime.Today.Month || (lMonth == DateTime.Today.Month && lNumbers[0] < DateTime.Today.Day))
                    lYear++;
                return new DateTime(lYear, lMonth, lNumbers[0]);
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
                return new DateTime(lYear, lMonth, lNumbers[0]);
            }
            return new DateTime(0001, 01, 01);
        }


        // --- Generic Function for Exctrat info in Speech ---

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

        private DateTime GetNextDay(DateTime iStartDay, string[] iSpeech)
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

        private List<int> GetNumbersInString(string iText, UInt16 iHowMany)
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
        private bool ContainsOneOf(string iSpeech, string iKey)
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
    }
}
