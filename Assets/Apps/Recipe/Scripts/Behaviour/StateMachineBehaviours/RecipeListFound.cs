using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class RecipeListFound : AStateMachineBehaviour
    {
        bool mCheck;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mCheck = false;
			Interaction.Mood.Set(MoodType.SURPRISED, false, true);
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mCheck && Primitive.Speaker.Voice.Status == SoundChannelStatus.FINISH)
            {
                Interaction.TextToSpeech.Say(Dictionary.GetString("listrecipefound"));
                mCheck = true;
            }
            else if (mCheck && Interaction.TextToSpeech.HasFinishedTalking)
                iAnimator.SetTrigger("DisplayRecipeList");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}