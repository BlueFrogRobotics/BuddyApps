using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Korian
{
    
    public class AskUser : AStateMachineBehaviour
    {
        //TODO : Tracking du user afin de rester face à lui
        private int mNumberListen;
        private float mTimeGiveUp;

        private float mTimer;
        private bool mUserSaidNo;
        private int mNumberTryListen;

        public override void Start()
        {
            KorianData.Instance.Mail = KorianData.MailType.NONE;
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
            if (Buddy.Actuators.Wheels.IsBusy)
                Buddy.Actuators.Wheels.Stop();
            if (Buddy.Navigation.IsBusy)
                Buddy.Navigation.Stop();

            mNumberListen = 0;
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetRandomString("doyouneedhelp"));
            },
                () => {
                    ExtLog.I(ExtLogModule.APP, typeof(AskUser), LogStatus.INFO, LogInfo.RUNNING, "Click no");
                    Buddy.GUI.Toaster.Hide();
                    Buddy.Vocal.StopListening();
                    Buddy.Vocal.Say("REGLE DE LEHPAD " + Buddy.Resources.GetRandomString("gobacktosleep"), (iOnEndSpeech)=> { mTimer = 0F; });
                    mUserSaidNo = true;
                },
                "no"
               /* Buddy.Resources.GetString("no")*/,
                
                () => {
                    ExtLog.I(ExtLogModule.APP, typeof(AskUser), LogStatus.INFO, LogInfo.RUNNING, "Click yes");
                    Buddy.Vocal.StopListening();
                    Buddy.GUI.Toaster.Hide();
                    KorianData.Instance.Mail = KorianData.MailType.MAILA;
                    Buddy.Vocal.Say(Buddy.Resources.GetRandomString("helpiscoming"));
                    Trigger("Reporting");
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
            if(!Buddy.Vocal.IsSpeaking && mUserSaidNo && mTimer > (mTimeGiveUp * 1.5F))
            {
                Trigger("Scan"); 
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mUserSaidNo = false;
        }


        /// <summary>
        /// This function is called when an answer is received from the user
        /// </summary>
        /// <param name="iInput">User speech input</param>
        private void OnEndListen(SpeechInput iInput)
        {
            if (iInput.IsInterrupted)
                return;
            if (Utils.GetRealStartRule(iInput.Rule) == "yes")
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.Vocal.StopListening();
                KorianData.Instance.Mail = KorianData.MailType.MAILA;
                Buddy.Vocal.Say(Buddy.Resources.GetRandomString("helpiscoming"), iInputlol => { OnEndSpeaking(iInputlol); });
            }
            else if (Utils.GetRealStartRule(iInput.Rule) == "no")
            {
                Buddy.GUI.Toaster.Hide();
                Buddy.Vocal.StopListening();
                Buddy.Vocal.Say("VOICI LES REGLE DE LEHPAD [200] " + Buddy.Resources.GetRandomString("gobacktosleep"));
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
                    KorianData.Instance.Mail = KorianData.MailType.MAILB;
                    Trigger("Reporting");
                }
            }
        }

        private void OnEndSpeaking(SpeechOutput iOutput)
        {
            Trigger("Reporting");
        }
    }
}

