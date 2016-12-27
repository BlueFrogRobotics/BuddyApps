using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BuddyOS.App;
using System;

namespace BuddyApp.HideAndSeek
{
    public class SetPlayers : AStateMachineBehaviour
    {
        private Button mButonValidate;
        private Animator mAnimator;
        private float mTimer = 0.0f;
        private bool mHasFinished = false;

        public override void Init()
        {
            //mButonValidate = GetComponent<Players>().ButtonValidate;
           // mButonValidate.onClick.AddListener(Validate);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Say("Jouons à cache-cache");
            //GetGameObject(2).SetActive(true);
            mAnimator = iAnimator;
            mTimer = 0.0f;
            mHasFinished = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTTS.HasFinishedTalking && mTimer > 3.0f && !mHasFinished)
            {
                mAnimator.SetTrigger("ChangeState");
                mHasFinished = true;
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mButonValidate.onClick.RemoveAllListeners();
            iAnimator.ResetTrigger("ChangeState");
        }


        private void Validate()
        {
            GetGameObject(2).SetActive(false);
            mAnimator.SetTrigger("ChangeState");
        }
    }
}