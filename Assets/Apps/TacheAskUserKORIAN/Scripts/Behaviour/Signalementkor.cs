using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TacheAskUserKORIAN
{

    public class Signalementkor : AStateMachineBehaviour
    {
        private int mNumberTry;
        private float mTimer;
        private float mTimerHelp;
        private int mRandom;
        

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
            mNumberTry = 0;
            mTimerHelp = 0;
            mTimer = 0F;
            Buddy.Behaviour.Mood = Mood.SCARED;
            if (TacheAskUserKORIANData.Instance.Mail == TacheAskUserKORIANData.MailType.MAILA)
                SendMail("mailA");
            else if (TacheAskUserKORIANData.Instance.Mail == TacheAskUserKORIANData.MailType.MAILB)
                SendMail("mailB");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            mTimerHelp += Time.deltaTime;
            if(mNumberTry < 3 && mTimer < 10F)
            {
                if (TacheAskUserKORIANData.Instance.Mail == TacheAskUserKORIANData.MailType.MAILA)
                    SendMail("MailA");
            }
            //Faire Tracking en continu

            if(!Buddy.Vocal.IsBusy && mTimerHelp > 5F)
            {
                mTimerHelp = 0F;
                int mRandom = Random.Range(0, 4);
                if (mRandom == 0)
                    Buddy.Vocal.Say("helpiscoming");
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.Mood = Mood.NEUTRAL;
        }

        private void SendMail(string iMessage)
        {
            EMail lMail = new EMail();
            lMail.Addresses.Clear();
            lMail.Addresses.Add("mc@bluefrogrobotics.com");//ADRESSE KORIAN
            lMail.Subject = /*string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;*/ "SUBJECT MAIL";
            lMail.Body = /*string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;*/ "BODY MAIL";

            Buddy.WebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnEndSending);
        }

        private void OnEndSending(bool iSuccess)
        {
            if (iSuccess)
                Debug.Log("SUCCESS");
            else
            {
                Debug.Log("FAIL");
                mNumberTry++;
                if(mNumberTry < 3)
                {
                    Buddy.Vocal.Say("cantsendmessage");
                    
                }
                else
                {
                    //Alarme + LED
                }
            }
                
        }

    }

}