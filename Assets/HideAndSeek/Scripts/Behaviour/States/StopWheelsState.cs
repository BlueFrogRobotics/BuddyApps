using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class StopWheelsState : AStateMachineBehaviour
    {
        private float mTimer = 0.0f;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWheels.StopWheels();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > 1.0f)
                iAnimator.SetTrigger("ChangeState");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}