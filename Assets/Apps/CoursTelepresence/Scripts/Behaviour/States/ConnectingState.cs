using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{

    public sealed class ConnectingState : AStateMachineBehaviour
    {
        [SerializeField]
        private Animator ConnectingScreenAnimator;

        private RTMManager mRTMManager;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Connecting state");

            //TODO check DB and stuff

            if (Buddy.Behaviour.Mood != Mood.NEUTRAL)
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(DBManager.Instance.Peering && DBManager.Instance.InfoRequestedDone)
            {
                mRTMManager.SetTabletId(DBManager.Instance.ListUIDTablet[0]);
                Trigger("IDLE");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Connecting state exit");
        }
    }

}