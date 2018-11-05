using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class HourChoiceState : AStateMachineBehaviour
    {
        private enum DayMoment
        {
            UNKNOW,
            AM,
            PM,
        }

        // TMP - Waiting for caroussel
        private enum HourModify
        {
            MINUTE,
            HOUR,
        }

        private const int TRY_NUMBER = 2;
        private const float TITLE_TIMER = 2.500F;

        private bool mQuit;
        private int mListen;

        // TMP - Waiting for caroussel
        private TimeSpan mCarousselHour;
        private TText mHourText;
        private TButton mSwitch;
        private HourModify mHourModify;

        // TMP
        public void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        /*
         *   This function wait for iTitleLifeTime seconds
         *   and then Hide the header title.
         */
        public IEnumerator TitleLifeTime(float iTitleLifeTime)
        {
            yield return new WaitForSeconds(iTitleLifeTime);
            Buddy.GUI.Header.HideTitle();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mHourModify = HourModify.MINUTE;
            mListen = 0;

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Callback & Grammar setting & First call to Vocon
            Buddy.Vocal.OnEndListening.Add(VoconGetHourResult);
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("whours"), new string[] { "reminder", "common" });
        }

        /*
         *   The function set hour variables, with the day moment parameter (AM/PM)
         *   If an error occured, (Time unconsistent), false is returned.
         */
        private bool HourSet(int iHour, int iMinute, DayMoment iDayMoment)
        {
            if (!(iHour >= 0 && iHour <= 24))
                return false;
            if (!(iMinute >= 0 && iMinute <= 59))
                return false;
            // Convert to AM
            if (iDayMoment == DayMoment.AM && iHour >= 12)
                iHour -= 12;
            // Convert to PM
            else if (iDayMoment == DayMoment.PM && iHour <= 12)
                iHour += 12;
            TimeSpan lTs = new TimeSpan(iHour, iMinute, 0);
            ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
            return true;
        }

        private void VoconGetHourResult(SpeechInput iSpeechInput)
        {
            bool lHourIsSet = false;

            if (iSpeechInput.IsInterrupted || mQuit)
                return;

            mListen++;
            DebugColor("Hour SPEECH" + mListen + ".ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Hour SPEECH" + mListen + ".Utterance: " + iSpeechInput.Utterance, "blue");

            // Launch hour extraction
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                lHourIsSet = ExtractHourFromSpeech(iSpeechInput.Utterance);
            // Hour extraction success - Show the heard hour and go to next state
            if (lHourIsSet)
            {
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderData.Instance.ReminderDate.ToShortTimeString());
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState(lHourIsSet);
                return;
            }

            // Hour extraction failed - Relaunch listenning until we make less than 2 listenning
            if (mListen < TRY_NUMBER)
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("whours"), new string[] { "reminder", "common" });
            // Listenning count is reached - So display UI & launch the last listenning
            else if (!Buddy.GUI.Toaster.IsBusy)
                DisplayHourEntry();
            // Misunderstood & User didn't click on validate - We request to quit
            else
                QuitReminder();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.OnEndListening.Remove(VoconGetHourResult);
            Buddy.Vocal.Stop();
        }

        private void QuitReminder()
        {
            mQuit = true;
            Debug.Log("------- QUIT -------");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.StopListening();
            Buddy.Vocal.SayKey("bye");
            Buddy.Vocal.OnEndListening.Remove(VoconGetHourResult);
            QuitApp();
        }

        /* ---- PARSING ----
         * 
         *  This function find the way the hour is pronounce, create an hour according to that,
         *  and add it to the reminder's date.
         */

        private bool ExtractHourFromSpeech(string iSpeech)
        {
            TimeSpan lTs;
            DayMoment lDayMoment = DayMoment.UNKNOW;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            // Check if the user asked AM or PM - Check if it's useful or not
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Try to get two numbers in speech TODO check if necessary further in the code
            lNumbers = GetNumbersInString(iSpeech, 2);
            // No numbers in the speech
            if (lNumbers == null)
            {
                if (ContainsOneOf(iSpeech, "midnight"))
                    return HourSet(0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "noon"))
                    return HourSet(12, 0, lDayMoment);
            }
            // One numbers in the speech
            else if (lNumbers.Count == 1)
            {
                if (ContainsOneOf(iSpeech, "cancel"))
                {
                    ReminderData.Instance.AppState--;
                    Trigger("DateChoiceState");
                }
                if (ContainsOneOf(iSpeech, "oclock"))
                    return HourSet(lNumbers[0], 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "half"))
                    return HourSet(lNumbers[0], 30, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarterto"))
                    return HourSet((lNumbers[0] - 1), 45, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarter"))
                    return HourSet(lNumbers[0], 15, lDayMoment);
                else if (ContainsOneOf(iSpeech, "intime"))
                {
                    lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    if (ContainsOneOf(iSpeech, "hour"))
                        return HourSet(DateTime.Now.Hour + lNumbers[0], DateTime.Now.Minute, DayMoment.UNKNOW);
                    else if (ContainsOneOf(iSpeech, "minute"))
                        return HourSet(DateTime.Now.Hour, DateTime.Now.Minute + lNumbers[0], DayMoment.UNKNOW);
                    else
                        DebugColor("add hours or minutes", "red");
                    return false;
                }
            }
            // Two numbers in the speech
            else if (lNumbers.Count >= 2)
            {
                // Ex: 10 past 8 => 8h10 - So use lSeconNum to hours and lMinuteNum to minutes
                if (ContainsOneOf(iSpeech, "pasthour"))
                    return HourSet(lNumbers[1], lNumbers[0], lDayMoment);
                // Ex: 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
                else if (ContainsOneOf(iSpeech, "to"))
                    return HourSet(lNumbers[1] - 1, 60 - lNumbers[0], lDayMoment);
                // Ex: in 2 hours and 10 minutes
                else if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour") && ContainsOneOf(iSpeech, "minute"))
                    return HourSet(DateTime.Now.Hour + lNumbers[0], DateTime.Now.Minute + lNumbers[1], DayMoment.UNKNOW);
            }
            return false;
        }

        // ----- UI -----

        private void DisplayHourEntry()
        {
            DebugColor("Display HOUR TOASTER", "red");
            //  The last listenning
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), new string[] { "reminder", "common" });

            //Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setuptime"));

            // Create the top left button, to go back
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() => GoToPreviousState());

            // Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Starting point is the time from now
                mCarousselHour = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => IncrementHour(1));

                // Creating of a text box to display the choosen hour in UI. (save to update it at each click)
                string lHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":0";
                mHourText = iOnBuild.CreateWidget<TText>();
                mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lHour);

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => IncrementHour(-1));

                // Switch button - (hour/minute)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
                mSwitch.OnClick.Add(() => UpdateTargetIncrement());
            },
            () => GoToPreviousState(), Buddy.Resources.GetString("cancel"),
            () => GoToNextState(false), Buddy.Resources.GetString("next"));
        }

        private void GoToPreviousState()
        {
            if (!Buddy.Vocal.IsSpeaking)
            {
                if (Buddy.GUI.Toaster.IsBusy)
                    Buddy.GUI.Header.HideTitle();
                ReminderData.Instance.AppState--;
                Trigger("DateChoiceState");
            }
        }

        private void GoToNextState(bool iHourIsSet)
        {
            if (!Buddy.Vocal.IsSpeaking)
            {
                // If the UI is displayed, we save caroussel hour and Hide the title
                if (Buddy.GUI.Toaster.IsBusy)
                {
                    if (!iHourIsSet)
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + mCarousselHour;
                    Buddy.GUI.Header.HideTitle();
                }
                DebugColor("HOUR IS: " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
                ReminderData.Instance.AppState++;
                Trigger("GetMessageState");
            }
        }

        /*
         *  This function increment each hour/minute of the ReminderDate, using iIncrement.
         */
        private void IncrementHour(int iIncrement)
        {
            if (mHourModify == HourModify.HOUR)
                mCarousselHour = mCarousselHour.Add(new TimeSpan(iIncrement, 0, 0));
            else
                mCarousselHour = mCarousselHour.Add(new TimeSpan(0, iIncrement, 0));
            mCarousselHour = mCarousselHour.Subtract(new TimeSpan(0, 0, mCarousselHour.Seconds));
            string lUpdateHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
            mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lUpdateHour);
        }

        /*
         *  This function switch between hour/minute as a target to the modification on date.
         *  It also update target text in UI.
         */
        private void UpdateTargetIncrement()
        {
            if (mHourModify >= HourModify.HOUR)
                mHourModify = HourModify.MINUTE;
            else
                mHourModify++;
            if (mHourModify == HourModify.HOUR)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("hour"));
            else
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
        }

        // --- Generic Function for Exctrat info in Speech ---

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

        /*
         *  TMP - Will be in shared
         */
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