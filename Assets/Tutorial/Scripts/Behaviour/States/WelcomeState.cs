using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class WelcomeState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(SayIntro());
        }

        private IEnumerator SayIntro()
        {
            Interaction.TextToSpeech.SayKey("welcomestate");

            yield return new WaitForSeconds(8F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}