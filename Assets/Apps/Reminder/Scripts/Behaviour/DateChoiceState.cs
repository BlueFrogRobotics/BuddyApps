using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class DateChoiceState : AStateMachineBehaviour
    {
        private enum DateModify
        {
            DAY,
            MONTH,
            YEAR,
        }

        private const float TITLE_TIMER = 2.500F;
        private const int JANUARY = 1;
        private const int DECEMBER = 12;
        private const int TRY_NUMBER = 2;

        private float mTitleTStamp;
        private bool mVocal;
        private int mListen;
        private bool mUi;
        private DateTime mCarousselDate;
        private TText mDateText;
        private TButton mSwitch;
        private DateModify mDateModify;
        private SpeechInputParameters mVoconParam;

        // TMP
        private void DebugColor(string msg, string color)
        {
            if (string.IsNullOrEmpty(color))
                Debug.Log(msg);
            else
                Debug.Log("<color=" + color + ">----" + msg + "----</color>");
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mDateModify = DateModify.DAY;
            mListen = 0;
            mVocal = false;
            mUi = false;
            mTitleTStamp = 0;
            // Init the data in an InitState - For now, when we go back to that state the date is reset
            ReminderData.Instance.AppState = 0;
            ReminderData.Instance.ReminderDate = new DateTime(0001, 01, 01);
            // Header setting
            Buddy.GUI.Header.DisplayParametersButton(false);
            Font lHeaderFont = Buddy.Resources.Get<Font>("os_awesome");
            Buddy.GUI.Header.SetCustomLightTitle(lHeaderFont);
            // Grammar setting for STT
            mVoconParam = new SpeechInputParameters();
            mVoconParam.Grammars = new string[] { "reminder" };
            // STT Callback Setting
            Buddy.Vocal.OnEndListening.Add(VoconGetDateResult);
        }

        private bool DateIsDefault(DateTime iDate)
        {
            if (DateTime.Compare(iDate, new DateTime(0001, 01, 01)) == 0)
                return true;
            return false;
        }

        private void VoconGetDateResult(SpeechInput iSpeechInput)
        {
            DebugColor("Date SPEECH.ToString: " + iSpeechInput.ToString(), "blue");
            DebugColor("Date SPEECH.Utterance: " + iSpeechInput.Utterance, "blue");
            if (!string.IsNullOrEmpty(iSpeechInput.Utterance))
                ReminderData.Instance.ReminderDate = ExtractDateFromSpeech(iSpeechInput.Utterance);
            if (!DateIsDefault(ReminderData.Instance.ReminderDate) && !mUi)
            {
                DebugColor("DATE IS: " + ReminderData.Instance.ReminderDate.ToShortDateString(), "green");
                Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("eared") + ReminderData.Instance.ReminderDate.ToShortDateString());
                mTitleTStamp = Time.time;
            }
            mListen++;
            mVocal = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!DateIsDefault(ReminderData.Instance.ReminderDate) && (Time.time - mTitleTStamp) > TITLE_TIMER)
            {
                ReminderData.Instance.AppState++;
                Trigger("HourChoiceState");
            }
            if (mVocal)
                return;
            if (mListen < TRY_NUMBER && DateIsDefault(ReminderData.Instance.ReminderDate))
            {
                Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("when"), mVoconParam.Grammars);
                mVocal = true;
            }
            else if (mListen >= TRY_NUMBER)
            {
                if (!mUi && DateIsDefault(ReminderData.Instance.ReminderDate))
                {
                    //The last listenning
                    Buddy.Vocal.SayAndListen(Buddy.Resources.GetString("srynotunderstand"), mVoconParam.Grammars);
                    mVocal = true;
                    if (!Buddy.Vocal.IsSpeaking)
                        DisplayDateEntry();
                }
                else if (!Buddy.Vocal.IsBusy && DateIsDefault(ReminderData.Instance.ReminderDate))
                {
                    Debug.Log("------- QUIT -------");
                    Buddy.GUI.Header.HideTitle();
                    Buddy.GUI.Toaster.Hide();
                    Buddy.GUI.Footer.Hide();
                    Buddy.Vocal.SayKey("bye");
                    if (!Buddy.Vocal.IsBusy)
                    {
                        Buddy.Vocal.OnEndListening.Remove(VoconGetDateResult);
                        Buddy.Vocal.Stop();
                        QuitApp();
                    }
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
            Buddy.Vocal.OnEndListening.Remove(VoconGetDateResult);
            Buddy.Vocal.Stop();
        }

        // ---- UI -----
        private void DisplayDateEntry()
        {
            mUi = true;
            //Set to default for now
            mCarousselDate = DateTime.Today.Date;
            //  Display of the title
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("setupdate"));
            // TMP - waiting for caroussel and dot list
            FDotNavigation lSteps = Buddy.GUI.Footer.CreateOnMiddle<FDotNavigation>();
            lSteps.Dots = ReminderData.Instance.AppStepNumbers;
            lSteps.Select(ReminderData.Instance.AppState);
            // Bug is fix - wait for push
            // lSteps.SetLabel("Steps");
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
            {
                // Init date to today
                mCarousselDate = DateTime.Today.Date;
                // Increment Button
                TButton lInc = iOnBuild.CreateWidget<TButton>();
                lInc.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_plus"));
                lInc.SetLabel(Buddy.Resources.GetString("inc"));
                lInc.OnClick.Add(() => 
                {
                    if (mDateModify == DateModify.DAY)
                        mCarousselDate = mCarousselDate.AddDays(1);
                    else if (mDateModify == DateModify.MONTH)
                        mCarousselDate = mCarousselDate.AddMonths(1);
                    else if (mDateModify == DateModify.YEAR)
                        mCarousselDate = mCarousselDate.AddYears(1);
                    mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + mCarousselDate.ToShortDateString());
                });

                // Text to display choosen date
                mDateText = iOnBuild.CreateWidget<TText>();
                mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + mCarousselDate.ToShortDateString());

                // Decrement Button
                TButton lDec = iOnBuild.CreateWidget<TButton>();
                lDec.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_minus"));
                lDec.SetLabel(Buddy.Resources.GetString("dec"));
                lDec.OnClick.Add(() => 
                {
                    if (mDateModify == DateModify.DAY)
                        mCarousselDate = mCarousselDate.AddDays(-1);
                    else if (mDateModify == DateModify.MONTH)
                        mCarousselDate = mCarousselDate.AddMonths(-1);
                    else if (mDateModify == DateModify.YEAR)
                        mCarousselDate = mCarousselDate.AddYears(-1);
                    mDateText.SetLabel(Buddy.Resources.GetString("datesetto") + mCarousselDate.ToShortDateString());
                });

                // Switch button - (day/month/year)
                mSwitch = iOnBuild.CreateWidget<TButton>();
                mSwitch.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_agenda_check"));
                mSwitch.SetLabel(Buddy.Resources.GetString("modify") + Buddy.Resources.GetString("day"));
                mSwitch.OnClick.Add(() =>
                {
                    if (mDateModify >= DateModify.YEAR)
                        mDateModify = DateModify.DAY;
                    else
                        mDateModify++;
                    if (mDateModify == DateModify.DAY)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("day"));
                    else if (mDateModify == DateModify.MONTH)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("month"));
                    else if (mDateModify == DateModify.YEAR)
                        mSwitch.SetLabel(Buddy.Resources.GetString("modify") + " " + Buddy.Resources.GetString("year"));
                });
            },
            () =>
            {
                // [TODO] Back to recipient when available
            },
            Buddy.Resources.GetString("cancel"),
            () =>
            {
                ReminderData.Instance.ReminderDate = mCarousselDate;
                DebugColor(ReminderData.Instance.ReminderDate.ToShortDateString(), "green");
                ReminderData.Instance.AppState++;
                Trigger("HourChoiceState");
            },
            Buddy.Resources.GetString("next"));
        }

        //  ---- PARSING FUNCTION ---- 

        private DateTime ExtractDateFromSpeech(string iSpeech)
        {
            string[] lWords = iSpeech.Split(' ');
            if (ContainsOneOf(iSpeech, "today") && lWords.Length == 1)
                return DateTime.Today.Date;
            else if (ContainsOneOf(iSpeech, "tomorrow") && lWords.Length == 1)
                return DateTime.Today.AddDays(1F).Date;
            else if (ContainsOneOf(iSpeech, "weekdays") && lWords.Length == 1)
                return GetNextDay(DateTime.Today, iSpeech.Split(' '));
            else if (ContainsOneOf(iSpeech, "dayaftertomorrow"))
                return DateTime.Today.AddDays(2F).Date;
            else if (ContainsOneOf(iSpeech, "weekdays") || ContainsOneOf(iSpeech, "the") || ContainsOneOf(iSpeech, "allmonth"))
                return TryToBuildDate(iSpeech);
            else if (ContainsOneOf(iSpeech, "intime"))
            {
                UInt16 lNb = 0;
                // If the speech contain less than 3 word, the order is inconsistent
                if (lWords.Length < 3)
                    new DateTime(0001, 01, 01);
                for (int lIndex = 0; lIndex < lWords.Length; ++lIndex)
                {
                    if (lWords[lIndex].ToLower() == Buddy.Resources.GetString("intime") && lIndex + 2 < lWords.Length)
                    {
                        // Check if is't necessary to test the presence of the word "day" / "month" / "year", maybe the grammar of vocon is enough
                        if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "day"))
                            return DateTime.Today.AddDays(lNb).Date;
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "month"))
                            return DateTime.Today.AddMonths(lNb).Date;
                        else if (UInt16.TryParse(lWords[lIndex + 1], out lNb) && ContainsOneOf(lWords[lIndex + 2], "year"))
                            return DateTime.Today.AddYears(lNb).Date;
                    }
                }
            }
            return new DateTime(0001, 01, 01);
        }

        // Try to build a date - Find day / number, month,
        // If number or month is absent - check if "next" is present -> GetNextDay
        // Find a day number
        private DateTime TryToBuildDate(string iSpeech)
        {
            UInt16 lDayNum = 0;
            int lMonth = DateTime.Today.Month;
            int lYear = DateTime.Today.Year;
            string[] lSpeechWords = iSpeech.Split(' ');

            // Get a number in speech
            foreach (string lWord in lSpeechWords)
            {
                if (UInt16.TryParse(lWord, out lDayNum))
                    break;
            }
            // If the day number is valid and a month is choose, create a date
            if (lDayNum >= 1 && lDayNum <= 31 && ContainsOneOf(iSpeech, "allmonth"))
            {
                // Find the choosen month - If an error occured, the date extraction is stopped.
                if ((lMonth = FindMonthNumber(lSpeechWords)) < 0)
                    return new DateTime(0001, 01, 01);
                // If the month number is passed OR, if the day is passed and the choosen month is the current month, increment the year.
                if (lMonth < DateTime.Today.Month || (lMonth == DateTime.Today.Month && lDayNum < DateTime.Today.Day))
                    lYear++;
                return new DateTime(lYear, lMonth, lDayNum);
            }
            // If the day number is valid but without month, we create a date to the next day number.
            else if (lDayNum >= 1 && lDayNum <= 31)
            {
                // If the day number is passed, use the next month
                if (lDayNum < DateTime.Today.Day)
                    lMonth++;
                // Set to january if we were in december - Happy new year !
                if (lMonth > DECEMBER)
                {
                    lMonth = JANUARY;
                    lYear++;
                }
                return new DateTime(lYear, lMonth, lDayNum);
            }
            // If no number is choose => speech = "the" + "next" ([TODO] Make this impossible with the grammar) || "weekdays" + "next"
            else if (lDayNum == 0 && ContainsOneOf(iSpeech, "next"))
                return GetNextDay(DateTime.Today, lSpeechWords);
            return new DateTime(0001, 01, 01);
        }

        /*
         *  Return the number of the first month found in iSpeech.
         *  If no month are found, negative value is return.
         */
        private int FindMonthNumber(string[] iSpeech)
        {
            string[] lMonthArray = Buddy.Resources.GetPhoneticStrings("allmonth");

            foreach (string lWord in iSpeech)
            {
                for (int lIndex = 0; lIndex < lMonthArray.Length; lIndex++)
                {
                    if (lMonthArray[lIndex].ToLower() == lWord.ToLower())
                        return (lIndex + 1);
                }
            }
            return -1;
        }

        /*
         *  Return the date in string format, of the next day, from a start day.
         *  (It takes the first encounter day in the array)
         *  If an error occured, a negative value is return.
         */
        private DateTime GetNextDay(DateTime iStartDay, string[] iSpeech)
        {
            bool lFind = false;
            string[] lWeekDays = Buddy.Resources.GetPhoneticStrings("weekdays");
            int lNumDay = -1;
            int lDaysToAdd = 0;

            // Find the first day in the array
            foreach (string lWord in iSpeech)
            {
                for (int lIndex = 0; lIndex < lWeekDays.Length; lIndex++)
                {
                    if (lWeekDays[lIndex] == lWord)
                    {
                        lNumDay = lIndex;
                        lFind = true;
                        break;
                    }
                }
                if (lFind)
                    break;
            }
            if (lNumDay < 0)
                return new DateTime(0001, 01, 01);
            // Substract the choosen day number with the current day number
            // The calcul assure the range is between [1-7]
            lDaysToAdd = ((lNumDay - (int)iStartDay.DayOfWeek + 7) % 7) + 1;
            return iStartDay.AddDays(lDaysToAdd).Date;
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
