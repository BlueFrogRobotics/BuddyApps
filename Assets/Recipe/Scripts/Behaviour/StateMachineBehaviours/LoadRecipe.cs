using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class Loadrecipe : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mTTS.IsSpeaking())
                mTTS.Stop();
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetGameObject(2).SetActive(true);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WCategory");
        }
    }
}