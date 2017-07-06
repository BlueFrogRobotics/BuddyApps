using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class StartRecipe : AStateMachineBehaviour
    {
        private bool mCheck;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Close_BG");
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = false;
            mCheck = false;
			Interaction.Mood.Set(MoodType.HAPPY, false, true);
            GetComponent<RecipeBehaviour>().StepList = GetComponent<RecipeBehaviour>().mRecipe.step;
            GetComponent<RecipeBehaviour>().StepIndex = 0;
            GetComponent<RecipeBehaviour>().IngredientIndex = 0;
            GetComponent<RecipeBehaviour>().IngredientNbr = GetComponent<RecipeBehaviour>().mRecipe.ingredient.Count;
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetRandomString("startrecipe") + " " + GetComponent<RecipeBehaviour>().mRecipe.name + "[800]");
                if (GetComponent<RecipeBehaviour>().mRecipe.person > 1)
                    Interaction.TextToSpeech.Say(Dictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + Dictionary.GetString("person") + "s:[500]", true);
                else
                    Interaction.TextToSpeech.Say(Dictionary.GetString("startingredient") + GetComponent<RecipeBehaviour>().mRecipe.person + Dictionary.GetString("person") + ":[500]", true);
                mCheck = true;
            }
            if (mCheck && Interaction.TextToSpeech.HasFinishedTalking)
                iAnimator.SetTrigger("DisplayIngredient");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.VocalManager.StopListenBehaviour = null;
            Interaction.Mood.Set(MoodType.NEUTRAL);
            if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated)
            {
                GetGameObject(0).GetComponent<Animator>().ResetTrigger("Close_BG");
                GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
                GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
            }
        }
    }
}