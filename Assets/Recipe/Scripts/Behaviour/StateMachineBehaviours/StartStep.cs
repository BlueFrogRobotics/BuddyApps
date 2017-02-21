using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class StartStep : AStateMachineBehaviour
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTTS.Say(mDictionary.GetRandomString("startstep") + " " + "[300]");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking)
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
            GetGameObject(2).SetActive(false);
            GetGameObject(1).SetActive(true);
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
        }
    }
}