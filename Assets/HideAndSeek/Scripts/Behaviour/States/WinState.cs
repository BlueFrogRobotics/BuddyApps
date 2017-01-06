﻿using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class WinState : AStateMachineBehaviour
    {
        private bool mHasLaughed = false;
        private bool mHasTalked = false;
        float mTimer = 0.0f;

        public override void Init()
        {

        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mHasTalked = false;
            mHasLaughed = false;
            mMood.Set(MoodType.HAPPY);
            mTimer = 0.0f;
            //mFace.SetExpression(MoodType.HAPPY);

        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;

            if (!mHasLaughed && mTTS.HasFinishedTalking)
            {
                mSpeaker.Voice.Play(BuddyOS.VoiceSound.LAUGH_5);
                
                mHasLaughed = true;
                
            }

            else if(mSpeaker.Voice.Status != SoundChannelStatus.PLAYING && mHasLaughed && !mHasTalked)
            {
                mHasTalked = true;
                mTTS.Say(mDictionary.GetString("win"));//"J'ai gagné");
                mTimer = 0.0f;
            }
            else if(mHasTalked && mTimer>2.5f && mTTS.HasFinishedTalking)
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