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

        private const int DEFAULT_TIME = 30;
        private const int TRY_NUMBER = 20;
        private const string HOUR_FORMAT = "H:mm:ss";

        private int mHour;
        private int mMinute;
        private int mSecond;

        private bool mVocal;
        private int mListen;
        private bool mHeaderTitle;
        private string mCarousselHour;
        private SpeechInputParameters mGrammar;

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

        private bool HourIsDefault()
        {
            if (mHour < 0 || mMinute < 0 || mSecond < 0)
                return true;
            return false;
        }

        private bool HourSet(int iH, int iM, int iS, DayMoment iDayMoment)
        {
            // TODO Convert with iDayMoment
            if (iH >= 0 && iH <= 24)
                mHour = iH;
            if (iM >= 0 && iM <= 59)
                mMinute = iM;
            if (iS >= 0 && iS <= 59)
                mSecond = iS;
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
            if (!HourIsDefault())
                ;
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
        private bool ExtractHourFromSpeech2(string iSpeech)
        {
            Int16 lFirstNum = -1;
            Int16 lSecondNum = -1;
            DayMoment lDayMoment = DayMoment.NEAREST;
            string[] lSpeechWords = iSpeech.Split(' ');

            // Check if the user asked AM or PM
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Get a number in speech
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
                // 10 past 8 => 8h10 - So use lSeconNum to hours and lMinuteNum to minutes
                if (ContainsOneOf(iSpeech, "hour"))
                    HourSet(lSecondNum, lFirstNum, 0, lDayMoment);
                // 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
                else if (ContainsOneOf(iSpeech, "to"))
                   HourSet(lSecondNum - 1, 60 - lFirstNum, 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "intime"))
                {
                    TimeSpan lTs = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                    ReminderData.Instance.ReminderDate.AddMinutes(mMinute);
                    ReminderData.Instance.ReminderDate.AddHours(mHour);
                    return true;
                }
            }
            // One numbers in the speech
            else if (lFirstNum > 0 && lSecondNum < 0)
            {
                // 8 o'clock => 8h00
                if (ContainsOneOf(iSpeech, "oclock") && lSecondNum < 0)
                    return HourSet(lFirstNum, 0, 0, lDayMoment);
                if (ContainsOneOf(iSpeech, "half"))
                    return HourSet(lFirstNum, 30, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "quarter"))
                    return HourSet(lFirstNum, 15, 0, lDayMoment);
            }
            // No numbers in the speech
            else if (lFirstNum < 0 && lSecondNum < 0)
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

        private void ExtractHourFromSpeech(string iSpeech)
        {
            UInt16 lHour = 0;
            string[] lSpeechWords = iSpeech.Split(' ');
            DayMoment lDayMoment = DayMoment.NEAREST;
            if (lSpeechWords.Length == 1)
            {
                if (ContainsOneOf(iSpeech, "midnight"))
                    HourSet(0, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "noon"))
                    HourSet(12, 0, 0, lDayMoment);
                else if (ContainsOneOf(iSpeech, "now"))
                    HourSet(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, lDayMoment);
            }

            // Check if the user asked AM or PM
            if (ContainsOneOf(iSpeech, "morning"))
                lDayMoment = DayMoment.AM;
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                lDayMoment = DayMoment.PM;

            // Just parse "<numbers> heure" and set minutes to 30, "<numbers> heure et demi"
            if (ContainsOneOf(iSpeech, "half"))
            {
                DebugColor("half", "yellow");
                foreach (string lWord in lSpeechWords)
                {
                    if (UInt16.TryParse(lWord, out lHour))
                        break;
                }
                HourSet(lHour, 30, 0, lDayMoment);
            }
            // Just parse "<numbers> heure" and substrart 15 minutes, "<numbers> heure moins le quart"
            else if (ContainsOneOf(iSpeech, "quarterto"))
            {
                DebugColor("quarterto", "yellow");
                foreach (string lWord in lSpeechWords)
                {
                    if (UInt16.TryParse(lWord, out lHour))
                        break;
                }
                HourSet((lHour - 1), 45, 0, lDayMoment);
            }
            // Just parse "<numbers> heure" and set minutes to 15, "<numbers> heure et quart"
            else if (ContainsOneOf(iSpeech, "quarter"))
            {
                DebugColor("quarter", "yellow");
                foreach (string lWord in lSpeechWords)
                {
                    if (UInt16.TryParse(lWord, out lHour))
                        break;
                }
                HourSet(lHour, 15, 0, lDayMoment);
            }
            // Handling <intime>
            else if (ContainsOneOf(iSpeech, "intime"))
                DebugColor("intime", "yellow");
            else if (Buddy.Platform.Language == Language.EN)
                ExtractHourEn(iSpeech, lDayMoment);
        }

        /*
         *  Handling of "[a | pour] <numbers> heure [ [moins] <numbers> ]"
         *  "[to | for] <numbers> (past | to) <numbers>" 
         *  8 heure 10 / 10 past 8 => 8h10
         *  8 heure moins 10 / 10 to 8 => 7h50
         */
        private void ExtractHourEn(string iSpeech, DayMoment iDayMoment)
        {
            Int16 lFirstNum = -1;
            Int16 lSecondNum = -1;
            string[] lSpeechWords = iSpeech.Split(' ');

            // Get a number in speech
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
            if (lFirstNum < 0)
                return;
            DebugColor("First:" + lFirstNum.ToString() + " Second:" + lSecondNum.ToString(), "cyan");
            // 8 o'clock => 8h00
            if (ContainsOneOf(iSpeech, "oclock") && lSecondNum < 0)
                HourSet(lFirstNum, 0, 0, iDayMoment);
            // 10 past 8 => 8h10 - So use lSeconNum to hours and lMinuteNum to minutes
            else if (ContainsOneOf(iSpeech, "hour"))
                HourSet(lSecondNum, lFirstNum, 0, iDayMoment);
            // 10 to 8 => 7h50 - So use lSecondNum to hours and substract to it lFirstNum in minutes
            else if (ContainsOneOf(iSpeech, "to"))
                HourSet(lSecondNum - 1, 60 - lFirstNum, 0, iDayMoment);
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
                    Trigger("DateChoiceState");
            });
            // Create Caroussel Here
            // TMP - waiting for caroussel and dot list
            Buddy.GUI.Footer.CreateOnMiddle<FLabeledButton>().SetLabel("STEP:" + ReminderData.Instance.AppState + "of 3");
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                mCarousselHour = "null";
                TText lTmpText = iOnBuild.CreateWidget<TText>();
                lTmpText.SetLabel("Waiting for Caroussel Toast - Default time: in " + DEFAULT_TIME + "seconds");
                //Set to default for now - Current time + 10 sec
                //mCarousselHour = DateTime.Today.AddSeconds(30F).TimeOfDay.ToString(HOUR_FORMAT);
            },
            () =>
            {
                if (!Buddy.Vocal.IsSpeaking)
                    Trigger("DateChoiceState");
            },
            "Cancel",
            () =>
            {
                TimeSpan lTs = new TimeSpan(DateTime.Today.Hour, DateTime.Today.Minute, DateTime.Today.Second + DEFAULT_TIME);
                ReminderData.Instance.ReminderDate = ReminderData.Instance.ReminderDate.Date + lTs;
                Trigger("HourChoiceState");
            },
            "Next");
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
    }
}