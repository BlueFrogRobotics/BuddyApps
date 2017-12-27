using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
    public class PMSettingsState : AnimatorSyncState {

		private Animator mSettingsAnimator;

        private bool mIsOpen;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mSettingsAnimator = GameObject.Find("UI/Settings").GetComponent<Animator>();

            mPreviousStateBehaviours.Add(GameObject.Find("UI/Menu").GetComponent<MainMenuBehaviour>());

            mIsOpen = false;

            mSettingsAnimator.gameObject.GetComponent<SettingsBehaviour>().InitState();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (PreviousBehaviourHasEnded() && !mIsOpen)
            {
                mSettingsAnimator.SetTrigger("open");
                mIsOpen = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			mSettingsAnimator.SetTrigger("close");
            mIsOpen = false;
        }
    }
}
