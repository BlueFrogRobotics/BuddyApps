using UnityEngine.UI;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    public class StartTimerState : AStateMachineBehaviour
    {
        const int START_TIMER = 5;

        private bool mStartTimer;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

			//BYOS.Instance.Header.DisplayParameters = true;
			Interaction.TextToSpeech.SayKey("startdetectiontimer");
            mStartTimer = false;
            //Detection.SoundDetector.StartMic();
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!Interaction.TextToSpeech.HasFinishedTalking || mStartTimer)
                return;

            BYOS.Instance.Toaster.Display<CountdownToast>().With(
                BYOS.Instance.Dictionary.GetString("startdetectiontimer"), START_TIMER,
                () => { Trigger("InitDetection"); },
                () => { Trigger("InitDetection"); },
                () => { Trigger("Cancel"); });

            mStartTimer = true;
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartTimer = false;
        }
    }
}