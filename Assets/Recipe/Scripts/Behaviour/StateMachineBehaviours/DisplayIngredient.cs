using UnityEngine;
using Buddy;
using System.Collections.Generic;

namespace BuddyApp.Recipe
{
    public class DisplayIngredient : AStateMachineBehaviour
    {
        private List<Ingredient> mIngredientList;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			Interaction.VocalManager.EnableTrigger = true;
            if (GetGameObject(3).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Window_List_Off"))
                GetGameObject(3).GetComponent<Animator>().SetTrigger("Open_WList");
            GetComponent<RecipeBehaviour>().DisplayIngredient();
            iAnimator.SetTrigger("TransitionIngredient");
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}