using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class DetectMouvState : AStateMachineBehaviour
    {

        private MovementDetector mMovDetector;

        public override void Init()
        {
            mMovDetector = GetComponent<MovementDetector>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {


        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.SetInteger("MovingDetect", (int)mMovDetector.DirectionMov);
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //iAnimator.ResetTrigger("ChangeState");
        }

    }
}