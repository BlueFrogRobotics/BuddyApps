using Buddy;
using Buddy.UI;

using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

using Buddy.Command;
using System.Globalization;


namespace BuddyApp.Reminder
{
    public class DateState : AStateMachineBehaviour
    {
        DateTime dt = DateTime.Now;

        VerticalCarouselInfo mCarouselYear;
        VerticalCarouselInfo mCarouselMonth;
        VerticalCarouselInfo mCarouselDay;
        private int mYear = 0;
        private int mMonth = 0;
        private int mDay = 0;

        private bool ScrollOn = false;

        public override void Start()
        {
            BYOS.Instance.Primitive.Speaker.FX.Load(
                   BYOS.Instance.Resources.Load<AudioClip>("Wheel"), 0
               );
        }

        // Use this for initialization
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocal = GetComponent<ReminderBehaviour>();
            mVocal.QuestionTime(Dictionary.GetRandomString("when"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ReminderData.Instance.VocalRequest != "" && mVocal.IsVocalGet)
            {

                Debug.Log("Coucou Hiboux");
                if (ExtractDate(ReminderData.Instance.VocalRequest))
                {
                    DateOk();
                }
                else
                {
                    if (ScrollOn)
                    {
                        Debug.Log("Saperlipopette");
                        CreateCarousels();
                    }
                    ReminderData.Instance.VocalRequest = "";
                    mVocal.QuestionTime(Dictionary.GetRandomString("what") + ", " + Dictionary.GetRandomString("when"));
                }
            }
        }

        private void CreateCarousels()
        {
            mCarouselDay = new VerticalCarouselInfo();
            mCarouselDay.Text = "Day";
            //mCarouselDay.Text = Dictionary.GetString("hours");
            mCarouselDay.LowerValue = 1;
            mCarouselDay.UpperValue = 31;
            mCarouselDay.OnScrollChange = iVal => {
                mDay = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };


            mCarouselMonth = new VerticalCarouselInfo();
            mCarouselMonth.Text = "Mth";
            //mCarouselMonth.Text = Dictionary.GetString("min") + "s";
            mCarouselMonth.LowerValue = 1;
            mCarouselMonth.UpperValue = 12;
            mCarouselMonth.OnScrollChange = iVal => {
                mMonth = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };


            mCarouselYear = new VerticalCarouselInfo();
            mCarouselYear.Text = "Yr";
            //mCarouselYear.Text = Dictionary.GetString("secs");
            mCarouselYear.LowerValue = dt.Year;
            mCarouselYear.UpperValue = 2100;
            mCarouselYear.OnScrollChange = iVal => {
                mYear = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };


            Toaster.Display<VerticalCarouselToast>().With(mCarouselDay, mCarouselMonth, mCarouselYear, OnValidate, null);
           
        }

        private void OnValidate()
        {
            mVocal.AllParam.Add(new DateTime(mYear, mMonth, mDay).Date.ToString("dd/MM/yyyy"));
            Toaster.Hide();
            DateOk();
            //Trigger("CountDown");

        }

        private void DateOk()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
            foreach (string param in mVocal.AllParam)
            {
                Debug.Log("String : " + param);
            }
            ReminderData.Instance.VocalRequest = "";
            Trigger("Hour");
        }

        private bool ExtractDate(string iSpeech)
        {
            Debug.Log("ISPEECH + " + iSpeech);
            Debug.Log("Alors ? " + Dictionary.GetPhoneticStrings("tomorrow")[0]);
            if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("today")))
            {
                mVocal.AllParam.Add(dt.Date.ToString("dd/MM/yyyy"));
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("dayaftertomorrow")))
            {
                dt = dt.AddDays(2);
                mVocal.AllParam.Add(dt.Date.ToString("dd/MM/yyyy"));
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("tomorrow")))
            {
                Debug.Log("Wtf Olivier ?");
                dt = dt.AddDays(1);
                mVocal.AllParam.Add(dt.Date.ToString("dd/MM/yyyy"));
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("intime")) && ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("day")))
            {
                int nbDay = 0;
                string[] words = iSpeech.Split(' ');
                for (int iw = 0; iw < words.Length; ++iw)
                {
                    if (words[iw].ToLower() == Dictionary.GetString("intime") && iw + 2 < words.Length)
                    {
                        if (Int32.TryParse(words[iw + 1], out nbDay) && ContainsOneOf(words[iw + 2], Dictionary.GetPhoneticStrings("day")))
                        {
                            Debug.Log("contains in days: " + words[iw + 2]);

                            dt = dt.AddDays(nbDay);
                            mVocal.AllParam.Add(dt.Date.ToString("dd/MM/yyyy"));
                            break;
                        }
                    }
                }
            }
            else if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("weekday")) || ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("month")))
            {
                int n;
                string lDay = "Monday";
                int lMonth = 0;
                int lNum = 0;

                if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("weekday")))
                {
                    foreach (string day in Dictionary.GetPhoneticStrings("weekday"))
                    {
                        if (ContainsOneOf(day, iSpeech.Split(' ')))
                        {
                            lDay = day;
                            break;
                        }
                    }
                }
                if (ContainsOneOf(iSpeech, Dictionary.GetPhoneticStrings("month")))
                {
                    foreach (string month in Dictionary.GetPhoneticStrings("month"))
                    {
                        if (ContainsOneOf(month, iSpeech.Split(' ')))
                        {
                            lMonth = FindMonth(month);
                            break;
                        }
                    }

                    foreach (string word in iSpeech.Split(' '))
                    {
                        if (int.TryParse(word, out n))
                        {
                            lNum = n;
                            break;
                        }
                    }
                    if (lNum == 0)
                        lNum = dt.Day;
                    mVocal.AllParam.Add(new DateTime(dt.Year, lMonth, lNum).Date.ToString("dd/MM/yyyy"));
                }
                else
                {
                    while (!(dt.ToString("dddd", new CultureInfo("fr-Fr")).Equals(lDay, StringComparison.OrdinalIgnoreCase)))
                    {
                        dt = dt.AddDays(1);
                    }
                    mVocal.AllParam.Add(dt.Date.ToString());
                }
                if ((lNum == 0 && lMonth != 0) || (lMonth == 2 && lNum > 28) || lNum > 31)
                    return false;

            }

            if (mVocal.AllParam.Count > 1)
                return true;
            if (!Toaster.IsDisplayed)
                ScrollOn = true;
            return false;
        }

        private int FindMonth(string month)
        {
            int lMonth = 1;

            if ( month.Equals("Janvier", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 1;
            }
            else if (month.Equals("Février", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 2;

            }
            else if (month.Equals("Mars", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 3;

            }
            else if (month.Equals("Avril", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 4;

            }
            else if (month.Equals("Mai", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 5;

            }
            else if (month.Equals("Juin", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 6;

            }
            else if (month.Equals("Juillet", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 7;

            }
            else if (month.Equals("Août", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 8;

            }
            else if (month.Equals("Septembre", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 9;

            }
            else if (month.Equals("Octobre", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 10;

            }
            else if (month.Equals("Novembre", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 11;

            }
            else if (month.Equals("Décembre", StringComparison.OrdinalIgnoreCase))
            {
                lMonth = 12;

            }
            return (lMonth);
        }

        private bool ContainsOneOf(string iSpeech, string[] iListSpeech)
        {
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

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Toaster.IsDisplayed)
            {
                Toaster.Hide();
            }
        }
    }
}