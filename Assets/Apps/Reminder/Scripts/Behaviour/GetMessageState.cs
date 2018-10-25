using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class GetMessageState : AStateMachineBehaviour
    {
        /*
         *  TODO:
         *  Afficher heure sur retour dans get msg
         *  Afficher date sur retour dans get hour
         */

        private bool mEndRecord;
        private string mRecordedMessage;
        private SpeechInputParameters mVoconParam;

        // TMP - Wait for time out in freespeech function
        private float mFreeSpeechTimeStamp;
        private float FREESPEECH_TIMER = 15F;


        // TMP - Debug
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
            mEndRecord = false;
            mRecordedMessage = null;

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Setting of vocon grammar
            mVoconParam = new SpeechInputParameters();
            mVoconParam.Grammars = new string[] { "reminder", "common" };

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            mFreeSpeechTimeStamp = Time.time;
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            DebugColor("FreeSpeech Msg SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("FreeSpeech Msg SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            mRecordedMessage = iSpeechInput.Utterance;
            mEndRecord = true;
            DisplayMessageEntry();
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                DebugColor("SORRY DONT UNDERSTAND", "red");
                return;
            }
            else if (ContainsOneOf(iSpeechInput.Utterance, "validate"))
                ValidateMessage();
            else if (ContainsOneOf(iSpeechInput.Utterance, "modify"))
                ModifyMessage();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // FreeSpeech is stopped, if time out is reached
            if (!mEndRecord && ((Time.time - mFreeSpeechTimeStamp) >= FREESPEECH_TIMER))
                Buddy.Vocal.Stop();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DebugColor("STATE EXIT", "red");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.Stop();
        }

        private void DisplayMessageEntry()
        {
            DebugColor("DISPLAY MSG", "blue");
            // Ask validate or modify and reco with vocon
            Buddy.Vocal.Say(Buddy.Resources.GetString("hereisthemsg"));
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("validateormodify"),
                null,
                mVoconParam.Grammars,
                (iOnEndListening) => { VoconResult(iOnEndListening); },
                null);

            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("message"));

            // Create the top left button
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() =>
            {
                // For now we restart to zero when user go back to hour choice
                if (!Buddy.Vocal.IsSpeaking)
                {
                    ReminderData.Instance.AppState--;
                    Trigger("HourChoiceState");
                }
            });


            // Create Step view to the button
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Create Text box to store / modify the reminder msg
                TTextBox lRecordMsg = iOnBuild.CreateWidget<TTextBox>();
                if (string.IsNullOrEmpty(mRecordedMessage))
                    lRecordMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                else
                    lRecordMsg.SetPlaceHolder(mRecordedMessage);
                lRecordMsg.OnEndEdit.Add((iText) => { mRecordedMessage = iText;});
            },
            () =>
            {
                // Click on Modify go back to record in freespeech, if buddy is not speaking
                if (!Buddy.Vocal.IsSpeaking)
                    ModifyMessage();
            },
            Buddy.Resources.GetString("modify"),
            () =>
            {
                // Click on Validate - If msg is valid, we go to next step
                if (!string.IsNullOrEmpty(mRecordedMessage) && !Buddy.Vocal.IsSpeaking)
                    ValidateMessage();
            },
            Buddy.Resources.GetString("validate"));
        }

        private void ModifyMessage()
        {
            DebugColor("MODIFY !", "blue");

            mEndRecord = false;
            mRecordedMessage = null;

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.Stop();

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            mFreeSpeechTimeStamp = Time.time;
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
            mFreeSpeechTimeStamp = Time.time;
        }

        private void ValidateMessage()
        {
            DebugColor("YOUR REMINDER :", "blue");
            DebugColor(mRecordedMessage, "green");
            DebugColor(ReminderData.Instance.ReminderDate.ToShortDateString() + " at " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
            DebugColor("REMINDER END", "blue");

            PlannedEventReminder mReminderEvent = new PlannedEventReminder(mRecordedMessage, ReminderData.Instance.ReminderDate);
            mReminderEvent.NotifyUser = true;
            Buddy.Platform.Calendar.Add(mReminderEvent);
            Buddy.Vocal.SayKey("reminderok");

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.Stop();
            QuitApp();
        }

        //  TMP - waiting for bug fix in utils
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
