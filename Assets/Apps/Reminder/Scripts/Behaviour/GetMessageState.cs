using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class GetMessageState : AStateMachineBehaviour
    {
        // TMP - Wait for time out in freespeech function
        private const int TRY_NUMBER = 1;
        private float FREESPEECH_TIMER = 15F;
        private const float QUIT_TIMEOUT = 20;

        private string mRecordedMessage;
        private int mListen;
        private float mTimer;
        private bool mQuit;

        // TMP - Debug
        public void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        /*
         *   This function wait for iFreeSpeechTimer seconds
         *   and then stop vocal listenning
         */
        public IEnumerator FreeSpeechLifeTime(float iFreeSpeechTimer)
        {
            yield return new WaitForSeconds(iFreeSpeechTimer);
            Buddy.Vocal.StopListening();
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
            mListen = 0;
            mTimer = -1;
            mQuit = false;
            mRecordedMessage = null;

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
            StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            if (mQuit)
                return;
            mListen++;
            DebugColor("FreeSpeech Msg SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("FreeSpeech Msg SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                mRecordedMessage = iSpeechInput.Utterance;
                DisplayMessageEntry();
                return;
            }

            if (mListen < TRY_NUMBER)
            {
                // Call freespeech
                DebugColor("FREESPEECH", "blue");
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"),
                    null,
                    (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
                    null,
                    SpeechRecognitionMode.FREESPEECH_ONLY);
                StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
            }
            else
                DisplayMessageEntry();
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted || mQuit)
                return;
            DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            DebugColor("Vocon Validate/Modif SPEECH RULE: " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                DebugColor("SORRY DONT UNDERSTAND", "red");
                return;
            }
            else if (string.Equals(Utils.GetRealStartRule(iSpeechInput.Rule), "yes") && !string.IsNullOrEmpty(mRecordedMessage))
                ValidateMessage();
            else if (string.Equals(Utils.GetRealStartRule(iSpeechInput.Rule), "modify"))
                ModifyMessage();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DebugColor("STATE EXIT", "red");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopAllCoroutines();
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
            QuitApp();
        }

        private void DisplayMessageEntry()
        {
            DebugColor("DISPLAY MSG", "blue");

            StartCoroutine(QuittingTimeout());

            // Ask validate or modify and reco with vocon
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("hereisthemsg") + "[200]" + Buddy.Resources.GetString("validateormodify"),
                null,
                new string[] { "reminder", "common" },
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
                TText lTmpText = iOnBuild.CreateWidget<TText>();
                if (string.IsNullOrEmpty(mRecordedMessage))
                    lTmpText.SetLabel("You will be able to edit the text soon");
                else
                    lTmpText.SetLabel(mRecordedMessage);

                //TTextBox lRecordMsg = iOnBuild.CreateWidget<TTextBox>();
                //if (string.IsNullOrEmpty(mRecordedMessage))
                //    lRecordMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                //else
                //    lRecordMsg.SetPlaceHolder(mRecordedMessage);
                ////lRecordMsg.OnEndEdit.Add((iText) => { mRecordedMessage = iText; mTimer = 0; });
                //lRecordMsg.OnChangeValue.Add((iText) => { mRecordedMessage = iText; mTimer = 0; });
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
            mRecordedMessage = null;
            mListen = 0;
            mTimer = -1;

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.Stop();

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
            StopAllCoroutines();
            StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
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
            StopAllCoroutines();
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
