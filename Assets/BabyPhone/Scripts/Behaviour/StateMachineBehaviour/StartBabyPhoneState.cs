using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

namespace BuddyApp.BabyPhone
{
    public class StartBabyPhoneState : AStateMachineBehaviour
    {
        private GameObject mStartState;
        private GameObject mWindoAppOverBlack;
        private BabyPhoneData mBabyPhoneData;

        public override void Init()
        {
            mStartState = GetGameObject(4);
            mWindoAppOverBlack = GetGameObject(2);
            mDictionary = BYOS.Instance.Dictionary;
            mBabyPhoneData = BabyPhoneData.Instance;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(true);         

            mTTS.Say(mDictionary.GetString("bbwelcome"));
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(false);
            mWindoAppOverBlack.SetActive(false);
            iAnimator.SetInteger("ForwardState", 0);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(mTTS.HasFinishedTalking)
            {
                bool lDoSetParam = mBabyPhoneData.DoSaveSetting;
                if (!lDoSetParam)
                    iAnimator.SetTrigger("SetParameters");
                else
                    iAnimator.SetTrigger("HeadAdjust");
            }
       
        }
    }
}
