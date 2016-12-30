using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class NoRecipeFound : AStateMachineBehaviour
    {

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GetComponent<RecipeBehaviour>().IsBackgroundActivated) {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
                GetGameObject(2).SetActive(true);
                GetGameObject(1).SetActive(false);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            }
            mTTS.Say(mDictionary.GetString("nomatchingrecipe"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking)
                iAnimator.SetTrigger("AskCategoryAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}