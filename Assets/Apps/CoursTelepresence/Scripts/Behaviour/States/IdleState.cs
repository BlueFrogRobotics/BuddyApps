﻿using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.CoursTelepresence
{

    public sealed class IdleState : AStateMachineBehaviour
    {

        private RTMCom mRTMCom;


        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMCom = GetComponent<RTMCom>();
            mRTMCom.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };
        }
        

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Idle state");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{

        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Idle state exit");
        }
    }

}