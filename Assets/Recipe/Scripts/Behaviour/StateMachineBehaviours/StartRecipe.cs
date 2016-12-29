using UnityEngine;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class StartRecipe : AStateMachineBehaviour
    {
        //private bool mDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //mDone = false;
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated)
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(2).SetActive(false);
                GetGameObject(1).SetActive(true);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
            mTTS.Say("Pour commencer préparez les ingrédients suivants pour " + GetComponent<RecipeBehaviour>().mRecipe.person + " personnes :");
            GetComponent<RecipeBehaviour>().IngredientIndex = 0;
            GetComponent<RecipeBehaviour>().IngredientNbr = GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count;
            iAnimator.SetTrigger("DisplayIngredient");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            /*if (!mDone && GetGameObject(5).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_RecipeList_Off"))
            {
                mDone = true;
                iAnimator.SetTrigger("DisplayIngredient");
            }*/
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}