using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class RecipeNotFound : AStateMachineBehaviour
    {
        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GetComponent<RecipeBehaviour>().RecipeNotFoundCount >= 2)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say(mDictionary.GetString("recipenotfound"));
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (mTTS.HasFinishedTalking && GetComponent<RecipeBehaviour>().RecipeNotFoundCount < 2)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().RecipeNotFoundCount++;
        }
    }
}