using UnityEngine;

namespace BuddyApp.PlayMath{
    public class InitState : AStateMachineBehaviour {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			Animator lBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
			lBackgroundAnimator.SetTrigger("open");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetTrigger("InitDone");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        }
    }
}
