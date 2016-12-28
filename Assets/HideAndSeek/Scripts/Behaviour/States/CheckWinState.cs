using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class CheckWinState : AStateMachineBehaviour
    {
        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GetComponent<Players>().NumPlayer>0)
            {
                iAnimator.SetTrigger("ChangeState");
            }

            else
                iAnimator.SetTrigger("Win");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
            iAnimator.ResetTrigger("Win");
        }

    }
}