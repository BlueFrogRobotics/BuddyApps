using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class WinState : AStateMachineBehaviour
    {
        private bool mHasTalked = false;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mHasTalked = false;
            mFace.SetExpression(MoodType.HAPPY);
            
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mHasTalked && mTTS.HasFinishedTalking)
            {
                mTTS.Say("J'ai gagné");
                mHasTalked = true;
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

    }
}