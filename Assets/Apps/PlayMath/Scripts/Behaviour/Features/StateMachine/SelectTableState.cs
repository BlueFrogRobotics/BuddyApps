using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class SelectTableState : AStateMachineBehaviour {

        private Animator mSetTableAnimator;

        private Text mTitleTop;
        private Text mGoToMenu;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            mSetTableAnimator = GameObject.Find("UI/Set_Table").GetComponent<Animator>();
            mSetTableAnimator.SetTrigger("open");

            TranslateUI();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            mSetTableAnimator.SetTrigger("close");
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        private void TranslateUI()
        {
            mTitleTop = GameObject.Find("UI/Set_Table/Top_UI/Title_Top").GetComponent<Text>();
            mGoToMenu = GameObject.Find("UI/Set_Table/Bottom_UI/Button_Menu/Text").GetComponent<Text>();

            mTitleTop.text = BYOS.Instance.Dictionary.GetString("settabletitle").ToUpper();
            mGoToMenu.text = BYOS.Instance.Dictionary.GetString("gotomenulabel").ToUpper();
        }
    }
}