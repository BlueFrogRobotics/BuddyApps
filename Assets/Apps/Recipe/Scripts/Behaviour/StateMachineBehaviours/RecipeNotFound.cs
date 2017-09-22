using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class RecipeNotFound : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCheck = false;
            Interaction.Mood.Set(MoodType.THINKING, false, true);
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                if (GetComponent<RecipeBehaviour>().RecipeNotFoundCount < 2)
                    Interaction.TextToSpeech.Say(Dictionary.GetRandomString("recipenotfound"));
                else
                    Interaction.Mood.Set(MoodType.NEUTRAL);
                mCheck = true;
            }
            else if (mCheck && Interaction.TextToSpeech.HasFinishedTalking)
                iAnimator.SetTrigger("AskRecipeAgain");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetComponent<RecipeBehaviour>().RecipeNotFoundCount++;
        }
    }
}