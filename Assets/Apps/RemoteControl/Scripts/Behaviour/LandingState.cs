using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.RemoteControl
{
    public class LandingState : AStateMachineBehaviour
    {
        private RemoteControlBehaviour mRemoteControlBehaviour;

        public override void Start()
        {
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (RemoteControlData.Instance.IsWizardOfOz)
                Trigger("WizardOfOz");
            else
                Trigger("Remote");
        }
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

    }
}