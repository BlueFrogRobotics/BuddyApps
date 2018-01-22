using Buddy;
using Buddy.UI;

using System;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

using Buddy.Command;
using System.Globalization;


namespace BuddyApp.Reminder
{

    public class HourState : AStateMachineBehaviour
    {
        VerticalCarouselInfo mCarouselHour;
        VerticalCarouselInfo mCarouselMinute;
        VerticalCarouselInfo mCarouselSecond;
        private int mHour = 0;
        private int mMinute = 0;
        private int mSecond = 0;

        private bool ScrollOn = false;

        // Use this for initialization

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocal = GetComponent<ReminderBehaviour>();
            mVocal.QuestionTime(Dictionary.GetRandomString("whours"));
        }
         

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ReminderData.Instance.VocalRequest != "" && mVocal.IsVocalGet)
            {

                Debug.Log("Coucou Hiboux");
                if (ExtractHour(ReminderData.Instance.VocalRequest))
                {
                    HourOk();
                }
                else
                {
                    if (ScrollOn)
                    {
                        Debug.Log("Saperlipopette");
                        CreateCarousels();
                    }
                    ReminderData.Instance.VocalRequest = "";
                    mVocal.QuestionTime(Dictionary.GetRandomString("what") + ", " + Dictionary.GetRandomString("whours"));
                }
            }
        }

        private void CreateCarousels()
        {
            mCarouselHour = new VerticalCarouselInfo();
            mCarouselHour.Text = "Hr";
            //mCarouselHour.Text = Dictionary.GetString("hours");
            mCarouselHour.LowerValue = 0;
            mCarouselHour.UpperValue = 23;
            mCarouselHour.OnScrollChange = iVal => {
                mHour = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };


            mCarouselMinute = new VerticalCarouselInfo();
            mCarouselMinute.Text = "Min";
            //mCarouselMinute.Text = Dictionary.GetString("min") + "s";
            mCarouselMinute.LowerValue = 0;
            mCarouselMinute.UpperValue = 59;
            mCarouselMinute.OnScrollChange = iVal => {
                mMinute = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };


            Toaster.Display<VerticalCarouselToast>().With(mCarouselHour, mCarouselMinute, null, OnValidate, null);
        }

        private void OnValidate()
        {
            string fString = "";

            if (mHour <= 9)
                fString += "0"; 
            fString += mHour + ":";
            if (mMinute <= 9)
                fString += "0";
            fString += mMinute;
            mVocal.AllParam.Add(fString);
            HourOk();
            Toaster.Hide();

        }

        private void HourOk()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
            foreach (string param in mVocal.AllParam)
            {
                Debug.Log("String : " + param);
            }
            ReminderData.Instance.VocalRequest = "";
            //QuitApp();
            Trigger("Rec");
        }

        private bool ExtractHour(string iSpeech)
        {
            string time = "";
            string[] lWord = iSpeech.Split(' ');

            for (int i = 0; i < lWord.Length; i++)
            {
                if (CheckString(lWord[i], "h") && lWord[i].Any(char.IsDigit))
                {
                    string[] lNum = lWord[i].Split('h');

                    if (lNum[0].Length == 1)
                        time += "0";
                    time += lNum[0] + ":";
                    if (lNum[1] != "")
                    {
                        if (lNum[1].Length == 1)
                            time += "0";
                        time += lNum[1];
                    }
                    else
                    {
                        time += "00";
                    }

                }
                else
                {
                    time = TimeSlot(lWord[i]);
                }
                if (time != "")
                {
                    Debug.Log("TIME : " + time);
                    mVocal.AllParam.Add(time);
                    return true;
                }
            }
            if (!Toaster.IsDisplayed)
                ScrollOn = true;
            return false;
        }

        private string TimeSlot(string word)
        {
            string time = "";

            if (word.Equals(Dictionary.GetRandomString("midnight"), StringComparison.OrdinalIgnoreCase))
                time = "00:00";
            else if (word.Equals(Dictionary.GetRandomString("morning"), StringComparison.OrdinalIgnoreCase))
                time = "08:00";
            else if (word.Equals(Dictionary.GetRandomString("noon"), StringComparison.OrdinalIgnoreCase))
                time = "12:00";
            else if (word.Equals(Dictionary.GetRandomString("afternoon"), StringComparison.OrdinalIgnoreCase))
                time = "16:00";
            else if (word.Equals(Dictionary.GetRandomString("evening"), StringComparison.OrdinalIgnoreCase))
                time = "20:00";
            else if (word.Equals(Dictionary.GetRandomString("now"), StringComparison.OrdinalIgnoreCase))
                time = DateTime.Now.Hour + ":" + DateTime.Now.Minute;

            return time;
        }

        private static bool CheckString(string str, string c)
        {
            return str.IndexOf(c, StringComparison.CurrentCultureIgnoreCase) != -1;
        }
        // Update is called once per frame

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Toaster.IsDisplayed)
            {
                Toaster.Hide();
            }
        }
    }
}
