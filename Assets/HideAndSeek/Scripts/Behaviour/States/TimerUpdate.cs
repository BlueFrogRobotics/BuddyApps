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

        public override void Init()
        {
            mTimer = 0.0f;
            mNumPrec = 0;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Init();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            int lNumAct = Mathf.FloorToInt(mTimer);
            if(lNumAct>mNumPrec)
            {
                mNumPrec = lNumAct;
                mTTS.Say(""+lNumAct);
            }
            if(lNumAct>2)
            {
                
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mYesHinge.SetPosition(0);
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}