using UnityEngine.UI;
using UnityEngine;

using Buddy;
using Buddy.UI;

using System.Collections;

namespace BuddyApp.Tutorial
{
    public class WaitState : AStateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("restart"),
                () => Trigger(TRIGGER_END_STATE),
                () => {}
            );
        }
    }
}