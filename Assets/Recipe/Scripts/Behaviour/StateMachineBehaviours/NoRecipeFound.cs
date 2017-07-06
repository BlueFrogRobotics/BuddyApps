using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class NoRecipeFound : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GetComponent<RecipeBehaviour>().IsBackgroundActivated) {
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
                GetGameObject(2).SetActive(true);
                GetGameObject(1).SetActive(false);
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            }
			Interaction.TextToSpeech.Say(Dictionary.GetString("nomatchingrecipe"));
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Interaction.TextToSpeech.HasFinishedTalking)
                iAnimator.SetTrigger("AskCategoryAgain");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}