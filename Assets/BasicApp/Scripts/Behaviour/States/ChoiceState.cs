using UnityEngine;
using UnityEngine.UI;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class ChoiceState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS.SayKey("choicestate");

            mToaster.Display<ChoiceToast>().With(
                   mDictionary.GetString("titleopt"),
                   new ButtonInfo() {
                       Label = mDictionary.GetString("firstopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   },
                   new ButtonInfo() {
                       Label = mDictionary.GetString("secondopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   },
                   new ButtonInfo() {
                       Label = mDictionary.GetString("thirdopt"),
                       OnClick = () => Trigger(TRIGGER_END_STATE)
                   });
        }
    }
}