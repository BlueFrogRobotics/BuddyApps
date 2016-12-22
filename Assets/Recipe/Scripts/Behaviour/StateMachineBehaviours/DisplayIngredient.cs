using UnityEngine;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayIngredient : AStateMachineBehaviour
    {
        private bool mSentenceDone = false;
        private List<Ingredient> mIngredientList;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated)
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(1).SetActive(true);
                GetGameObject(2).SetActive(false);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
            GetGameObject(3).GetComponent<Animator>().SetTrigger("Open_WList");
            if (!mSentenceDone)
            {
                mTTS.Say("Pour commencer préparez les ingrédients suivants:");
                GetComponent<RecipeBehaviour>().IngredientIndex = 0;
                GetComponent<RecipeBehaviour>().IngredientNbr = GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count;
                mSentenceDone = true;
            }
            GetComponent<RecipeBehaviour>().DisplayIngredient();
            iAnimator.SetTrigger("TransitionIngredient");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}