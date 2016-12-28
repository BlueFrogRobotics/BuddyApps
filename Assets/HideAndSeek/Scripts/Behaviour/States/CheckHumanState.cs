using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class CheckHumanState : AStateMachineBehaviour
    {

        private HumanDetector mHumanDetector;

        public override void Init()
        {
            mHumanDetector = GetComponent<HumanDetector>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
            
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
                iAnimator.SetBool("IsHuman", mHumanDetector.IsHumanDetected);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //iAnimator.ResetTrigger("ChangeState");
        }

    }
}