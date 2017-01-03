using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class WaitState : AStateMachineBehaviour
    {
        [SerializeField]
        private float timerLenght = 2.0f;

        private float mTimer = 0.0f;


        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer = 0.0f;
            if (!mRGBCam.IsOpen)
                mRGBCam.Open();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(mTimer>timerLenght)
                iAnimator.SetTrigger("ChangeState");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}