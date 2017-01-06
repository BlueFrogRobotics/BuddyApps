using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class QuitState : AStateMachineBehaviour
    {

        private WindowLinker mWindowLinker;
        private bool mHasExit = false;

        public override void Init()
        {
            mWindowLinker = GetGameObject((int)HideAndSeekData.ObjectsLinked.WINDOW_LINKER).GetComponent<WindowLinker>();
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mHasExit && !mTTS.IsSpeaking)
            {
                mHasExit = true;
                mWindowLinker.QuitApplication();
            }
        }


        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }


    }
}