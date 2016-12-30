using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class TimerEnd : AStateMachineBehaviour
    {
        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Say(mDictionary.GetString("iGo"));//("J'y vais");
            mYesHinge.SetPosition(10);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking)
            {
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }


    }
}