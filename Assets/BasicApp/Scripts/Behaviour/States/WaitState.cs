using UnityEngine.UI;
using UnityEngine;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.BasicApp
{
    public class WaitState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mToaster.Display<BinaryQuestionToast>().With(
                mDictionary.GetString("restart"),
                () => Trigger(TRIGGER_END_STATE),
                () => {}
            );
        }
    }
}