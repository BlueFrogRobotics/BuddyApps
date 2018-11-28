using UnityEngine;

namespace BuddyApp.TakePhoto
{
    public class Quit : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            QuitApp();
        }
    }
}

