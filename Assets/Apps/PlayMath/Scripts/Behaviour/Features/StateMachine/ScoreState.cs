using UnityEngine;

namespace BuddyApp.PlayMath{
    public class ScoreState : AStateMachineBehaviour {

		private Animator mScoreAnimator;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mScoreAnimator = GameObject.Find("UI/EndGame_Score").GetComponent<Animator>();
			mScoreAnimator.SetTrigger("open");

            mScoreAnimator.gameObject.GetComponent<ScoreBehaviour>().TranslateUI();

            GameObject.Find("UI/EndGame_Score").GetComponent<ScoreBehaviour>().DisplayScore();
        }

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mScoreAnimator.SetTrigger("close");
		}
    }
}
