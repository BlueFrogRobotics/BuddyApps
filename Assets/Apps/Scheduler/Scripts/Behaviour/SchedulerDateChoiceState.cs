using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System.Timers;

namespace BuddyApp.Scheduler
{
    public sealed class SchedulerDateChoiceState : AStateMachineBehaviour
    {
        private enum DateStatus
        {
            E_FIRST_LISTENING,
            E_SECOND_LISTENING,
            E_UI_DISPLAY
        }

        private DateStatus mDateStatus = DateStatus.E_FIRST_LISTENING;

        // TMP - Waiting for caroussel
        private enum DateModify
        {
            DAY,
            MONTH,
            YEAR,
        }

        private enum TimerState
        {
            RUNNING,
            TIMEOUT,
            STOPPED,
        }
        
        private const float QUIT_TIMEOUT = 20000.0F;
        private const float TITLE_TIMER = 2.500F;
        private const int JANUARY = 1;
        private const int DECEMBER = 12;

        private Timer mTimeOut;
        private TimerState mTimerState = TimerState.STOPPED;

        // TMP - Waiting for caroussel
        private TText mDateText;
        private TButton mSwitch;
        private DateModify mDateModify;

        private bool mBUIBlocked = false;

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
            DebugColor("State Enter Date", "blue");

			SchedulerDateManager.GetInstance().AppState = SchedulerDateManager.E_SCHEDULER_STATE.E_DATE_CHOICE;

            mDateModify = DateModify.DAY;
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

            // Header setting
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            lHeaderFont.material.color = new Color(0, 0, 0);
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);

			// Setting of Vocon param
			Buddy.Vocal.DefaultInputParameters.RecognitionMode = SpeechRecognitionMode.GRAMMAR_ONLY;
			Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "date", "timespan", "common" };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            
            // If a date has been already choose, just display Date entry with this date on caroussel
            if (DateStatus.E_UI_DISPLAY == mDateStatus)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(SchedulerDateManager.STR_WHEN));
                DisplayDateEntry();
            }
            else
            {
                Buddy.Vocal.SayKeyAndListen(SchedulerDateManager.STR_WHEN);
            }

            DebugColor("Finish State Enter Date", "blue");
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset the timeout timer, on touch
            if (TimerState.TIMEOUT == mTimerState)
            {
                QuitScheduler();
                mTimerState = TimerState.STOPPED;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            mTimeOut.Stop();
            mTimerState = TimerState.STOPPED;
        }

        private void OnEndListening(SpeechInput iSpeechInput)
        {
            DebugColor("Vocon Validate/Modif SPEECH RULE: " + iSpeechInput.Rule + " --- " + Utils.GetRealStartRule(iSpeechInput.Rule), "blue");
            
            if (iSpeechInput.IsInterrupted)// || -1 == iSpeechInput.Confidence)
            {
                if (DateStatus.E_UI_DISPLAY != mDateStatus)
                {
                    mDateStatus = DateStatus.E_UI_DISPLAY;
                    DisplayDateEntry();
                }
                return;
            }
            
            if (Utils.GetRealStartRule(iSpeechInput.Rule) == SchedulerDateManager.STR_QUIT)
                QuitScheduler();

            //  Launch Extraction date - The ReminderDate's hour is saved and restore after the extraction
            DateTime lDate = SchedulerDateManager.GetInstance().SchedulerDate;
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance)
                && SchedulerLanguageManager.GetInstance().GetDateLanguage().ExtractDateFromSpeech(
                            Utils.GetRealStartRule(iSpeechInput.Rule).Trim(' '),
                            iSpeechInput.Utterance.Trim(' '),
                            ref lDate))
            {
				SchedulerDateManager.GetInstance().SchedulerDate = lDate;

                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") +
					SchedulerLanguageManager.GetInstance().GetDateLanguage().DateToString(SchedulerDateManager.GetInstance().SchedulerDate));
                
                StartCoroutine(TitleLifeTime(TITLE_TIMER));
                GoToNextState();
                return;
            }
            
            // Extraction date failed - Relaunch listenning until we make less than TRY_NUMBER listenning
            // Listenning count is reached - So display date entry GUI
            // Misunderstood & User didn't click on validate - We request to quit
            switch (mDateStatus)
            {
                case DateStatus.E_FIRST_LISTENING:
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(SchedulerDateManager.STR_SORRY));
                    mDateStatus = DateStatus.E_SECOND_LISTENING;
                    break;

                case DateStatus.E_SECOND_LISTENING:
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(SchedulerDateManager.STR_SORRY));
                    DisplayDateEntry();
                    mDateStatus = DateStatus.E_UI_DISPLAY;
                    break;

                case DateStatus.E_UI_DISPLAY:
                    break;
            }
        }
        
        private void QuitScheduler()
        {
            Trigger("Exit");
        }

        /*
         *  Displaying UI to choose Date
         *  And launch the last listenning
         */
        private void DisplayDateEntry()
        {
            DebugColor("DISPLAY DATE", "blue");

            // Launch the quit timeout
            mTimeOut.Start();
            mTimerState = TimerState.RUNNING;

            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(SchedulerDateManager.STR_SETUPDATE));

            //  Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = Enum.GetValues(typeof(SchedulerDateManager.E_SCHEDULER_STATE)).Length;
            lSteps.Select((int)SchedulerDateManager.GetInstance().AppState);
            lSteps.SetLabel(Buddy.Resources.GetString(SchedulerDateManager.STR_STEPS));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => { IncrementDate(1); });

                // Creating of a text to display the choosen date in UI.
                mDateText = iOnBuild.CreateWidget<TText>();
                mDateText.SetLabel(Buddy.Resources.GetString("datesetto") +
                    SchedulerLanguageManager.GetInstance().GetDateLanguage().DateToString(SchedulerDateManager.GetInstance().SchedulerDate));

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => { IncrementDate(-1); });

                // Switch button - (day/month/year)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_agenda_check"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("day"));
                mSwitch.OnClick.Add(() => { UpdateTargetIncrement(); }); 
            },
            // Cancel date
            () => { /* Back to recipient when available */ Buddy.Vocal.StopAndClear(); }, Buddy.Resources.GetString("cancel"),
            // Validate date
            () => GoToNextState(true), Buddy.Resources.GetString("next"));
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

            mDateStatus = DateStatus.E_UI_DISPLAY;
            
            DebugColor("DATE IS: " + SchedulerDateManager.GetInstance().SchedulerDate.ToShortDateString(), "green");
            Trigger("HourChoiceState");
        }

        /*
         *  This function increment each part of the date ReminderDate, using iIncrement.
         *  It also stop vocal.
         */
        private void IncrementDate(int iIncrement)
        {
            Buddy.Vocal.StopListening();
            if (mDateModify == DateModify.DAY)
				SchedulerDateManager.GetInstance().SchedulerDate = SchedulerDateManager.GetInstance().SchedulerDate.AddDays(iIncrement);
            else if (mDateModify == DateModify.MONTH)
				SchedulerDateManager.GetInstance().SchedulerDate = SchedulerDateManager.GetInstance().SchedulerDate.AddMonths(iIncrement);
            else if (mDateModify == DateModify.YEAR)
				SchedulerDateManager.GetInstance().SchedulerDate = SchedulerDateManager.GetInstance().SchedulerDate.AddYears(iIncrement);
            
            mDateText.SetLabel(Buddy.Resources.GetString("datesetto") +
                SchedulerLanguageManager.GetInstance().GetDateLanguage().DateToString(SchedulerDateManager.GetInstance().SchedulerDate));
        }

        /*
         *  This function switch between day/month/year as a target to the modification on date.
         *  It also update target text in UI, and stop vocal.
         */
        private void UpdateTargetIncrement()
        {
            Buddy.Vocal.StopListening();
            mDateModify++;
            if (mDateModify > DateModify.YEAR)
                mDateModify = DateModify.DAY;
            if (mDateModify == DateModify.DAY)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("day"));
            else if (mDateModify == DateModify.MONTH)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("month"));
            else if (mDateModify == DateModify.YEAR)
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("year"));
        }
    }
}
