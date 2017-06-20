using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class ToasterState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(IntroduceToasterSpeech());
        }

        private IEnumerator IntroduceToasterSpeech()
        {
            Interaction.TextToSpeech.SayKey("toasterstate");

            yield return new WaitForSeconds(7F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}