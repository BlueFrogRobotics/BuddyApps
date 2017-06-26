using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class NumpadState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.TextToSpeech.SayKey("numpadstate");

            Toaster.Display<NumPadToast>().With(
                CheckPassword,
                () => Trigger(TRIGGER_END_STATE),
                () => Interaction.TextToSpeech.SayKey("onfailnumpad"),
                () => Trigger(TRIGGER_END_STATE)
            );
        }

        private IEnumerator CheckPassword(string iPwd)
        {
            if (iPwd == "1234")
                yield return true;
            else
                yield return false;
        }
    }
}