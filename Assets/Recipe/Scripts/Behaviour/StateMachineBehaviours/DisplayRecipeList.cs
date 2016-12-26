using UnityEngine;
using System.Collections.Generic;
using BuddyOS.App;

namespace BuddyApp.Recipe
{
    public class DisplayRecipeList : AStateMachineBehaviour
    {
        private List<Recipe> mRecipeList;

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
            GetComponent<RecipeBehaviour>().DisplayRecipe();
            GetGameObject(5).GetComponent<Animator>().SetTrigger("Open_WRecipeList");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(5).GetComponent<Animator>().SetTrigger("Close_WRecipeList");
        }
    }
}