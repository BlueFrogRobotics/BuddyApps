using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Korian
{

    public class Signalement : AStateMachineBehaviour
    {
        private int mNumberTry;
        private float mTimer;
        private float mTimerHelp;
        private int mRandom;
        private bool mIsSent;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mIsSent = false;
            mNumberTry = 0;
            mTimerHelp = 0;
            mTimer = 0F;
            Buddy.Behaviour.SetMood(Mood.SCARED, false);
            if (KorianData.Instance.Mail == KorianData.MailType.MAILA)
                SendMail(Buddy.Resources.GetRandomString("maila"));
            else if (KorianData.Instance.Mail == KorianData.MailType.MAILB)
                SendMail(Buddy.Resources.GetRandomString("mailb"));
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            mTimerHelp += Time.deltaTime;
            if (!mIsSent && mNumberTry < 3 && mTimer < 2F)
            {
                mTimer = 0F;
                if (!mIsSent && KorianData.Instance.Mail == KorianData.MailType.MAILA)
                    SendMail(Buddy.Resources.GetRandomString("maila"));
                else if (!mIsSent && KorianData.Instance.Mail == KorianData.MailType.MAILB)
                    SendMail(Buddy.Resources.GetRandomString("mailb"));
            }

            //Faire Tracking en continu

            if(!Buddy.Vocal.IsBusy && mTimerHelp > 5F)
            {
                mTimerHelp = 0F;
                int mRandom = Random.Range(0, 3);
                if (mRandom == 0)
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("helpiscoming"));
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL, false);
        }

        private void SendMail(string iMessage)
        {
            EMail lMail = new EMail();
            lMail.Addresses.Clear();
            lMail.Addresses.Add("mc@bluefrogrobotics.com");//ADRESSE KORIAN
            lMail.Subject = /*string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;*/ "SUBJECT MAIL";
            lMail.Body = /*string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;*/ iMessage;

            Buddy.WebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnEndSending);
        }

        private void OnEndSending(bool iSuccess)
        {
            if (iSuccess)
            {
                mIsSent = true;
                Debug.Log("SUCCESS");
            }
            else
            {
                Debug.Log("FAIL");
                mNumberTry++;
                if (mNumberTry < 3)
                {
                    Buddy.Vocal.SayKey("cantsendmessage");
                }
                else
                {
                    //Alarme + LED
                }
            }
                
        }

    }

}