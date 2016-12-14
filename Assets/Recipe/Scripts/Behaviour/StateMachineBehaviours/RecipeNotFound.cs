using UnityEngine;
using System.Collections;
using BuddyOS.App;
using BuddyOS;

namespace BuddyApp.Recipe
{
    public class RecipeNotFound : AStateMachineBehaviour
    {
        private int mCount;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCount++;
            if (mCount > 2)
                iAnimator.SetTrigger("ChooseWithScreen");
            else
                mTTS.Say("Désolé je n'ai pas trouvé cette recette, veux tu bien répéter ?");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mTTS.IsSpeaking())
                iAnimator.SetTrigger("AskRecipeAgain");
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}