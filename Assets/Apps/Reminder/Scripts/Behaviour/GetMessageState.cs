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
         *  TODO: 25.10.18
         *  Fix increment / decrement inverser
         *  test freespeech sur banc de test et demande MaJ sur epsilon
         *  ne pas regler les secondes
         */

        private bool mUi;
        private bool mReminderOk;
        private bool mEndRecord;
        private bool mVocal;
        private string mRecordedMessage;

        private TTextBox mRecordMsg;
 
        private SpeechInputParameters mVoconParam;

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
            mEndRecord = false;
            mReminderOk = false;
            mUi = false;
            mRecordedMessage = null;
            // Setting of Header
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            mVoconParam = new SpeechInputParameters();
            mVoconParam.Grammars = new string[] { "reminder" };
            //Buddy.Vocal.OnEndListening.Add(FreeSpeechResult);
        }

        private void FreeSpeechResult(SpeechInput iSpeechInput)
        {
            DebugColor("FreeSpeech SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("FreeSpeech SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            mRecordMsg.SetPlaceHolder(iSpeechInput.Utterance);
            mEndRecord = true;
            mVocal = false;
        }

        private void VoconResult(SpeechInput iSpeechInput)
        {
            DebugColor("Vocon SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Vocon SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            mVocal = false;
        }   

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mVocal)
                return;
            //if (!mEndRecord && !mVocal)
            //{
            //    // Freespech reco
            //    DebugColor("FREESPEECH", "blue");
            //    //mVocal = true;
            //    //Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("record"),
            //    //    null,
            //    //    (iOnEndListening) => { FreeSpeechResult(iOnEndListening); },
            //    //    null,
            //    //    SpeechRecognitionMode.FREESPEECH_ONLY);
            //}
            if (!mUi && !Buddy.GUI.Toaster.IsBusy)
            {
                DebugColor("DISPLAY MSG", "blue");
                Buddy.Vocal.Say(Buddy.Resources.GetString("hereisthemsg"));
                DisplayMessageEntry();
            }
            if (mReminderOk && !Buddy.Vocal.IsBusy)
            {
                DebugColor(mRecordedMessage, "green");
                DebugColor(ReminderData.Instance.ReminderDate.ToShortDateString() + " at " + ReminderData.Instance.ReminderDate.ToLongTimeString(), "green");
//                QuitApp();
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            //  Buddy.Vocal.OnEndListening.Remove(VoconGet...Result);
            //  Buddy.Vocal.Stop();
        }

        private void DisplayMessageEntry()
        {
            mUi = true;
            //mVocal = true;
            //// Ask validate or modify and reco with vocon
            //Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("validateormodify"),
            //    null,
            //    mVoconParam.Grammars,
            //    (iOnEndListening) => { VoconResult(iOnEndListening); },
            //    null);
            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("message"));
            // Create the top left button
            FButton lViewModeButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lViewModeButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_left"));
            lViewModeButton.OnClick.Add(() =>
            {
                // For now we restart to zero when user go back to date choice
                if (!Buddy.Vocal.IsSpeaking)
                {
                    ReminderData.Instance.AppState--;
                    Trigger("HourChoiceState");
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
                mRecordMsg = iOnBuild.CreateWidget<TTextBox>();
                mRecordMsg.SetPlaceHolder(Buddy.Resources.GetString("enteryourmsg"));
                mRecordMsg.OnEndEdit.Add((iText) => { mRecordedMessage = iText; DebugColor(iText, "green"); });
            },
            () =>
            {
                // Back to the free speech record
            },
            Buddy.Resources.GetString("modify"),
            () =>
            {
                if (string.IsNullOrEmpty(mRecordedMessage))
                    return ;
                DebugColor("MSG RECORD !", "blue");
                PlannedEventReminder mReminderEvent = new PlannedEventReminder(mRecordedMessage, ReminderData.Instance.ReminderDate);
                //mReminderEvent.EventTime = ReminderData.Instance.ReminderDate;
                //mReminderEvent.ReminderTime = ReminderData.Instance.ReminderDate;
                //mReminderEvent.ReminderContent = mRecordedMessage;
                mReminderEvent.NotifyUser = true;
                Buddy.Platform.Calendar.Add(mReminderEvent);
                Buddy.Vocal.SayKey("reminderok");
                Buddy.GUI.Header.HideTitle();
                Buddy.GUI.Toaster.Hide();
                Buddy.GUI.Footer.Hide();
                mReminderOk = true;
            },
            Buddy.Resources.GetString("validate"));
        }
    }
}
