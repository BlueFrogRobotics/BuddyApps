﻿using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoAnswer : AStateMachineBehaviour
    {
        private int mCount = 0;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCount++;
            if (mCount > 2)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say("Désolé je n'ai rien entendu, veux tu bien répéter ?");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}