using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System.Timers;

namespace BuddyApp.Reminder
{
    public sealed class HourChoiceState : AStateMachineBehaviour
    {
        private enum HourStatus
        {
            E_FIRST_LISTENING,
            E_SECOND_LISTENING,
            E_UI_DISPLAY
        }

        private HourStatus mHourStatus = HourStatus.E_FIRST_LISTENING;

        private enum DayMoment
        {
            UNKNOW,
            AM,
            PM,
        }

        // TMP - Waiting for caroussel
        private enum HourModify
        {
            MINUTE,
            HOUR,
        }

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
        private bool mBUIBlocked;

        // TMP - Waiting for caroussel
        private TText mHourText;
        private TButton mSwitch;
        private HourModify mHourModify;

        // TMP
        public void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        /*
         *   This function wait for iTitleLifeTime seconds
         *   and then Hide the header title.
         */
        public IEnumerator TitleLifeTime(float iTitleLifeTime)
        {
            yield return new WaitForSeconds(iTitleLifeTime);
            Buddy.GUI.Header.HideTitle();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            DebugColor("BEGIN HOUR", "blue");

            ReminderDateManager.GetInstance().AppState = ReminderDateManager.E_REMINDER_STATE.E_HOUR_CHOICE;

            mHourModify = HourModify.MINUTE;
            mBUIBlocked = false;

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
            Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder_hour", "common" };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_WHOURS);

            // If an hour has been already chosen, just display hour entry with this date on caroussel
            if (mHourStatus == HourStatus.E_UI_DISPLAY)
            {
                DisplayHourEntry();
            }
            else // Callback & Grammar setting & First call to Vocon
            {
                ReminderDateManager.GetInstance().ReminderDate += new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            }
        }

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
            mHourStatus = HourStatus.E_UI_DISPLAY;

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
                if (HourStatus.E_UI_DISPLAY != mHourStatus)
                {
                    mHourStatus = HourStatus.E_UI_DISPLAY;
                    DisplayHourEntry();
                }
                return;
            }

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == ReminderDateManager.STR_QUIT)
            {
                QuitReminder();
                return;
            }

            // Launch hour extraction with success
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance)
                && SharedLanguageManager<ReminderLanguage>.GetInstance().GetLanguage().ExtractHourFromSpeech(iSpeechInput.Rule, iSpeechInput.Utterance))
            {
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderDateManager.GetInstance().ReminderDate.ToShortTimeString());
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState();
                return;
            }

            // Hour extraction failed - Relaunch listenning until we make less than 2 listenning
            // Listenning count is reached - So display UI & launch the last listenning
            // Misunderstood & User didn't click on validate - We request to quit

            switch (mHourStatus)
            {
                case HourStatus.E_FIRST_LISTENING:
                    Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_SORRY);
                    mHourStatus = HourStatus.E_SECOND_LISTENING;
                    break;

                case HourStatus.E_SECOND_LISTENING:
                    Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_SORRY);
                    DisplayHourEntry();
                    mHourStatus = HourStatus.E_UI_DISPLAY;
                    break;

                case HourStatus.E_UI_DISPLAY:
                    break;
            }
        }

        private void QuitReminder()
        {
            Trigger("ExitReminder");
        }
        
        // ----- UI -----

        private void DisplayHourEntry()
        {
            DebugColor("DISPLAY HOUR", "blue");

            // Launch the quit timeout
            mTimeOut.Start();
            mTimerState = TimerState.RUNNING;
            
            //Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setuptime"));
            
            // Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = Enum.GetValues(typeof(ReminderDateManager.E_REMINDER_STATE)).Length;
            lSteps.Select((int)ReminderDateManager.GetInstance().AppState);
            lSteps.SetLabel(Buddy.Resources.GetString("steps"));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => { IncrementHour(1); });

                // Creating of a text to display the choosen hour in UI.
                mHourText = iOnBuild.CreateWidget<TText>();
                mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + ReminderDateManager.GetInstance().ReminderDate.ToShortTimeString());

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => { IncrementHour(-1); });

                // Switch button - (hour/minute)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_clock"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
                mSwitch.OnClick.Add(() => { UpdateTargetIncrement(); });
            },
            () => GoToPreviousState(), Buddy.Resources.GetString("cancel"),
            () => GoToNextState(true), Buddy.Resources.GetString("next"));
        }

        private void GoToPreviousState()
        {
            if (mBUIBlocked)
            {
                return;
            }
            mBUIBlocked = true;

            DebugColor("HOUR BACK TO DATE", "blue");
            if (Buddy.GUI.Toaster.IsBusy)
                Buddy.GUI.Header.HideTitle();
            
            Trigger("DateChoiceState");
        }

        private void GoToNextState(bool iManualValidation = false)
        {
            if (mBUIBlocked)
            {
                return;
            }
            mBUIBlocked = true;

            // Update the date with caroussel if the function is called from the toaster
            if (iManualValidation)
            {
                Buddy.GUI.Header.HideTitle();
            }
            DebugColor("HOUR IS: " + ReminderDateManager.GetInstance().ReminderDate.ToLongTimeString(), "green");
            
            Trigger("GetMessageState");
        }

        /*
         *  This function increment each hour/minute of the ReminderDate, using iIncrement.
         *  It also stop vocal.
         */
        private void IncrementHour(int iIncrement)
        {
            Buddy.Vocal.StopListening();
            if (mHourModify == HourModify.HOUR)
                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Add(new TimeSpan(iIncrement, 0, 0));
            else
                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Add(new TimeSpan(0, iIncrement, 0));
            ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Subtract(new TimeSpan(0, 0, ReminderDateManager.GetInstance().ReminderDate.Second));
            mHourText.SetLabel(Buddy.Resources.GetString("hoursetto") + ReminderDateManager.GetInstance().ReminderDate.ToString(@"hh\:mm"));
        }

        /*
         *  This function switch between hour/minute as a target to the modification on date.
         *  It also update target text in UI, and stop vocal.
         */
        private void UpdateTargetIncrement()
        {
            Buddy.Vocal.StopListening();
            if (mHourModify >= HourModify.HOUR)
                mHourModify = HourModify.MINUTE;
            else
                mHourModify++;
            if (mHourModify == HourModify.HOUR)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("hour"));
            else
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("minute"));
        }
    }
}