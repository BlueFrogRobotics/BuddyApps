using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public sealed class ReminderExitState : AStateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state 
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.Say(Buddy.Resources.GetString(ReminderDateManager.STR_BYE), (iOutput) => { QuitApp(); });
        }
    }
}
