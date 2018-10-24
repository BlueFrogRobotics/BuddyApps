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

        private enum HourModify
        {
            HOUR,
            MINUTE,
            SECOND,
        }

        private const int TRY_NUMBER = 2;

        private int mHour;
        private int mMinute;
        private int mSecond;

        private bool mVocal;
        private int mListen;
        private bool mUi;
        private SpeechInputParameters mVoconParam;
        private TText mHourText;
        private TButton mSwitch;
        private TimeSpan mCarousselHour;
        private HourModify mHourModify;

        // TMP
        public void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mVocal = false;
            mListen = 0;
            mUi = false;
            mHour = -1;
            mMinute = -1;
            mSecond = -1;
            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            // Setting of the grammar for STT
            mVoconParam = new SpeechInputParameters();
            mVoconParam.Grammars = new string[] { "reminder" };
            // Setting of the callback
            Buddy.Vocal.OnEndListening.Add(VoconGetHourResult);
        }

        /*
         *  Test if the hour has been correctly set
         */
        private bool HourIsDefault()
        {
            if (mHour < 0 || mMinute < 0 || mSecond < 0)
                return true;
            return false;
        }

        /*
         *   The function set hour variables, with the day moment parameter (AM/PM)
         */
        private bool HourSet(int iH, int iM, int iS, DayMoment iDayMoment)
        {
            // TODO Convert with iDayMoment
            if (iH >= 0 && iH <= 24)
                mHour = iH;
            if (iM >= 0 && iM <= 59)
                mMinute = iM;
            if (iS >= 0 && iS <= 59)
                mSecond = iS;
            // Convert to AM
            if (iDayMoment == DayMoment.AM && mHour >= 12)
                mHour -= 12;
            // Convert to PM
            else if (iDayMoment == DayMoment.PM && mHour <= 12)
                mHour += 12;
            return true;
        }

        private void VoconGetHourResult(SpeechInput iSpeechInput)
        {
            DebugColor("Hour SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Hour SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                ExtractHourFromSpeech(iSpeechInput.Utterance);
            if (!HourIsDefault())
            {
                TimeSpan lTs = new TimeSpan(mHour, mMinute, mSecond);
                ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                DebugColor("HOUR IS: " + ReminderData.Instance.ReminderDate.ToShortTimeString(), "green");
            }
            mListen++;
            mVocal = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If Hour is correctly set - go to next step
            if (!HourIsDefault())
            {
                ReminderData.Instance.AppState++;
                Trigger("GetMessageState");
            }
            if (mVocal)
                return;
            if (mListen < TRY_NUMBER && HourIsDefault())
            {
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("whours"), mVoconParam.Grammars);
                mVocal = true;
            }
            else if (mListen >= TRY_NUMBER)
            {
                if (!mUi && HourIsDefault())
                {
                    //The last listenning
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), mVoconParam.Grammars);
                    mVocal = true;
                    if (!Buddy.Vocal.IsSpeaking)
                        DisplayHourEntry();
                }
                //else if (!Buddy.Vocal.IsBusy && HourIsDefault())
                //    QuitReminder();
            }
        }

        private void QuitReminder()
        {
            Debug.Log("------- QUIT -------");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.SayKey("bye");
            if (!Buddy.Vocal.IsBusy)
            {
                Buddy.Vocal.OnEndListening.Remove(VoconGetHourResult);
                Buddy.Vocal.Stop();
                QuitApp();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.OnEndListening.Remove(VoconGetHourResult);
            Buddy.Vocal.Stop();
        }

        /* ---- PARSING ----
         * 
         * <time> : [à | pour] ((<numbers#nb> heure [(du matin | du soir | de l’après-midi) | <numbers#nb> | et demi | et quart | moins le quart ]) | midi | minuit); 
         * 
         * [TODO] <intime> : dans <numbers#nb> ( ( heure [<numbers#nb>] [minute] ) | ( minute [<numbers#nb>] [seconde] ) | seconde )
         * 
         *  OPTIONNEL: a ou pour XX ou midi ou minuit ou maintenant
         *      
         *  XX: <numbers> heure [ (du matin | du soir | de l’après-midi) | <numbers#nb> | et demi | et quart | moins le quart]
         *  
         *  
         *  a XX
         *  pour XX
         *  midi
         *  minuit
         *  maintenant
         *  
         *  8 heure 10 / 10 past 8 => 8h10
         *  8 heure moins 10 / 10 to 8 => 7h50
         *  
         */

        private bool ExtractHourFromSpeech(string iSpeech)
        {
            TimeSpan lTs;
            DayMoment lDayMoment = DayMoment.UNKNOW;
            string[] lSpeechWords = iSpeech.Split(' ');
            List<int> lNumbers;

            // Check if the user asked AM or PM
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
                    return HourSet(0, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "noon"))
                    return HourSet(12, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "now"))
                    return HourSet(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, lDayMoment);
            }
            // One numbers in the speech
            else if (lNumbers.Count == 1)
            {
                if (ContainsOneOf(iSpeech, "oclock"))
                    return HourSet(lNumbers[0], 0, 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "half"))
                    return HourSet(lNumbers[0], 30, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarterto"))
                    return HourSet((lNumbers[0] - 1), 45, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarter"))
                    return HourSet(lNumbers[0], 15, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "intime"))
                {
                    lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    if (ContainsOneOf(iSpeech, "hour"))
                    {
                        lTs = new TimeSpan(DateTime.Now.Hour + lNumbers[0], DateTime.Now.Minute, DateTime.Now.Second);
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                        return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                    }
                    else if (ContainsOneOf(iSpeech, "minute"))
                    {
                        lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + lNumbers[0], DateTime.Now.Second);
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                        return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                    }
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
                    return HourSet(lNumbers[1], lNumbers[0], 0, lDayMoment);
                // Ex: 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
                else if (ContainsOneOf(iSpeech, "to"))
                    return HourSet(lNumbers[1] - 1, 60 - lNumbers[0], 0, lDayMoment);
                // Ex: in 2 hours and 10 minutes
                else if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour") && ContainsOneOf(iSpeech, "minute"))
                {
                    lTs = new TimeSpan(DateTime.Now.Hour + lNumbers[0], DateTime.Now.Minute + lNumbers[1], DateTime.Now.Second);
                    ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                    return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                }
            }
            return false;
        }

        // ----- UI -----

        private void DisplayHourEntry()
        {
            mUi = true;
            //Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setuptime"));
            // Create the top left button
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() =>
            {
                // For now we restart to zero when user go back to date choice
                if (!Buddy.Vocal.IsSpeaking)
                {
                    ReminderData.Instance.AppState--;
                    Trigger("DateChoiceState");
                }
            });
            // TMP - waiting for caroussel and dot list
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            // Bug is fix - wait for push
            // lSteps.SetLabel("Steps");
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Init hour to now
                mCarousselHour = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => IncrementHour(1));

                // Text to display choosen date
                string lHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
                mHourText = iOnBuild.CreateWidget<TText>();
                mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lHour);

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => IncrementHour(-1));

                // Switch button - (hour/minute/second)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("hour"));
                mSwitch.OnClick.Add(() => UpdateModifOnHour());
            },
            () =>
            {
                ReminderData.Instance.AppState--;
                Trigger("DateChoiceState");
            },
            Buddy.Resources.GetString("cancel"),
            () =>
            {
                ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + mCarousselHour;
                DebugColor(ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
                ReminderData.Instance.AppState++;
                Trigger("GetMessageState");
            },
            Buddy.Resources.GetString("next"));
        }

        private void IncrementHour(int iIncrement)
        {
            int lSecondInc = -10;
            if (iIncrement > 0)
                lSecondInc = 10;
            if (mHourModify == HourModify.HOUR)
                mCarousselHour = mCarousselHour.Add(new TimeSpan(iIncrement, 0, 0));
            else if (mHourModify == HourModify.MINUTE)
                mCarousselHour = mCarousselHour.Add(new TimeSpan(0, iIncrement, 0));
            else if (mHourModify == HourModify.SECOND)
                mCarousselHour = mCarousselHour.Add(new TimeSpan(0, 0, lSecondInc));
            string lUpdateHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
            mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lUpdateHour);
        }

        private void UpdateModifOnHour()
        {
            if (mHourModify >= HourModify.SECOND)
                mHourModify = HourModify.HOUR;
            else
                mHourModify++;
            if (mHourModify == HourModify.HOUR)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("hour"));
            else if (mHourModify == HourModify.MINUTE)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
            else if (mHourModify == HourModify.SECOND)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("second"));
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
         *  TMP - waiting for bug fix in utils - After that use Utils.ContainsOneOf
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