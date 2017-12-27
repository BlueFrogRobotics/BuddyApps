using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class PMMainMenuState : AnimatorSyncState  {

		private Animator mMenuAnimator;

        private bool mIsOpen;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mMenuAnimator = GameObject.Find("UI/Menu").GetComponent<Animator>();

            // Define all the possible previous state behaviours
            mPreviousStateBehaviours.Add(GameObject.Find("UI/Set_Table").GetComponent<SelectTableBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/EndGame_Score").GetComponent<ScoreBehaviour>());
            //mPreviousStateBehaviours.Add(GameObject.Find("UI/Best_Score").GetComponent<BestScoreBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/EndGame_Certificate").GetComponent<CertificateBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/Settings").GetComponent<SettingsBehaviour>());
            mPreviousStateBehaviours.Add(GameObject.Find("UI/Four_Answer").GetComponent<QuestionBehaviour>());

            mIsOpen = false;

            mMenuAnimator.gameObject.GetComponent<MainMenuBehaviour>().InitState();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mMenuAnimator.SetTrigger("open");
                mIsOpen = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mMenuAnimator.SetTrigger("close");
            mIsOpen = false;
        }
    }
}
