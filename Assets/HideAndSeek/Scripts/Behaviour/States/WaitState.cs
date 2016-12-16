using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class WaitState : AStateMachineBehaviour
    {
        private float mTimer = 0.0f;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(mTimer>15.0f)
                iAnimator.SetTrigger("ChangeState");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}