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
        private enum MessageStatus
        {
            E_FIRST_LISTENING,
            E_SECOND_LISTENING,
            E_UI_DISPLAY
        }

        private MessageStatus mMsgStatus = MessageStatus.E_FIRST_LISTENING;

        private const int TRY_NUMBER = 2;
        private const float QUIT_TIMEOUT = 20000;
        private const float FREESPEECH_TIMER = 15F;
        private const int RECOGNITION_SENSIBILITY = 5000;
        private const string CREDENTIAL_DEFAULT_URL = "http://bfr-dev.azurewebsites.net/dev/BuddyDev-cmfc3b05c071.txt";
        
        private float mTimer;
        private string mFreeSpeechCredentials;

        private bool mBIgnoreOnEndListening;
        private bool mBTouched;
        TTextBox mRecordedMsg;

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

        private IEnumerator GetCredentialsAndRunFreeSpeech()
        {
            WWW lWWW = new WWW(CREDENTIAL_DEFAULT_URL);
            yield return lWWW;

            mFreeSpeechCredentials = lWWW.text;

            // Setting for freespeech
            SpeechInputParameters lFreeSpeechParam = new SpeechInputParameters
            {
                RecognitionMode = SpeechRecognitionMode.FREESPEECH_ONLY,
                Credentials = mFreeSpeechCredentials
            };
            
            Buddy.Vocal.OnEndListening.Clear();
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
            ReminderDateManager.GetInstance().AppState = ReminderDateManager.E_REMINDER_STATE.E_MESSAGE_CHOICE;
            
            mTimer = -1;
            mBIgnoreOnEndListening = false;
            mRecordedMsg = null;

            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder", "common" };
            Buddy.Vocal.OnEndListening.Clear();

            // When touching the screen
            Buddy.GUI.Screen.OnTouch.Clear();
            Buddy.GUI.Screen.OnTouch.Add((iInput) => { mBTouched = true; Buddy.Vocal.StopListening(); });

            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            
            if (MessageStatus.E_UI_DISPLAY == mMsgStatus) // If was already displayed
            {
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(VoconResult);
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("hereisthemsg") + "[200]" + Buddy.Resources.GetString("validateormodify"));

                DisplayMessageEntry();
            }
            else // First call freespeech
            {
                StartCoroutine(GetCredentialsAndRunFreeSpeech());
            }
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
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            StopAllCoroutines();

            mRecordedMsg = null;
            mMsgStatus = MessageStatus.E_UI_DISPLAY;
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            // Stop Coroutine if the vocal has stop because of end of users's speech
            StopAllCoroutines();

            DebugColor("FreeSpeech Msg SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("FreeSpeech Msg SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");

            if (mBIgnoreOnEndListening)
            {
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.StopAndClear();
                return;
            }

            if (mBTouched)
            {
                mBTouched = false;
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(VoconResult);

                DisplayMessageEntry();
                return;
            }

            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                ReminderDateManager.GetInstance().ReminderMsg = iSpeechInput.Utterance;

                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(VoconResult);

                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("hereisthemsg") + "[200]" + Buddy.Resources.GetString("validateormodify"));

                DisplayMessageEntry();
                return;
            }

            if (MessageStatus.E_FIRST_LISTENING == mMsgStatus)
            {
                mMsgStatus = MessageStatus.E_SECOND_LISTENING;

                // Call freespeech
                DebugColor("FREESPEECH", "blue");
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(ReminderDateManager.STR_SORRY),
                    null,
                    FreeSpeechResult,
                    null,
                    SpeechRecognitionMode.FREESPEECH_ONLY);
                StartCoroutine(FreeSpeechLifeTime(FREESPEECH_TIMER));
            }
            else if (MessageStatus.E_SECOND_LISTENING == mMsgStatus)
            {
                mMsgStatus = MessageStatus.E_UI_DISPLAY;

                // Launch Vocon - Validation/or/Modify
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(VoconResult);

                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("hereisthemsg") + "[200]" + Buddy.Resources.GetString("validateormodify"));

                DisplayMessageEntry();
            }
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted || -1 == iSpeechInput.Confidence)
            {
                Debug.LogError("Listening was interrupted!!");
                return;
            }

            DebugColor("Vocon Validate/Modif SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon Validate/Modif SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            DebugColor("Vocon Validate/Modif SPEECH RULE: " + iSpeechInput.Rule + " --- " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == ReminderDateManager.STR_QUIT)
                QuitReminder();

            // Reset timer if the user say something
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                mTimer = 0;

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == "yes")
            {
                // If the message is empty, warn the user
                if (string.IsNullOrEmpty(ReminderDateManager.GetInstance().ReminderMsg))
                {
                    Buddy.Vocal.OnEndListening.Clear();
                    Buddy.Vocal.OnEndListening.Add(VoconResult);
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("sryemptymsg"));
                }
                else
                {
                    ValidateMessage();
                }
            }
            else if (Utils.GetRealStartRule(iSpeechInput.Rule) == "modify")
            {
                ModifyMessage();
            }
            else if (mTimer < QUIT_TIMEOUT)
            {
                Buddy.Vocal.OnEndListening.Clear();
                Buddy.Vocal.OnEndListening.Add(VoconResult);
                Buddy.Vocal.SayAndListen(ReminderDateManager.STR_SORRY);
                Debug.LogError("TRUCMUCHe");
            }
        }

        private void QuitReminder()
        {
            mBIgnoreOnEndListening = true;
            Trigger("ExitReminder");
        }

        private void DisplayMessageEntry()
        {
            mMsgStatus = MessageStatus.E_UI_DISPLAY;

            DebugColor("DISPLAY  MESSAGE", "blue");
            
            if (null == mRecordedMsg)
            {
                //  Display of the title
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("message"));

                // Create the top left button
                FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
                lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
                lViewModeButton.OnClick.Add(() =>
                {
                    DebugColor("MSG BACK TO HOUR", "blue");
                    mBIgnoreOnEndListening = true;
                    Trigger("HourChoiceState");
                });

                // Create Step view to the button
                FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
                lSteps.Dots = Enum.GetValues(typeof(ReminderDateManager.E_REMINDER_STATE)).Length;
                lSteps.Select((int)ReminderDateManager.GetInstance().AppState);
                lSteps.SetLabel(Buddy.Resources.GetString("steps"));

                // TMP - waiting for caroussel
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
                {
                    mRecordedMsg = iOnBuild.CreateWidget<TTextBox>();
                    if (string.IsNullOrEmpty(ReminderDateManager.GetInstance().ReminderMsg))
                        mRecordedMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                    else
                        mRecordedMsg.SetText(ReminderDateManager.GetInstance().ReminderMsg);
                    mRecordedMsg.OnChangeValue.Add((iText) => { ReminderDateManager.GetInstance().ReminderMsg = iText; mTimer = 0; Buddy.Vocal.StopListening(); });
                },
                () => ModifyMessage(),
                Buddy.Resources.GetString("modify"),
                () =>
                {
                    if (!string.IsNullOrEmpty(ReminderDateManager.GetInstance().ReminderMsg))
                        ValidateMessage();
                },
                Buddy.Resources.GetString("validate"));
                StartCoroutine(QuittingTimeout());
            }
            else
            {
                if (string.IsNullOrEmpty(ReminderDateManager.GetInstance().ReminderMsg))
                {
                    mRecordedMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                }
                else
                {
                    mRecordedMsg.SetText(ReminderDateManager.GetInstance().ReminderMsg);
                }
            }
        }

        private void ModifyMessage()
        {
            StopAllCoroutines();
            Buddy.Vocal.StopAndClear();

            ReminderDateManager.GetInstance().ReminderMsg = null;
            mTimer = -1;

            /*
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            */

            // Call freespeech
            DebugColor("FREESPEECH", "blue");
            Buddy.Vocal.OnEndListening.Clear();
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

            DebugColor("REMINDER SAVED:" + ReminderDateManager.GetInstance().ReminderMsg, "green");
            DebugColor("REMINDER SAVED:" + ReminderDateManager.GetInstance().ReminderDate.ToShortDateString() + " at " + ReminderDateManager.GetInstance().ReminderDate.ToLongTimeString(), "green");

            // Save the reminder
            PlannedEventReminder mReminderEvent = new PlannedEventReminder(ReminderDateManager.GetInstance().ReminderMsg, ReminderDateManager.GetInstance().ReminderDate, true);
            Buddy.Platform.Calendar.Add(mReminderEvent);

            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.SayKey("reminderok", (iOutput) => { QuitApp(); });
        }
    }
}
