using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class TimerUpdate : AStateMachineBehaviour
    {

        private float mTimer;
        private int mNumPrec = 0;
        private bool mHasFinished = false;

        public override void Init()
        {
            mTimer = 0.0f;
            mNumPrec = 0;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Init();
            mHasFinished = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            int lNumAct = Mathf.FloorToInt(mTimer);
            if(lNumAct>mNumPrec && lNumAct<11)
            {
                mNumPrec = lNumAct;
                mTTS.Say(""+lNumAct);
            }
            if(lNumAct>9 && !mTTS.IsSpeaking && !mHasFinished)
            {
                mHasFinished = true;
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mYesHinge.SetPosition(20);
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}