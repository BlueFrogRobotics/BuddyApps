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
        private const float QUIT_TIMEOUT = 20;
        private const float TITLE_TIMER = 2.500F;
        private const int RECOGNITION_SENSIBILITY = 5000;

        private bool mQuit;
        private int mListen;
        private float mTimer;
        private IEnumerator mQuitOnTimeout;
        private SpeechInputParameters mRecognitionParam;

        // Type of Extract hour function
        private delegate bool ExtractHourFromSpeech(string iSpeech);
        // Extraction hour function array. The index is based on the enum: Language
        private ExtractHourFromSpeech[] mExtractHourFromSpeech = new ExtractHourFromSpeech[Enum.GetValues(typeof(Language)).Length];

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

        public IEnumerator QuittingTimeout()
        {
            mTimer = 0;
            while (mTimer < QUIT_TIMEOUT)
            {
                yield return null;
                mTimer += Time.deltaTime;
            }
            DebugColor("TIMEOUT", "red");
            QuitReminder();
        }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mHourModify = HourModify.MINUTE;
            mListen = 0;
            mQuit = false;
            mTimer = -1;

            // Because we need to stop this coroutine independently of others
            mQuitOnTimeout = QuittingTimeout();

            // Filling the extraction function for each language
            mExtractHourFromSpeech[(int)Language.EN] = ExtractHourFromEnglishSpeech;
            mExtractHourFromSpeech[(int)Language.FR] = ExtractHourFromFrenchSpeech;

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Setting of Vocon param
            mRecognitionParam = new SpeechInputParameters();
            mRecognitionParam.RecognitionThreshold = RECOGNITION_SENSIBILITY;
            mRecognitionParam.Grammars = new string[] { "reminder_hour", "common" };

            // If an hour has been already choose, just display hour entry with this date on caroussel
            if (ReminderData.Instance.HourSaved)
            {
                DisplayHourEntry(new TimeSpan(ReminderData.Instance.ReminderDate.Hour,
                                                    ReminderData.Instance.ReminderDate.Minute,
                                                    ReminderData.Instance.ReminderDate.Second));
            }
            // Callback & Grammar setting & First call to Vocon
            else
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("whours")), null, VoconGetHourResult, null, mRecognitionParam);
        }

        /* 
         *   The function set hour variables, with the day moment parameter (AM/PM).
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

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "quit")
                QuitReminder();

            // Launch hour extraction
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                lHourIsSet = mExtractHourFromSpeech[(int)Buddy.Platform.Language](iSpeechInput.Utterance);

            // Hour extraction success - Show the heard hour and go to next state
            if (lHourIsSet)
            {
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderData.Instance.ReminderDate.ToShortTimeString());
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState();
                return;
            }

            // Hour extraction failed - Relaunch listenning until we make less than 2 listenning
            if (mListen < TRY_NUMBER || mTimer >= 0)
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("whours")), null, VoconGetHourResult, null, mRecognitionParam);
            // Listenning count is reached - So display UI & launch the last listenning
            else if (!Buddy.GUI.Toaster.IsBusy)
                DisplayHourEntry(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            // Misunderstood & User didn't click on validate - We request to quit
            else
                QuitReminder();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset the timeout timer, on touch
            if (Input.touchCount > 0)
            {
                mTimer = -1;
                if (Buddy.GUI.Toaster.IsBusy)
                    mTimer = 0;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopCoroutine(mQuitOnTimeout);
            Buddy.Vocal.StopAndClear();
        }

        private void QuitReminder()
        {
            DebugColor("QUITTING HOUR CHOICE", "red");
            mQuit = true;
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopAllCoroutines();
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.SayKey("bye", (iOutput) => { QuitApp(); });
        }

        /* ---- PARSING ----
        * 
        *  This function find the way the hour is pronounce, create an hour according to that,
        *  and add it to the reminder's date.
        */

        private bool ExtractHourFromFrenchSpeech(string iSpeech)
        {
            DayMoment lDayMoment = DayMoment.UNKNOW;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            // Check if the user asked AM or PM
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Try to get two numbers in speech
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
            else if (lNumbers.Count == 1 && lNumbers[0] >= 0)
            {
                if (ContainsOneOf(iSpeech, "intime"))
                {
                    if (ContainsOneOf(iSpeech, "hour"))
                    {
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddHours(lNumbers[0]).TimeOfDay;
                        return true;
                    }
                    else if (ContainsOneOf(iSpeech, "minute"))
                    {
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddMinutes(lNumbers[0]).TimeOfDay;
                        return true;
                    }
                    return false;
                }
                else if (ContainsOneOf(iSpeech, "hour"))
                {
                    if (ContainsOneOf(iSpeech, "half"))
                        return HourSet(lNumbers[0], 30, lDayMoment);
                    else if (ContainsOneOf(iSpeech, "quarterto"))
                        return HourSet((lNumbers[0] - 1), 45, lDayMoment);
                    else if (ContainsOneOf(iSpeech, "quarter"))
                        return HourSet(lNumbers[0], 15, lDayMoment);
                    else
                        return HourSet(lNumbers[0], 0, lDayMoment);
                }
            }
            // Two numbers in the speech
            else if (lNumbers.Count >= 2 && lNumbers[0] >= 0 && lNumbers[1] >= 0)
            {
                // Ex: in 2 hours and 10 minutes
                if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour"))
                {
                    int lMinutesToAdd = (lNumbers[0] * 60) + lNumbers[1];
                    ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddMinutes(lMinutesToAdd).TimeOfDay;
                    return true;
                }
                else if (ContainsOneOf(iSpeech, "hour"))
                {
                    if (ContainsOneOf(iSpeech, "to"))
                        return HourSet(lNumbers[0] - 1, 60 - lNumbers[1], lDayMoment);
                    else
                        return HourSet(lNumbers[0], lNumbers[1], lDayMoment);
                }
            }
            return false;
        }

        private bool ExtractHourFromEnglishSpeech(string iSpeech)
        {
            DayMoment lDayMoment = DayMoment.UNKNOW;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            // Check if the user asked AM or PM - Check if it's useful or not
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Try to get two numbers in speech
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
                    if (ContainsOneOf(iSpeech, "hour"))
                    {
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddHours(lNumbers[0]).TimeOfDay;
                        return true;
                    }
                    else if (ContainsOneOf(iSpeech, "minute"))
                    {
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddMinutes(lNumbers[0]).TimeOfDay;
                        return true;
                    }
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
                {
                    int lMinutesToAdd = (lNumbers[0] * 60) + lNumbers[1];
                    ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + DateTime.Now.AddMinutes(lMinutesToAdd).TimeOfDay;
                    return true;
                }
            }
            return false;
        }

        // ----- UI -----

        private void DisplayHourEntry(TimeSpan iCarousselStartHour)
        {
            // Launch the quit timeout
            StartCoroutine(mQuitOnTimeout);

            // The message is different if we are back to GetMessageState
            if (ReminderData.Instance.HourSaved)
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("whours")), null, VoconGetHourResult, null, mRecognitionParam);
            else
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("srynotunderstand")), null, VoconGetHourResult, null, mRecognitionParam);

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
                //  Set the starting point of the carousel
                mCarousselHour = iCarousselStartHour;

                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => { IncrementHour(1); mListen = 0; });

                // Creating of a text to display the choosen hour in UI.
                mHourText = iOnBuild.CreateWidget<TText>();
                mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + mCarousselHour.ToString(@"hh\:mm"));

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => { IncrementHour(-1); mListen = 0; });

                // Switch button - (hour/minute)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
                mSwitch.OnClick.Add(() => { UpdateTargetIncrement(); mListen = 0; });
            },
            () => GoToPreviousState(), Buddy.Resources.GetString("cancel"),
            () => GoToNextState(true), Buddy.Resources.GetString("next"));
        }

        private void GoToPreviousState()
        {
            if (Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Header.HideTitle();
            ReminderData.Instance.AppState--;
            Trigger("DateChoiceState");
        }

        private void GoToNextState(bool iManualValidation = false)
        {
            // Update the date with caroussel if the function is called from the toaster
            if (iManualValidation)
            {
                ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + mCarousselHour;
                Buddy.GUI.Header.HideTitle();
            }
            DebugColor("HOUR IS: " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
            ReminderData.Instance.AppState++;
            ReminderData.Instance.HourSaved = true;
            Trigger("GetMessageState");
        }

        /*
         *  This function increment each hour/minute of the ReminderDate, using iIncrement.
         *  It also stop vocal.
         */
        private void IncrementHour(int iIncrement)
        {
            Buddy.Vocal.StopListening();
            if (mHourModify == HourModify.HOUR)
                mCarousselHour = mCarousselHour.Add(new TimeSpan(iIncrement, 0, 0));
            else
                mCarousselHour = mCarousselHour.Add(new TimeSpan(0, iIncrement, 0));
            mCarousselHour = mCarousselHour.Subtract(new TimeSpan(0, 0, mCarousselHour.Seconds));
            mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + mCarousselHour.ToString(@"hh\:mm"));
        }

        /*
         *  This function switch between hour/minute as a target to the modification on date.
         *  It also update target text in UI, and stop vocal.
         */
        private void UpdateTargetIncrement()
        {
            Buddy.Vocal.StopListening();
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