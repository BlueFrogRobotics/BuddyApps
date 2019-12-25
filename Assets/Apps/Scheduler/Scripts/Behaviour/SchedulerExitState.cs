using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Scheduler
{
    public sealed class SchedulerExitState : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state 
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(Buddy.Resources.GetString(SchedulerDateManager.STR_BYE), (iOutput) => { QuitApp(); });
        }
    }
}
