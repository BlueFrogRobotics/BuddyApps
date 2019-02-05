using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.TacheAskUserKORIAN
{
    
    public class AskUserkor : AStateMachineBehaviour
    {
        //TODO : Tracking du user afin de rester face à lui
        private int mNumberListen;
        private float mTimeGiveUp;

        private float mTimer;
        private bool mUserSaidNo;
        private int mNumberTryListen;

        public override void Start()
        {
            TacheAskUserKORIANData.Instance.Mail = TacheAskUserKORIANData.MailType.NONE;
            mTimeGiveUp = 3;
            mTimer = 0F;
            mUserSaidNo = false;
            //We need Buddy to listen at least once, if the developer forgot to enter the number of listen we initialize it at 1.
            if (mNumberTryListen == 0)
                mNumberTryListen = 1;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

            mNumberListen = 0;
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetRandomString("doyouneedhelp"));
            },
                () => {
                    ExtLog.I(ExtLogModule.APP, typeof(AskUser), LogStatus.INFO, LogInfo.RUNNING, "Click no");
                    Buddy.GUI.Toaster.Hide();
                    Buddy.Vocal.StopListening();
                    Buddy.Vocal.Say("REGLE DE LEHPAD " + Buddy.Resources.GetRandomString("gobacktosleep"));
                    mUserSaidNo = true;
                },
                "no"
               /* Buddy.Resources.GetString("no")*/,
                
                () => {
                    ExtLog.I(ExtLogModule.APP, typeof(AskUser), LogStatus.INFO, LogInfo.RUNNING, "Click yes");
                    Buddy.Vocal.StopListening();
                    Buddy.GUI.Toaster.Hide();
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("helpiscoming"));
                    Trigger("SIGNALEMENT");
                },
                "yes"
                //Buddy.Resources.GetString("yes")

            );


            Buddy.Vocal.SayAndListen(
                Buddy.Resources.GetString("doyouneedhelp"),
                null,
                (iInput) => { OnEndListen(iInput); });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if(!Buddy.Vocal.IsSpeaking && mUserSaidNo && mTimer > mTimeGiveUp)
            {
                Trigger("RETOUR PATROUILLE"); 
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mUserSaidNo = false;
        }
         
        //private void OnEndSending(bool iSuccess)
        //{
        //    if (iSuccess)
        //        Debug.Log("SUCCESS");
        //    else
        //        Debug.Log("FAIL");
        //}

        /// <summary>
        /// This function is called when an answer is received from the user
        /// </summary>
        /// <param name="iInput">User speech input</param>
        private void OnEndListen(SpeechInput iInput)
        {
            if (iInput.IsInterrupted)
                return;
            if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "yes"))
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.Vocal.StopListening();
                Buddy.Vocal.Say(Buddy.Resources.GetRandomString("helpiscoming"), iInputlol => { OnEndSpeaking(iInputlol); });
                TacheAskUserKORIANData.Instance.Mail = TacheAskUserKORIANData.MailType.MAILA;
            }
            else if (Utils.ContainsOneOf(Buddy.Vocal.LastHeardInput.Utterance, "no"))
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.Vocal.StopListening();
                Buddy.Vocal.Say("REGLE DE LEHPAD " + Buddy.Resources.GetRandomString("gobacktosleep"));
                mUserSaidNo = true;
            }
            else
            {
                if (mNumberListen < mNumberTryListen)
                {
                    mNumberListen++;
                    Buddy.Vocal.Listen(
                        iInputRec => { OnEndListen(iInputRec); }
                        );
                }
                else
                {
                    Buddy.GUI.Toaster.Hide();
                    TacheAskUserKORIANData.Instance.Mail = TacheAskUserKORIANData.MailType.MAILB;
                    Trigger("SIGNALEMENT");
                    //SendMail("mailB");
                }
            }
        }

        //private void SendMail(string iMessage)
        //{
        //    EMail lMail = new EMail();
        //    lMail.Addresses.Clear();
        //    lMail.Addresses.Add("mc@bluefrogrobotics.com");//ADRESSE KORIAN
        //    lMail.Subject = /*string.IsNullOrEmpty(mXMLData.SubjectMail) ? Buddy.Resources.GetRandomString(STR_MAIL_SUBJECT) : mXMLData.SubjectMail;*/ "SUBJECT MAIL";
        //    lMail.Body = /*string.IsNullOrEmpty(mXMLData.BodyMail) ? Buddy.Resources.GetRandomString(STR_MAIL_TEXT) : mXMLData.BodyMail;*/ "BODY MAIL";

        //    Buddy.WebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnEndSending);
        //}

        private void OnEndSpeaking(SpeechOutput iOutput)
        {
            Trigger("SIGNALEMENT");
        }
    }
}

