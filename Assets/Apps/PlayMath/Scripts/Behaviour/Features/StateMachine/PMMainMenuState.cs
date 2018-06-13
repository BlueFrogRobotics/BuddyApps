using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath
{
    public class PMMainMenuState : AnimatorSyncState
    {
        private bool mIsOpen;

        private Animator mBackgroundAnimator;

        public override void Start()
        {
            mIsOpen = false;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBackgroundAnimator = GameObject.Find("UI/Background_Black").GetComponent<Animator>();
            
            if (mIsOpen)
                mBackgroundAnimator.SetTrigger("close");
            mIsOpen = true;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mBackgroundAnimator.SetTrigger("open");

        }
    }
}
