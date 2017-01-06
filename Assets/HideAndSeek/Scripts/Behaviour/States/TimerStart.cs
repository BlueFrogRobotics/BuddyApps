﻿using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class TimerStart : AStateMachineBehaviour
    {
        public override void Init()
        {
            
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mMood.Set(MoodType.NEUTRAL);
            GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>().SetAppBlack();
            mTTS.Say(mDictionary.GetString("willCount"));//("Je vais compter jusqu a 10 et je vous cherche");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(mTTS.HasFinishedTalking)
            {
                mYesHinge.SetPosition(40);
                //Debug.Log("angle max: " + mYesHinge.MaximumAngle);
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }

    }
}