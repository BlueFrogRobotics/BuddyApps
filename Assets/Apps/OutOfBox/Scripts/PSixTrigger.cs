using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PSixTrigger : AStateMachineBehaviour
    {
        private float mTimer = -1000F;
        private bool mTransitionEnd;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTransitionEnd = false;
            Buddy.Vocal.SayKey("psixintro", (iOut) => 
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
                {
                    iOnBuild.CreateWidget<TText>().SetLabel("Ok Buddy");
                }, () => 
                {
                    StartCoroutine(OutOfBoxUtils.WaitTimeAsync(2F, () =>
                    {
                        Buddy.GUI.Toaster.Hide();
                        
                    }));
                }, () => 
                {
                    Buddy.Vocal.Say("psixunderstand", (iSpeechOut) =>
                    {
                        StartCoroutine(OutOfBoxUtils.WaitTimeAsync(1F, () => 
                        {
                            Buddy.Vocal.SayKey("psixokbuddy");
                            Buddy.Vocal.EnableTrigger = true;
                            Buddy.Vocal.ListenOnTrigger = true;
                            Buddy.Vocal.OnTrigger.Add(BuddyTrigged);
                            mTimer = 0F;
                        }));
                    });
                });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if(mTimer > 7F && !mTransitionEnd)
            {
                Buddy.Vocal.SayKey("psixmouth", (iOut) => TransitionToEnd());
                mTransitionEnd = true;
            }
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            mTransitionEnd = true;
            Buddy.Vocal.Say(Buddy.Resources.GetString("psixseeheart"), (iSpeech) => { TransitionToEnd(); });
        }

        private void TransitionToEnd()
        {
            Buddy.Vocal.SayKey("psixask", (iOut) =>
            {
                Buddy.Vocal.Listen((iListen) => {
                    OutOfBoxUtils.PlayBIAsync(() => Buddy.Vocal.Say(Buddy.Resources.GetString("psixthanks"), (iSpeech) =>
                    {
                        //Launch diagnostic
                        QuitApp();
                    }));
                });
                
            });
        }
    }
}


