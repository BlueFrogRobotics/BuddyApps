using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class QuestionState : AStateMachineBehaviour {

		private Animator mQuestionAnimator;
        private QuestionBehaviour mQuestionBehaviour;
        private Scrollbar mTimeScrollbar;
        //TODO Replace with GameParameters max allowed time
        private double mMaxTime;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mQuestionAnimator = GameObject.Find("UI/Four_Answer").GetComponent<Animator>();
			mQuestionAnimator.SetTrigger("open");

            mQuestionBehaviour = GameObject.Find("UI/Four_Answer").GetComponent<QuestionBehaviour>();
            mQuestionBehaviour.GenerateEquation();

            mTimeScrollbar = GameObject.Find("UI/Four_Answer/Bottom_UI/Time_Progress").GetComponent<Scrollbar>();
            mTimeScrollbar.size = 0;
            mTimeScrollbar.value = 0;
            mMaxTime = 10;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            double lElapsedTime = mQuestionBehaviour.ElapsedTimeSinceStart();
            if(lElapsedTime < mMaxTime)
                mTimeScrollbar.size = (float) (lElapsedTime / mMaxTime);
            else //TimeOut
                mQuestionBehaviour.TimeOut();

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mQuestionAnimator.SetTrigger("close");
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
