using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class FinishRecipe : AStateMachineBehaviour
    {
        private bool mDone;

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mDone = false;
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("finish"));
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!mDone && Interaction.TextToSpeech.HasFinishedTalking)
            {
                mDone = true;
                GetComponent<Animator>().SetTrigger("AskAnotherRecipe");
            }
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}