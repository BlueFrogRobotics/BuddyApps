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

        public override void Init()
        {
            //mButonValidate = GetComponent<Players>().ButtonValidate;
           // mButonValidate.onClick.AddListener(Validate);
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Say("Montrez moi vos visages");
            //GetGameObject(2).SetActive(true);
            mAnimator = iAnimator;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if(mTTS.HasFinishedTalking())
                mAnimator.SetTrigger("ChangeState");
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