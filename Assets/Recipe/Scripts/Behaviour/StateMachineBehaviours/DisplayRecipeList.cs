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
            GetComponent<RecipeBehaviour>().DisplayRecipe();
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetGameObject(1).SetActive(false);
            GetGameObject(2).SetActive(true);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WRecipeList");
        }
    }
}