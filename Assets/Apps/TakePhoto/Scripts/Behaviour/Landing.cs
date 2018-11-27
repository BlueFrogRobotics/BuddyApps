using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;


namespace BuddyApp.TakePhoto
{
    public sealed class Landing : AStateMachineBehaviour
    {
        private float mTimer;

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0F;
            Buddy.Vocal.SayKey("movehands");
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters();
            Buddy.Vocal.DefaultInputParameters.RecognitionThreshold = 6000;

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(!Buddy.Vocal.IsBusy || mTimer > 6F)
                Trigger("LookForUser");
        }

    }
}