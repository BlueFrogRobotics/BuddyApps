using UnityEngine;
using System.Collections;

public class TimerExitState : AStateGuardian {

    private GameObject mBackgroundPrefab;
    private GameObject mQuestionPrefab;
    private GameObject mHaloPrefab;
    private Animator mBackgroundAnimator;
    private Animator mQuestionAnimator;
    private Animator mHaloAnimator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        mBackgroundPrefab = mStatePatrolManager.BackgroundPrefab;
        mQuestionPrefab = mStatePatrolManager.QuestionPrefab;
        mHaloPrefab = mStatePatrolManager.HaloPrefab;
        mBackgroundAnimator = mStatePatrolManager.BackgroundAnimator;
        mQuestionAnimator = mStatePatrolManager.QuestionAnimator;
        mHaloAnimator = mStatePatrolManager.HaloAnimator;

        animator.SetBool("ChangeState", false);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //if (mHaloAnimator.GetBool("IsOff"))
       // {
            animator.SetBool("ChangeState", true);
       // }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("ChangeState", false);
        animator.SetBool("Cancelled", false);
        mBackgroundPrefab.SetActive(false);
        mQuestionPrefab.SetActive(false);
        mHaloPrefab.SetActive(false);
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
