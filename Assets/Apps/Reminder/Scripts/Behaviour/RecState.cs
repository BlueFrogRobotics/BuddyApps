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
    public class RecState : AStateMachineBehaviour
    {

        VerticalCarouselInfo mCarouselRec;
        private int mRec = 0;
        bool ScrollOn = false;

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mVocal = GetComponent<ReminderBehaviour>();
            mVocal.QuestionTime(Dictionary.GetRandomString("recurrence"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ReminderData.Instance.VocalRequest != "" && mVocal.IsVocalGet)
            {
                Debug.Log("Coucou Hiboux");
                if (ExtractRec(ReminderData.Instance.VocalRequest))
                {
                    RecOk();
                }
                else
                {
                    if (ScrollOn)
                    {
                        Debug.Log("Saperlipopette");
                        CreateCarousels();
                    }
                    ReminderData.Instance.VocalRequest = "";
                    mVocal.QuestionTime(Dictionary.GetRandomString("what"));
                }
            }
        }

        private bool ExtractRec(string iSpeech)
        {
            string rec = "";

            iSpeech = iSpeech.ToLower();
            
            if (iSpeech.Contains(Dictionary.GetRandomString("no")))
            {
                rec = "0";
            }
            else if (iSpeech.Contains(Dictionary.GetRandomString("day")))
            {
                rec = "1";
            }
            else if (iSpeech.Contains(Dictionary.GetRandomString("week")))
            {
                rec = "2";
            }
            else if (iSpeech.Contains(Dictionary.GetRandomString("month")))
            {
                rec = "3";
            }

            if (rec != "")
            {
                Debug.Log("ReC : " + rec);
                mVocal.AllParam.Add(rec);
                return true;
            }
            if (!Toaster.IsDisplayed)
                ScrollOn = true;
            return false;
        }

        private void CreateCarousels()
        {
            mCarouselRec = new VerticalCarouselInfo();
            mCarouselRec.Text = "Days";
            //mCarouselRec.Text = Dictionary.GetString("hours");
            mCarouselRec.LowerValue = 0;
            mCarouselRec.UpperValue = 3;
            mCarouselRec.OnScrollChange = iVal => {
                mRec = iVal;
                if (BYOS.Instance.Primitive.Speaker.FX.Status != SoundChannelStatus.PLAYING)
                    BYOS.Instance.Primitive.Speaker.FX.Play(0);
            };

            Toaster.Display<VerticalCarouselToast>().With(mCarouselRec, null, null, OnValidate, null);
        }

        private void OnValidate()
        {
            mVocal.AllParam.Add("" + mRec);
            Toaster.Hide();
            RecOk();

        }

        private void RecOk()
        {
            BYOS.Instance.Interaction.TextToSpeech.Say("Ok");
            foreach (string param in mVocal.AllParam)
            {
                Debug.Log("String : " + param);
            }
            ReminderData.Instance.VocalRequest = "";
            Trigger("Record");
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}