using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public class HourChoiceState : AStateMachineBehaviour
    {
        private const int DEFAULT_TIME = 30;
        private const int TRY_NUMBER = 1;
        private const string HOUR_FORMAT = "H:mm:ss";

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
            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            // Setting of the grammar for STT
            mGrammar = new SpeechInputParameters();
            mGrammar.Grammars = new string[] { "reminder" };
            // Setting of the callback
            Buddy.Vocal.OnEndListening.Add((iSpeechInput) => { VoconGetHourResult(iSpeechInput); });
        }

        private void VoconGetHourResult(SpeechInput iSpeechInput)
        {
            DebugColor("SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                ReminderData.Instance.HourChoice = ExtractHourFromSpeech(iSpeechInput.Utterance);
            DebugColor("HOUR IS: " + ReminderData.Instance.HourChoice, "green");
            mListen++;
            mVocal = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!string.IsNullOrEmpty(ReminderData.Instance.HourChoice))
                ;
            if (mVocal)
                return;
            if (mListen < TRY_NUMBER && string.IsNullOrEmpty(ReminderData.Instance.HourChoice))
            {
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("whours"), mGrammar.Grammars);
                mVocal = true;
            }
            else if (mListen >= TRY_NUMBER)
            {
                if (!mHeaderTitle && string.IsNullOrEmpty(ReminderData.Instance.HourChoice))
                {
                    //The last listenning
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), mGrammar.Grammars);
                    mVocal = true;
                    mHeaderTitle = true;
                    DisplayHourEntry();
                }
                else if (!Buddy.Vocal.IsBusy && string.IsNullOrEmpty(ReminderData.Instance.HourChoice))
                {
                    Debug.Log("------- QUIT -------");
                    Buddy.GUI.Header.HideTitle();
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();
                    Buddy.Vocal.SayKey("bye");
                    if (!Buddy.Vocal.IsBusy)
                    {
                        Buddy.Vocal.Stop();
                        QuitApp();
                    }
                }
            }
        }


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
                ReminderData.Instance.HourChoice = DateTime.Today.AddSeconds(DEFAULT_TIME).TimeOfDay.ToString(HOUR_FORMAT);
                Trigger("HourChoiceState");
            },
            "Next");
        }

        /*
         * <time> : [à | pour] ((<numbers#nb> heure [(du matin | du soir | de l’après-midi) | <numbers#nb> | et demi | et quart | moins le quart ]) | midi | minuit); 
         * 
         * [TODO] <intime> : ??
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
         */
        private string ExtractHourFromSpeech(string iSpeech)
        {
            string[] lWord = iSpeech.Split(' ');
            if (lWord.Length == 1)
            {
                if (ContainsOneOf(iSpeech, "midnight"))
                    return "00:00";
                else if (ContainsOneOf(iSpeech, "noon"))
                    return "12:00";
                else if (ContainsOneOf(iSpeech, "now"))
                    return DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            }
            // Transform the hour to am
            else if (ContainsOneOf(iSpeech, "morning"))
                ;
            // Transform the hour to pm
            else if (ContainsOneOf(iSpeech, "evening") || ContainsOneOf(iSpeech, "afternoon"))
                ;
            // Just parse "<numbers> heure" and set minutes to 30
            else if (ContainsOneOf(iSpeech, "half"))
                ;
            // Just parse "<numbers> heure" and set minutes to 15
            else if (ContainsOneOf(iSpeech, "quarter"))
                ;
            // Just parse "<numbers> heure" and substrart 15 minutes
            else if (ContainsOneOf(iSpeech, "quarterto"))
                ;
            // Handling of "[a | pour] <numbers> heure [ <numbers> ]"
            else
                return ExtractHour(iSpeech);
            return null;
        }

        /*
         *  Extract hour from a string, with "xxhxx" format.
        */
        private string ExtractHour(string iSpeech)
        {
            string lTime = "";
            string[] lWord = iSpeech.Split(' ');

            for (int i = 0; i < lWord.Length; i++)
            {
                if (CheckString(lWord[i], "h"))
                {
                    string[] lNum = lWord[i].Split('h');

                    if (lNum[0].Length == 1)
                        lTime += "0";
                    lTime += lNum[0] + ":";
                    if (lNum[1] != "")
                    {
                        if (lNum[1].Length == 1)
                            lTime += "0";
                        lTime += lNum[1];
                    }
                    else
                        lTime += "00";
                }
                else
                    lTime = TimeSlot(lWord[i]);
                if (lTime != "")
                    return lTime;
            }
            return null;
        }

        private static bool CheckString(string str, string c)
        {
            return str.IndexOf(c, StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        private string TimeSlot(string iWord)
        {
            string lTime = "";
            if (ContainsOneOf(iWord, "midnight"))
                lTime = "00:00";
            else if (ContainsOneOf(iWord, "noon"))
                lTime = "12:00";
            else if (ContainsOneOf(iWord, "now"))
                lTime = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
            return lTime;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.Stop();
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
