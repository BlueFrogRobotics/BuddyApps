using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

namespace BuddyApp.BabyPhone
{
    public class StartBabyPhone : AStateMachineBehaviour
    {

        private GameObject mStartState;

        public override void Init()
        {
            mStartState = GetGameObject(1);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(true);
            mTTS.Say("Explication concernant lutilisation de lapplication");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(false);
            iAnimator.SetBool("StartApp", false);
            iAnimator.SetFloat("ForwardState", 0);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}
