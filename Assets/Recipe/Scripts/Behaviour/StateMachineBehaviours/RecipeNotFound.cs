using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class RecipeNotFound : AStateMachineBehaviour
    {
        private int mCount;

        public override void Init()
        {
            mCount = 0;
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mCount >= 2)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say(mDictionary.GetString("recipenotfound"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking && mCount < 2)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCount++;
        }
    }
}