using UnityEngine.UI;
using UnityEngine;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class BinaryState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTTS.SayKey("binarystate");

            mToaster.Display<BinaryQuestionToast>().With(
                mDictionary.GetString("binaryquestion"),
                () => Trigger(TRIGGER_YES_CLICK),
                () => Trigger(TRIGGER_NO_CLICK)
            );
        }
    }
}