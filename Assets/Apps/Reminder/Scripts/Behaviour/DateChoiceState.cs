using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System.Timers;

namespace BuddyApp.Reminder
{
    public sealed class DateChoiceState : AStateMachineBehaviour
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
            ReminderDateManager.GetInstance().AppState = ReminderDateManager.E_REMINDER_STATE.E_DATE_CHOICE;

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
            Buddy.Vocal.DefaultInputParameters.Grammars = new string[] { "reminder_date", "common" };
            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);
            
            // If a date has been already choose, just display Date entry with this date on caroussel
            if (DateStatus.E_UI_DISPLAY == mDateStatus)
            {
                Buddy.Vocal.StopAndClear();
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(ReminderDateManager.STR_WHEN));
                DisplayDateEntry();
            }
            else
            {
                Buddy.Vocal.SayKeyAndListen(ReminderDateManager.STR_WHEN);
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

            if (Utils.GetRealStartRule(iSpeechInput.Rule) == ReminderDateManager.STR_QUIT)
                QuitReminder();

            //  Launch Extraction date - The ReminderDate's hour is saved and restore after the extraction
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance)
                && SharedLanguageManager<ReminderLanguage>.GetInstance().GetLanguage().ExtractDateFromSpeech(iSpeechInput.Rule, iSpeechInput.Utterance))
            {
                /*
                TimeSpan lHourmem = new TimeSpan(ReminderDateManager.GetInstance().ReminderDate.Hour,
                                                        ReminderDateManager.GetInstance().ReminderDate.Minute,
                                                        ReminderDateManager.GetInstance().ReminderDate.Second);

                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.Date + lHourmem;
                */
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + SharedLanguageManager<ReminderLanguage>.GetInstance().GetLanguage().DateToString());
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
                    //Buddy.Vocal.SayKeyAndListen("when");
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(ReminderDateManager.STR_SORRY));
                    mDateStatus = DateStatus.E_SECOND_LISTENING;
                    break;

                case DateStatus.E_SECOND_LISTENING:
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString(ReminderDateManager.STR_SORRY));
                    DisplayDateEntry();
                    mDateStatus = DateStatus.E_UI_DISPLAY;
                    break;

                case DateStatus.E_UI_DISPLAY:
                    break;
            }
        }
        
        private void QuitReminder()
        {
            Trigger("ExitReminder");
        }

        /*
         *  Return true if iDate is equal to the default date time.
         *  The default date time is set to 0001, 01, 01 in InitReminder
         */
        private bool DateIsDefault(DateTime iDate)
        {
            if (DateTime.Compare(iDate, new DateTime(0001, 01, 01)) == 0)
                return true;
            return false;
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
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString(ReminderDateManager.STR_SETUPDATE));

            //  Creating of dot view at the bottom
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = Enum.GetValues(typeof(ReminderDateManager.E_REMINDER_STATE)).Length;
            lSteps.Select((int)ReminderDateManager.GetInstance().AppState);
            lSteps.SetLabel(Buddy.Resources.GetString(ReminderDateManager.STR_STEPS));

            // TMP - waiting for caroussel
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                //  Set the starting point of the carousel
                //ReminderDateManager.GetInstance().ReminderDate = iCarouselStartDate;

                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => { IncrementDate(1); });

                // Creating of a text to display the choosen date in UI.
                mDateText = iOnBuild.CreateWidget<TText>();
                mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + SharedLanguageManager<ReminderLanguage>.GetInstance().GetLanguage().DateToString());

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
            
            DebugColor("DATE IS: " + ReminderDateManager.GetInstance().ReminderDate.ToShortDateString(), "green");
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
                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddDays(iIncrement);
            else if (mDateModify == DateModify.MONTH)
                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddMonths(iIncrement);
            else if (mDateModify == DateModify.YEAR)
                ReminderDateManager.GetInstance().ReminderDate = ReminderDateManager.GetInstance().ReminderDate.AddYears(iIncrement);

            mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + SharedLanguageManager<ReminderLanguage>.GetInstance().GetLanguage().DateToString());
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
