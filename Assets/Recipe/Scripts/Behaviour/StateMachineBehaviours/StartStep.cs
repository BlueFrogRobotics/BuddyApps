using UnityEngine;
using Buddy;

namespace BuddyApp.Recipe
{
    public class StartStep : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("startstep") + " " + "[300]");
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (Interaction.TextToSpeech.HasFinishedTalking)
                GetComponent<Animator>().SetTrigger("DisplayStep");
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
            GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
        }
    }
}