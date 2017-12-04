using UnityEngine;

namespace BuddyApp.PlayMath{
    public class BestScoreState : AnimatorSyncState {

		private Animator mBestScoreAnimator;

        private bool mIsOpen;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBestScoreAnimator = GameObject.Find("UI/Best_Score").GetComponent<Animator>();

            mPreviousStateBehaviours.Add(GameObject.Find("UI/Menu").GetComponent<MainMenuBehaviour>());

            mIsOpen = false;

			GameObject.Find("UI/Best_Score").GetComponent<BestScoreBehaviour>().DisplayScore();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mBestScoreAnimator.SetTrigger("open");
                mIsOpen = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mBestScoreAnimator.SetTrigger("close");
            mIsOpen = false;
        }
    }
}
