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
            GetComponent<RecipeBehaviour>().StepList = GetComponent<RecipeBehaviour>().mRecipe.step;
            GetComponent<RecipeBehaviour>().StepIndex = 0;
            if (GetComponent<RecipeBehaviour>().mRecipe.person > 1)
                mTTS.Say(mDictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + mDictionary.GetString("person") + "s:");
            else
                mTTS.Say(mDictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + mDictionary.GetString("person") + ":");
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