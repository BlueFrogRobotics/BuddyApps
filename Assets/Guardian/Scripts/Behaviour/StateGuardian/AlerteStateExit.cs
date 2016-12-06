using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BuddyApp.Guardian
{
    public class AlerteStateExit : AStateGuardian
    {

        private GameObject mBackgroundPrefab;
        private GameObject mHaloPrefab;
        private Animator mBackgroundAnimator;
        private Animator mHaloAnimator;
        private GameObject mObjectButtonAskPassword;
        private Button mButtonPassword;
        private Animator mAnimator;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBackgroundPrefab = StateManager.BackgroundPrefab;
            mHaloPrefab = StateManager.HaloPrefab;
            mBackgroundAnimator = StateManager.BackgroundAnimator;
            mHaloAnimator = StateManager.HaloAnimator;
            mObjectButtonAskPassword = StateManager.ObjectButtonAskPassword;

            animator.SetBool("ChangeState", false);
            mButtonPassword = mObjectButtonAskPassword.GetComponentInChildren<Button>();
            mButtonPassword.onClick.AddListener(AskPassword);
            mAnimator = animator;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //if (mHaloAnimator.GetBool("IsOff"))
            //{
            animator.SetBool("ChangeState", true);
            // }

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBackgroundAnimator.SetTrigger("Close_BG");
            animator.SetBool("ChangeState", false);
            animator.SetBool("Cancelled", false);
            //mBackgroundPrefab.SetActive(false);
            mHaloPrefab.SetActive(false);
            mButtonPassword.onClick.RemoveAllListeners();
            mAnimator.SetBool("AskPassword", false);
        }

        private void AskPassword()
        {
            mAnimator.SetBool("AskPassword", true);
            Debug.Log("ask password");
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
}