using UnityEngine;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class LoadingState : AStateMachineBehaviour
    {
        private GameObject mWindowLoading;
        private float mTimer = 0.0f;

        public override void Init()
        {
            mWindowLoading = GetGameObject((int)HideAndSeekData.ObjectsLinked.LOADING);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mWindowLoading.GetComponent<Animator>().SetTrigger("Open_WLoading");
            mRGBCam.Resolution = RGBCamResolution.W_176_H_144;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer>6.0f)
            {
                if (mRGBCam.IsOpen)
                    mRGBCam.Close();
                mWindowLoading.GetComponent<Animator>().SetTrigger("Close_WLoading");
                iAnimator.SetTrigger("ChangeState");
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            iAnimator.ResetTrigger("ChangeState");
        }


    }
}