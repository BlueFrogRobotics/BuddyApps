using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System.Timers;

namespace BuddyApp.Reminder
{
    public class RecurrenceChoiceState : AStateMachineBehaviour
    {
        private enum RecurrenceStatus
        {
            E_FIRST_LISTENING,
            E_SECOND_LISTENING,
            E_UI_DISPLAY
        }

        private RecurrenceStatus mRecurrenceStatus = RecurrenceStatus.E_FIRST_LISTENING;

        private enum TimerState
        {
            RUNNING,
            TIMEOUT,
            STOPPED,
        }

        private const float QUIT_TIMEOUT = 20000.0F;
        private const float TITLE_TIMER = 2.500F;

        private Timer mTimeOut;
        private TimerState mTimerState = TimerState.STOPPED;

        // TMP - Debug function - Add to shared ?
        private void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        /*
         *  This function wait for iTitleLifeTime seconds
         *  and then Hide the header title.
         */
        public IEnumerator TitleLifeTime(float iTitleLifeTime)
        {
            yield return new WaitForSeconds(iTitleLifeTime);
            Buddy.GUI.Header.HideTitle();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DebugColor("State Enter Recurrence", "blue");

            ReminderDateManager.GetInstance().AppState = ReminderDateManager.E_REMINDER_STATE.E_RECURRENCE_CHOICE;

            // Set the time out quit event.
            mTimeOut = new Timer(QUIT_TIMEOUT);
            mTimeOut.Elapsed += delegate { mTimerState = TimerState.TIMEOUT; };
            mTimeOut.Enabled = true;
            mTimeOut.Stop();

            // When touching the screen
            Buddy.GUI.Screen.OnTouch.Clear();
            Buddy.GUI.Screen.OnTouch.Add((iInput) => { mTimeOut.Interval = QUIT_TIMEOUT; });
            Buddy.GUI.Screen.OnTouch.Add((iInput) => { Buddy.Vocal.StopListening(); });

            // Setting of Vocon param
            Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "recurrence", "common" };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            // If a date has been already choose, just display Date entry with this date on caroussel
            if (RecurrenceStatus.E_UI_DISPLAY == mRecurrenceStatus)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(ReminderDateManager.STR_WHEN));
                DisplayRecurrenceEntry();
            }
            else
            {
                Buddy.Vocal.SayKeyAndListen("howoften");
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset the timeout timer, on touch
            if (TimerState.TIMEOUT == mTimerState)
            {
                QuitReminder();
                mTimerState = TimerState.STOPPED;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRecurrenceStatus = RecurrenceStatus.E_UI_DISPLAY;

            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            mTimeOut.Stop();
            mTimerState = TimerState.STOPPED;
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            if (iSpeechInput.IsInterrupted)
            {
                if (RecurrenceStatus.E_UI_DISPLAY != mRecurrenceStatus)
                {
                    mRecurrenceStatus = RecurrenceStatus.E_UI_DISPLAY;
                    DisplayRecurrenceEntry();
                }
                return;
            }

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == ReminderDateManager.STR_QUIT)
            {
                QuitReminder();
                return;
            }
            
            RepetitionTime lRepetitionTime = RepetitionTime.ONCE;
            List<DayOfWeek> lRepetitionDays = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance)
                && ReminderLanguageManager.GetInstance().GetRecurrenceLanguage().ExtractRecurrenceFromSpeech(
                                        Utils.GetRealStartRule(iSpeechInput.Rule),
                                        iSpeechInput.Utterance,
                                        ref lRepetitionTime,
                                        ref lRepetitionDays))
            {

                ReminderDateManager.GetInstance().RepetitionTime = lRepetitionTime;
                ReminderDateManager.GetInstance().RepetitionDays = lRepetitionDays;
                
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderDateManager.GetInstance().ReminderDate.ToShortTimeString());
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState();
                return;
            }

            // Recurrence extraction failed - Relaunch listenning until we make less than 2 listenning
            // Listenning count is reached - So display UI & launch the last listenning
            // Misunderstood & User didn't click on validate - We request to quit
            switch (mRecurrenceStatus)
            {
                case RecurrenceStatus.E_FIRST_LISTENING:
                    Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_SORRY);
                    mRecurrenceStatus = RecurrenceStatus.E_SECOND_LISTENING;
                    break;

                case RecurrenceStatus.E_SECOND_LISTENING:
                    Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_SORRY);
                    DisplayRecurrenceEntry();
                    mRecurrenceStatus = RecurrenceStatus.E_UI_DISPLAY;
                    break;

                case RecurrenceStatus.E_UI_DISPLAY:
                    break;
            }
        }

        private void DisplayRecurrenceEntry()
        {
            DebugColor("DISPLAY RECURRENCE", "blue");

            // Launch the quit timeout
            mTimeOut.Start();
            mTimerState = TimerState.RUNNING;

            //Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setuprecurrence"));

            // Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = Enum.GetValues(typeof(ReminderDateManager.E_REMINDER_STATE)).Length;
            lSteps.Select((int)ReminderDateManager.GetInstance().AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Recurrence Button
                TButton lOnce = iOnBuild.CreateWidget<TButton>();
                TButton lDayly = iOnBuild.CreateWidget<TButton>();
                TButton lWeekly = iOnBuild.CreateWidget<TButton>();
                TButton lTwoWeeks = iOnBuild.CreateWidget<TButton>();
                TButton lMontly = iOnBuild.CreateWidget<TButton>();
                TButton lAnnual = iOnBuild.CreateWidget<TButton>();

                // Labels
                lOnce.SetLabel(Buddy.Resources.GetString("once"));
                lDayly.SetLabel(Buddy.Resources.GetString("dayly"));
                lWeekly.SetLabel(Buddy.Resources.GetString("weekly"));
                lTwoWeeks.SetLabel(Buddy.Resources.GetString("everytwoweeks"));
                lMontly.SetLabel(Buddy.Resources.GetString("monthly"));
                lAnnual.SetLabel(Buddy.Resources.GetString("annual"));

                // Actions
                lOnce.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.ONCE; });
                lDayly.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.DAYLY; });
                lWeekly.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.WEEKLY; });
                lTwoWeeks.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.EVERY_TWO_WEEKS; });
                lMontly.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.MONTHLY; });
                lAnnual.OnClick.Add(() => { ReminderDateManager.GetInstance().RepetitionTime = RepetitionTime.ANNUAL; });
            },
            () => GoToPreviousState(), Buddy.Resources.GetString("cancel"),
            () => GoToNextState(true), Buddy.Resources.GetString("next"));
        }

        private void QuitReminder()
        {
            Trigger("ExitReminder");
        }

        private void GoToPreviousState()
        {
            if (Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Header.HideTitle();

            Trigger("HourChoiceState");
        }

        private void GoToNextState(bool iManualValidation = false)
        {
            // Update the date with caroussel if the function is called from the toaster
            if (iManualValidation)
            {
                Buddy.GUI.Header.HideTitle();
            }
            
            Trigger("GetMessageState");
        }
    }
}
