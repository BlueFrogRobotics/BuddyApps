using UnityEngine;
using UnityEngine.UI;
using Buddy;
using Buddy.UI;

namespace BuddyApp.Recipe
{
    public class ChooseWithScreen : AStateMachineBehaviour
    {

        public override void Start()
        {
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			Interaction.Mood.Set(MoodType.NEUTRAL);
			//if (!GetComponent<RecipeBehaviour>().IsBackgroundActivated) {
			//GetGameObject(0).GetComponent<Animator>().SetTrigger("Open_BG");
			//GetComponent<RecipeBehaviour>().IsBackgroundActivated = true;
			//}

			Toaster.Display<BackgroundToast>().With();

            GetGameObject(13).GetComponent<Text>().text = Dictionary.GetString("starter");
            GetGameObject(14).GetComponent<Text>().text = Dictionary.GetString("dish");
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Open_WCategory");
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("chooserecipecategory"));
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (!Interaction.TextToSpeech.HasFinishedTalking)
				Interaction.TextToSpeech.Silence(0);
            GetGameObject(6).GetComponent<Animator>().SetTrigger("Close_WCategory");
        }
    }
}