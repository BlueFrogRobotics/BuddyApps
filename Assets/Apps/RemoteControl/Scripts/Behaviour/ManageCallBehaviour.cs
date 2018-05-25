using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RemoteControl
{
    public class ManageCallBehaviour : AStateMachineBehaviour
    {
        [SerializeField]
        private bool ReceiveCall;
        [SerializeField]
        private bool RejectedCall;

        private RemoteControlBehaviour mRemoteControlBehaviour;
        private bool mHasInitializedRemote;

        public override void Start()
        {
            Debug.Log("HOLA");
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
        }

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mHasInitializedRemote = false;
            if (ReceiveCall)
            {
                StartCoroutine(ActivateDisplay());
            }
            else
            {
                if (RejectedCall)
                {
                    Debug.Log("Rejected");
                    mRemoteControlBehaviour.CloseApp();
                    QuitApp();
                }
                else
                {
                    Debug.Log("Accepted");
                    mRemoteControlBehaviour.LaunchCall();
                }
            }

        }

        private IEnumerator ActivateDisplay()
        {
            yield return mRemoteControlBehaviour.Call();
            mHasInitializedRemote = true;
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mHasInitializedRemote)
            {
                Debug.Log("Call is Received");
                Trigger("CallReceived");
            }
        }
        
    }
}