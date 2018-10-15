using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public class HourChoiceState : AStateMachineBehaviour
    {
        private enum DayMoment
        {
            NEAREST,
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
        private bool mHeaderTitle;
        private SpeechInputParameters mGrammar;
        private TText mHourText;
        private TButton mSwitch;
        private TimeSpan mCarousselHour;
        private HourModify mHourModify;

        // TMP
        private void DebugColor(string msg, string color)
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
            mHeaderTitle = false;
            mHour = -1;
            mMinute = -1;
            mSecond = -1;
            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            // Setting of the grammar for STT
            mGrammar = new SpeechInputParameters();
            mGrammar.Grammars = new string[] { "reminder" };
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
            if (iDayMoment == DayMoment.AM)
                ;
            // Convert to PM
            else if (iDayMoment == DayMoment.PM)
                ;
            // No convert - Use the nearest if consistent
            else
                ;
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
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("whours"), mGrammar.Grammars);
                mVocal = true;
            }
            else if (mListen >= TRY_NUMBER)
            {
                if (!mHeaderTitle && HourIsDefault())
                {
                    //The last listenning
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), mGrammar.Grammars);
                    mVocal = true;
                    mHeaderTitle = true;
                    DisplayHourEntry();
                }
                else if (!Buddy.Vocal.IsBusy && HourIsDefault())
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
            Int16 lFirstNum = -1;
            Int16 lSecondNum = -1;
            DayMoment lDayMoment = DayMoment.NEAREST;
            string[] lSpeechWords = iSpeech.Split(' ');

            // Check if the user asked AM or PM
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Get two numbers in speech
            for (int lIndex = 0; lIndex < lSpeechWords.Length; lIndex++)
            {
                // Find first number
                if (Int16.TryParse(lSpeechWords[lIndex], out lFirstNum))
                {
                    // Find second number
                    for (int lI = lIndex + 1; lI < lSpeechWords.Length; lI++)
                    {
                        if (Int16.TryParse(lSpeechWords[lI], out lSecondNum))
                            break;
                    }
                    break;
                }
            }
            // Two numbers in the speech
            if (lFirstNum > 0 && lSecondNum > 0)
            {
                // Ex: 10 past 8 => 8h10 - So use lSeconNum to hours and lMinuteNum to minutes
                if (ContainsOneOf(iSpeech, "pasthour"))
                    return HourSet(lSecondNum, lFirstNum, 0, lDayMoment);
                // Ex: 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
                else if (ContainsOneOf(iSpeech, "to"))
                   return HourSet(lSecondNum - 1, 60 - lFirstNum, 0, lDayMoment);
                // Ex: in 2 hours and 10 minutes
                else if (ContainsOneOf(iSpeech, "intime") && ContainsOneOf(iSpeech, "hour") && ContainsOneOf(iSpeech, "minute"))
                {
                    lTs = new TimeSpan(DateTime.Now.Hour + lFirstNum, DateTime.Now.Minute + lSecondNum, DateTime.Now.Second);
                    ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                    return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                }
            }
            // One numbers in the speech
            else if (lFirstNum > 0 && lSecondNum <= 0)
            {
                if (ContainsOneOf(iSpeech, "oclock"))
                    return HourSet(lFirstNum, 0, 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "half"))
                    return HourSet(lFirstNum, 30, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarterto"))
                    return HourSet((lFirstNum - 1), 45, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarter"))
                    return HourSet(lFirstNum, 15, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "intime"))
                {
                    lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    if (ContainsOneOf(iSpeech, "hour"))
                    {
                    lTs = new TimeSpan(DateTime.Now.Hour + lFirstNum, DateTime.Now.Minute, DateTime.Now.Second);
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                        return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                    }
                    else if (ContainsOneOf(iSpeech, "minute"))
                    {
                        lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + lFirstNum, DateTime.Now.Second);
                        ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                        return HourSet(ReminderData.Instance.ReminderDate.Hour, ReminderData.Instance.ReminderDate.Minute, ReminderData.Instance.ReminderDate.Second, lDayMoment);
                    }
                    else
                        DebugColor("add hours or minutes", "red");
                    return false;
                }
            }
            // No numbers in the speech
            else if (lFirstNum <= 0 && lSecondNum <= 0)
            {
                if (ContainsOneOf(iSpeech, "midnight"))
                    return HourSet(0, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "noon"))
                    return HourSet(12, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "now"))
                    return HourSet(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, lDayMoment);
            }
            return false;
        }

        // ----- UI -----

        private void DisplayHourEntry()
        {
            //Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setuptime"));
            // Create the top left button to switch between count mode and video mode.
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
            // TODO wait for fix, NullRef in SetLabel 
            // lSteps.SetLabel("Steps");
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Init hour to now
                mCarousselHour = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() =>
                {
                    if (mHourModify == HourModify.HOUR)
                        mCarousselHour = mCarousselHour.Add(new TimeSpan(1, 0, 0));
                    else if (mHourModify == HourModify.MINUTE)
                        mCarousselHour = mCarousselHour.Add(new TimeSpan(0, 1, 0));
                    else if (mHourModify == HourModify.SECOND)
                        mCarousselHour = mCarousselHour.Add(new TimeSpan(0, 0, 10));
                    string lUpdateHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
                    mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lUpdateHour);
                });

                // Text to display choosen date
                string lHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
                mHourText = iOnBuild.CreateWidget<TText>();
                mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lHour);

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() =>
                {
                    if (mHourModify == HourModify.HOUR)
                        mCarousselHour = mCarousselHour.Subtract(new TimeSpan(1, 0, 0));
                    else if (mHourModify == HourModify.MINUTE)
                        mCarousselHour = mCarousselHour.Subtract(new TimeSpan(0, 1, 0));
                    else if (mHourModify == HourModify.SECOND)
                        mCarousselHour = mCarousselHour.Subtract(new TimeSpan(0, 0, 10));
                    string lUpdateHour = mCarousselHour.Hours.ToString() + ":" + mCarousselHour.Minutes.ToString() + ":" + mCarousselHour.Seconds.ToString();
                    mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + lUpdateHour);
                });

                // Switch button - (hour/minute/second)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + Buddy.Resources.GetString("hour"));
                mSwitch.OnClick.Add(() =>
                {
                    if (mHourModify >= HourModify.SECOND)
                        mHourModify = HourModify.HOUR;
                    else
                        mHourModify++;
                    if (mHourModify == HourModify.HOUR)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + Buddy.Resources.GetString("hour"));
                    else if (mHourModify == HourModify.MINUTE)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + Buddy.Resources.GetString("minute"));
                    else if (mHourModify == HourModify.SECOND)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + Buddy.Resources.GetString("second"));
                });
            },
            () =>
            {
                ReminderData.Instance.AppState--;
                Trigger("DateChoiceState");
            },
            "Cancel",
            () =>
            {
                ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + mCarousselHour;
                DebugColor(ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
                ReminderData.Instance.AppState++;
                Trigger("GetMessageState");
            },
            "Next");
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