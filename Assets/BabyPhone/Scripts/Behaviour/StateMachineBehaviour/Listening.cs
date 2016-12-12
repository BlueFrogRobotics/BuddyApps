using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.BabyPhone
{
    public class Listening : AStateMachineBehaviour
    {
        private InputMicro mMicro;

        public override void Init()
        {
            mMicro = GetGameObject(2).GetComponent<InputMicro>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}
