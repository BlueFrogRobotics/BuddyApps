using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    /*
     *  REMAINING BUG:
     *  
     *  It's possible that buddy quit before he said "Bye"
     *  => For now i can't check if the vocal queue is empty before quitting.
     *  Check with yield waitforsecond betweeen two IsSpeaking false ?
     *  
     *  When we use tempo in text to speech, ex:[100],
     *  Buddy.Vocal.IsSpeaking is false during the tempo
     *  
     *  Sometimes object in the footer are duplicate, in getmessage state
     *  This bugs seems to be fix, waiting for more test to validate that is really fix.
     *  
     *  Impossible to launch vocon inside OnEndListenning callback ? not sure
     */
    public sealed class GetMessageState : AStateMachineBehaviour
    {
        // TMP - Wait for time out in freespeech function
        private const int TRY_NUMBER = 1;
        private const float QUIT_TIMEOUT = 20;
        private const float FREESPEECH_TIMER = 15F;
        private const int RECOGNITION_SENSIBILITY = 5000;

        private string mRecordedMessage;
        private int mListen;
        private float mTimer;
        private bool mQuit;
        private SpeechInputParameters mRecognitionParam;

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
            DebugColor("STOPLISTENNING", "red");
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
            yield return new WaitUntil(() => Buddy.Vocal.IsBusy);
            QuitReminder();
        }

        // TODO Test without coroutine to check if bug is real
        public IEnumerator LaunchVocon()
        {
            yield return new WaitUntil(() => Buddy.Vocal.IsBusy);

            if (!Buddy.GUI.Toaster.IsBusy)
                DisplayMessageEntry();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mListen = 0;
            mTimer = -1;
            mQuit = false;
            mRecordedMessage = null;

            mRecognitionParam = new SpeechInputParameters();
            mRecognitionParam.RecognitionThreshold = RECOGNITION_SENSIBILITY;
            mRecognitionParam.Grammars = new string[] { "reminder", "common" };

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0, 1F);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
                null,
                FreeSpeechResult,
                null,
                SpeechRecognitionMode.FREESPEECH_ONLY);
            StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
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
                StartCoroutine(LaunchVocon());
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            Debug.LogWarning("CALLBACK IS RUNNNING - timer: " + mTimer.ToString() + "TIMEOUT: " + QUIT_TIMEOUT + "mQuit: " + mQuit);

            if (iSpeechInput.IsInterrupted || mQuit || mTimer >= QUIT_TIMEOUT)
                return;

            DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            DebugColor("Vocon Validate/Modif SPEECH RULE: " + iSpeechInput.Rule + " --- " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");

            // Reset timer if the user say something
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                mTimer = 0;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
            {
                // If the message is empty, warn the user
                if (string.IsNullOrEmpty(mRecordedMessage))
                {
                    Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("sryemptymsg")),
                        null,
                        VoconResult,
                        null,
                        mRecognitionParam);
                }
                else
                    ValidateMessage();
            }
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "modify")
                ModifyMessage();
            else if (mTimer < QUIT_TIMEOUT)
            {
                Buddy.Vocal.SayAndListen(new SpeechOutput(Buddy.Resources.GetString("srynotunderstand")),
                    null,
                    VoconResult,
                    null,
                    mRecognitionParam);
                DebugColor("SORRY DONT UNDERSTAND", "red");
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
        }

        private void QuitReminder()
        {
            mQuit = true;
            Buddy.Vocal.SayKey("bye");
            DebugColor("QUITTING GET MSG", "red");
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.StopListening();
            QuitApp();
        }

        private void DisplayMessageEntry()
        {
            DebugColor("DISPLAY MSG", "blue");

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
                TTextBox lRecordMsg = iOnBuild.CreateWidget<TTextBox>();
                if (string.IsNullOrEmpty(mRecordedMessage))
                    lRecordMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                else
                    lRecordMsg.SetPlaceHolder(mRecordedMessage);
                //lRecordMsg.OnEndEdit.Add((iText) => { mRecordedMessage = iText; mTimer = 0; });
                lRecordMsg.OnChangeValue.Add((iText) => { mRecordedMessage = iText; mTimer = 0; });
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
            StartCoroutine(QuittingTimeout());
        }

        private void ModifyMessage()
        {
            StopAllCoroutines();

            mRecordedMessage = null;
            mListen = 0;
            mTimer = -1;

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();

            // ? issue  ? Test with Vocal.Stop fix when available
            Buddy.Vocal.StopListening();

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

            Buddy.Vocal.SayKey("reminderok");

            DebugColor("REMINDER SAVED:" + mRecordedMessage, "green");
            DebugColor("REMINDER SAVED:" + ReminderData.Instance.ReminderDate.ToShortDateString() + " at " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");

            // Save the reminder
            PlannedEventReminder mReminderEvent = new PlannedEventReminder(mRecordedMessage, ReminderData.Instance.ReminderDate);
            mReminderEvent.NotifyUser = true;
            Buddy.Platform.Calendar.Add(mReminderEvent);

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
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
