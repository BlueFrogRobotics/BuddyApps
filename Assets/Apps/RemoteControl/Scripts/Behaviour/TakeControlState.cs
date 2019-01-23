using BlueQuark;

using UnityEngine;

using UnityEngine.UI;

using System;

using System.Collections;

using System.Collections.Generic;

namespace BuddyApp.RemoteControl
{
    public class TakeControlState : AStateMachineBehaviour
    {

        private RemoteControlBehaviour mRemoteControlBehaviour;

        public override void Start()
        {
            mRemoteControlBehaviour = GetComponent<RemoteControlBehaviour>();
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mRemoteControlBehaviour.LaunchCall();
        }


        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
        }
    }
}
