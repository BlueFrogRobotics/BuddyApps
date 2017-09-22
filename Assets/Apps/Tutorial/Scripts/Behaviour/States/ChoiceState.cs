using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class ChoiceState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.TextToSpeech.SayKey("choicestate");

            Toaster.Display<ChoiceToast>().With(
                   Dictionary.GetString("titleopt"),
                   new ButtonInfo() {
                       Label = Dictionary.GetString("firstopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   },
                   new ButtonInfo() {
                       Label = Dictionary.GetString("secondopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   },
                   new ButtonInfo() {
                       Label = Dictionary.GetString("thirdopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   });
        }
    }
}