using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PSixTrigger : AStateMachineBehaviour
    {
        private float mTimer = -1000F;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Vocal.Say("psixintro", (iOut) => 
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
                            Buddy.Vocal.Say("psixokbuddy");
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
            if(mTimer > 5F)
            {
                Buddy.Vocal.Say("");
                mTimer = -2000F;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            //mTrigger = true;
            //Buddy.Vocal.Say(Buddy.Resources.GetString("seeheart"), (iSpeech) => { TransitionToEnd(); });
        }
    }
}


