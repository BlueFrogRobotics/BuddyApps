using BlueQuark;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.HelloWorld
{
    public sealed class LearnToTrigger : AStateMachineBehaviour
    {
        private const float TRIGGER_TIMEOUT = 5F;

        private const float SPEECH_TIMEOUT = 5F;

        private float mTrigTimeStamp;

        private float mSpeechTimeStamp;

        private bool mTrigger;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Behaviour.ResetMood();
            mTrigger = false;
            mTrigTimeStamp = -1000F;
            Buddy.Vocal.Say(Buddy.Resources.GetString("magicword"), (iSpeechOutput) =>
            {
                Buddy.GUI.Toaster.Display<ParameterToast>().With((iOnBuild) =>
                {
                    iOnBuild.CreateWidget<TText>().SetLabel("Ok Buddy");
                }, () =>
                {
                    // OnDisplay
                    StartCoroutine(HelloWorldUtils.WaitTimeAsync(2F, () => { Buddy.GUI.Toaster.Hide(); }));
                }, () =>
                {
                    // OnHide 
                    Buddy.Vocal.Say(Buddy.Resources.GetString("understand") + "," + Buddy.Resources.GetString("sayokbuddy"), (iOut) =>
                    {
                        Buddy.Vocal.EnableTrigger = true;
                        Buddy.Vocal.ListenOnTrigger = true;
                        Buddy.Vocal.OnTrigger.Add(BuddyTrigged);
                        mTrigTimeStamp = Time.time;
                    });
                });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mTrigger)
                return;

            if (mTrigTimeStamp > 0 && (Time.time - mTrigTimeStamp) > TRIGGER_TIMEOUT) {
                Buddy.Vocal.Say(Buddy.Resources.GetString("mouth"), (iSpeech) => { TransitionToEnd(); });
                mTrigTimeStamp = -1000F;
            }
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            mTrigger = true;
            Buddy.Vocal.Say(Buddy.Resources.GetString("seeheart"), (iSpeech) => { TransitionToEnd(); });
        }

        private void TransitionToEnd()
        {
            string lFinalSpeech = Buddy.Resources.GetString("askme") + "[100]"
                + Buddy.Resources.GetString("time") + "[100]" + Buddy.Resources.GetString("day") + "[100]" + Buddy.Resources.GetString("math");
            Buddy.Vocal.Say(lFinalSpeech, (iSpeechOutput) =>
            {
                Buddy.Vocal.Say(Buddy.Resources.GetString("end"), (iSpeech) => { QuitApp(); });
            });
        }
    }
}
