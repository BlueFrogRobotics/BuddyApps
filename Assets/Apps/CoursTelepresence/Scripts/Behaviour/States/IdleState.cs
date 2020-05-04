﻿using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;
using UnityEngine.UI;

namespace BuddyApp.CoursTelepresence
{

    public sealed class IdleState : AStateMachineBehaviour
    {

        private RTMManager mRTMManager;
        private RTCManager mRTCManager;
        private Button mCallButton;
        private string mChannelId = "channeltest";

        override public void Start()
        {
            // This returns the GameObject named RTMCom.
            mRTMManager = GetComponent<RTMManager>();
            mRTCManager = GetComponent<RTCManager>();

            mRTMManager.OncallRequest = (CallRequest lCall) => { Trigger("INCOMING CALL"); };

            mCallButton = GetGameObject(10).GetComponentInChildren<Button>();
            mCallButton.onClick.AddListener(() => {
                Debug.LogWarning("Join channel " + mChannelId + " waiting for tablet answer");
                Trigger("CALLING");
                mRTCManager.Join(mChannelId);
                mRTMManager.RequestConnexion(mChannelId, Buddy.Platform.RobotUID);
            }
            );

            
        }
        

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Idle state");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Idle state exit");
        }
    }

}