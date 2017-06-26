using UnityEngine.UI;
using UnityEngine;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class BinaryState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.TextToSpeech.SayKey("binarystate");

            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("binaryquestion"),
                () => Trigger(TRIGGER_YES_CLICK),
                () => Trigger(TRIGGER_NO_CLICK)
            );
        }
    }
}