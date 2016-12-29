using UnityEngine;
using BuddyOS.App;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayIngredient : AStateMachineBehaviour
    {
        private List<Ingredient> mIngredientList;

        public override void Init()
        {
        }

        protected override void OnEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER DISPLAY INGREDIENT");
            if (GetGameObject(3).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_List_Off"))
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Open_WList");
            GetComponent<RecipeBehaviour>().DisplayIngredient();
            iAnimator.SetTrigger("TransitionIngredient");
        }

        protected override void OnUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

        protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("EXIT DISPLAY INGREDIENT");
        }
    }
}