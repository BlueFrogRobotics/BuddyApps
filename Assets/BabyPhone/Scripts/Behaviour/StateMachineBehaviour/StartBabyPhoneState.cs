using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

namespace BuddyApp.BabyPhone
{
    public class StartBabyPhoneState : AStateMachineBehaviour
    {

        private GameObject mStartState;
        private GameObject mWindoAppOverBlack;

        public override void Init()
        {
            mStartState = GetGameObject(4);
            mWindoAppOverBlack = GetGameObject(2);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(true);         

            mTTS.Say("BabyPhone");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(false);
            mWindoAppOverBlack.SetActive(false);
            iAnimator.SetInteger("ForwardState", 0);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}
