using UnityEngine;

namespace BuddyApp.PlayMath{
    public class TakePhotoState : AStateMachineBehaviour {

		private Animator mTakePhotoAnimator;

        private TakePhotoBehaviour mTakePhotoBehaviour;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mTakePhotoAnimator = GameObject.Find("UI/Take_Photo").GetComponent<Animator>();
			mTakePhotoAnimator.SetTrigger("open");

            mTakePhotoBehaviour = GameObject.Find("UI/Take_Photo").GetComponent<TakePhotoBehaviour>();
            mTakePhotoBehaviour.DisplayCamera();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mTakePhotoAnimator.SetTrigger("close");
        }
    }
}
