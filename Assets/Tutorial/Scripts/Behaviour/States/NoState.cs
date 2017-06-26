using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class NoState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(SaySomething());
        }

        private IEnumerator SaySomething()
        {
            Interaction.TextToSpeech.SayKey("nostate");

            yield return new WaitForSeconds(7F);

            while (Interaction.TextToSpeech.IsSpeaking)
                yield return null;

            Trigger(TRIGGER_END_STATE);
        }
    }
}