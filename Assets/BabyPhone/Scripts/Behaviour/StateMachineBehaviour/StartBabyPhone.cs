using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;
using System;

namespace BuddyApp.BabyPhone
{
    public class StartBabyPhone : AStateMachineBehaviour
    {
        private TextToSpeech mTTS;
        private GameObject mStartState;

        public override void Init()
        {
            mStartState = GetGameObject(2);
            mTTS = BYOS.Instance.TextToSpeech;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(true);
            mTTS.Say("Buddy prend soit de bébé");

        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mStartState.SetActive(false);
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}
