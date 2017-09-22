using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class AskRecipe : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Debug.Log("ENTER ASK RECIPE");
            GetComponent<RecipeBehaviour>().NoAnswerCount = 0;
            GetComponent<RecipeBehaviour>().RecipeNotFoundCount = 0;
            GetComponent<RecipeBehaviour>().mRecipeList = null;
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("askprepare"));
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Interaction.TextToSpeech.HasFinishedTalking && Interaction.SpeechToText.HasFinished)
                iAnimator.SetTrigger("QuestionFinished");
            if (Input.touchCount>0 && Input.GetTouch(0).tapCount > 1)
                iAnimator.SetTrigger("ChooseWithScreen");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			Interaction.TextToSpeech.Silence(0);
            Debug.Log("EXIT ASK RECIPE");
        }
    }
}