using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.BabyPhone
{
    public class CountState : AStateMachineBehaviour
    {
        private GameObject mCounter;
        private GameObject mWindoAppOverWithe;
        private GameObject mBlackground;
        private Animator mBackgroundBlackAnimator;
        private Animator mCounterAnimator;


        public override void Init()
        {
            mCounter = GetGameObject(8);
            mWindoAppOverWithe = GetGameObject(3);
            mCounterAnimator = mCounter.GetComponent<Animator>();
            mBlackground = GetGameObject(1);
            mBackgroundBlackAnimator = mBlackground.GetComponent<Animator>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //open black background only if it is not already open
            if (iAnimator.GetInteger("ForwardState") > -1 )
                mBackgroundBlackAnimator.SetTrigger("Open_BG");

            mWindoAppOverWithe.SetActive(true);

            //start counter
            mCounter.SetActive(true);
            mCounterAnimator.SetTrigger("Open_WTimer");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCounter.SetActive(false);
            //mWindoAppOverWithe.SetActive(false);
            mCounterAnimator.SetTrigger("Close_WTimer");
            //mBackgroundBlackAnimator.SetTrigger("Close_BG");
            iAnimator.SetInteger("ForwardState", 22);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}
