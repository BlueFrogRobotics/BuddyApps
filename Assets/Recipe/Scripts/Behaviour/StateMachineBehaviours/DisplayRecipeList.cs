using UnityEngine;
using System.Collections.Generic;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class DisplayRecipeList : AStateMachineBehaviour
    {
        private List<Recipe> mRecipeList;
        private bool mDone;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated) {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetGameObject(2).SetActive(false);
                GetGameObject(1).SetActive(true);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
            GetGameObject(5).GetComponent<Animator>().SetTrigger("Open_WRecipeList");
            mDone = false;
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mDone && GetGameObject(5).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_RecipeList_Idle"))
            {
                mDone = true;
                GetComponent<RecipeBehaviour>().DisplayRecipe();
            }
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().DestroyRecipePrefab();
            GetGameObject(5).GetComponent<Animator>().SetTrigger("Close_WRecipeList");
            /*if (iAnimator.GetBool("StartRecipe"))
            {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
                GetGameObject(2).SetActive(true);
                GetGameObject(1).SetActive(false);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            }*/
        }
    }
}