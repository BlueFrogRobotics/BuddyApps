using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    /*
     *  REMAINING BUG:
     *  Impossible to launch vocon inside OnEndListenning callback ? not sure
     */
    public sealed class GetMessageState : AStateMachineBehaviour
    {
        // TMP - Wait for time out in freespeech function
        private const int TRY_NUMBER = 2;
        private const float QUIT_TIMEOUT = 20;
        private const float FREESPEECH_TIMER = 15F;
        private const int RECOGNITION_SENSIBILITY = 5000;
        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-fdec0a04c070.txt";

        private string mRecordedMessage;
        private int mListen;
        private float mTimer;
        private bool mQuit;
        private SpeechInputParameters mRecognitionParam;
        private string mFreeSpeechCredentials;

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
            yield return new WaitUntil(() => !Buddy.Vocal.IsListening);
            yield return new WaitForSeconds(iFreeSpeechTimer);
            DebugColor("STOP LISTENNING", "red");
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
            DebugColor("TIMEOUT", "red");
            QuitReminder();
        }

        public IEnumerator LaunchVocon()
        {
            yield return new WaitUntil(() => Buddy.Vocal.IsBusy);

            if (!Buddy.GUI.Toaster.IsBusy)
                DisplayMessageEntry();
        }

        private IEnumerator GetCredentialsAndRunFreeSpeech()
        {
            WWW lWWW = new WWW("http://bfr-dev.azurewebsites.net/dev/BuddyDev-fdec0a04c070.txt");
            yield return lWWW;

            mFreeSpeechCredentials = lWWW.text;

            // Setting for freespeech
            SpeechInputParameters lFreeSpeechParam = new SpeechInputParameters();
            lFreeSpeechParam.RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY;
            lFreeSpeechParam.Credentials = mFreeSpeechCredentials;

            Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("record")),
                null,
                FreeSpeechResult,
                null,
                lFreeSpeechParam);
            StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
            DebugColor("FREESPEECH", "blue");
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListen = 0;
            mTimer = -1;
            mQuit = false;
            mRecordedMessage = null;

            // Setting of Vocon param
            mRecognitionParam = new SpeechInputParameters();
            mRecognitionParam.RecognitionThreshold = RECOGNITION_SENSIBILITY;
            mRecognitionParam.Grammars = new string[] { "reminder", "common" };

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Call freespeech
            StartCoroutine(GetCredentialsAndRunFreeSpeech());
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            // Stop Coroutine if the vocal has stop because of end of users's speech
            StopAllCoroutines();
            if (mQuit)
                return;

            DebugColor("FreeSpeech Msg SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("FreeSpeech Msg SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            mListen++;

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
                    FreeSpeechResult,
                    null,
                    SpeechRecognitionMode.FREESPEECH_ONLY);
                StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
            }
            else
                DisplayMessageEntry();
            //StartCoroutine(LaunchVocon()); // TODO TEST if it works without coroutine
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted || mQuit)
                return;

            DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            DebugColor("Vocon Validate/Modif SPEECH RULE: " + iSpeechInput.Rule + " --- " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "quit")
                QuitReminder();

            // Reset timer if the user say something
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                mTimer = 0;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
            {
                // If the message is empty, warn the user
                if (string.IsNullOrEmpty(mRecordedMessage))
                    Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("sryemptymsg")), null, VoconResult, null, mRecognitionParam);
                else
                    ValidateMessage();
            }
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "modify")
                ModifyMessage();
            else if (mTimer < QUIT_TIMEOUT)
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("srynotunderstand")), null, VoconResult, null, mRecognitionParam);
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
            DebugColor("STATE EXIT", "red");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopAllCoroutines();
            Buddy.Vocal.Stop();
            Buddy.Vocal.StopAndClear();
        }

        private void QuitReminder()
        {
            DebugColor("QUITTING GET MSG", "red");
            mQuit = true;
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            StopAllCoroutines();
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.SayKey("bye", (iOutput) => { QuitApp(); });
        }

        private void DisplayMessageEntry()
        {
            // Launch Vocon - Validation/or/Modify
            Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("hereisthemsg") + "[200]" + Buddy.Resources.GetString("validateormodify")),
                null,
                VoconResult,
                null,
                mRecognitionParam);

            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("message"));

            // Create the top left button
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() =>
            {
                ReminderData.Instance.AppState--;
                Trigger("HourChoiceState");
            });


            // Create Step view to the button
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                TTextBox lRecordMsg = iOnBuild.CreateWidget<TTextBox>();
                if (string.IsNullOrEmpty(mRecordedMessage))
                    lRecordMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                else
                    lRecordMsg.SetText(mRecordedMessage);
                lRecordMsg.OnChangeValue.Add((iText) => { mRecordedMessage = iText; mTimer = 0; Buddy.Vocal.StopListening(); });
            },
            () => ModifyMessage(),
            Buddy.Resources.GetString("modify"),
            () => 
            {
                if (!string.IsNullOrEmpty(mRecordedMessage))
                    ValidateMessage();
            },
            Buddy.Resources.GetString("validate"));
            StartCoroutine(QuittingTimeout());
        }

        private void ModifyMessage()
        {
            StopAllCoroutines();
            Buddy.Vocal.StopAndClear();

            mRecordedMessage = null;
            mListen = 0;
            mTimer = -1;

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                FreeSpeechResult,
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
            StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
        }

        private void ValidateMessage()
        {
            StopAllCoroutines();
            Buddy.Vocal.StopAndClear();

            DebugColor("REMINDER SAVED:" + mRecordedMessage, "green");
            DebugColor("REMINDER SAVED:" + ReminderData.Instance.ReminderDate.ToShortDateString() + " at " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");

            // Save the reminder
            PlannedEventReminder mReminderEvent = new PlannedEventReminder(mRecordedMessage, ReminderData.Instance.ReminderDate, true);
            Buddy.Platform.Calendar.Add(mReminderEvent);

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.SayKey("reminderok", (iOutput) => { QuitApp(); });
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
