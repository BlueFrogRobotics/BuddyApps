using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BuddyApp.Reminder
{
 
    public class StartReminder : AStateMachineBehaviour
    {

        public override void Start()
        {

        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(ReminderData.Instance.GiveReminder)
            {
                Trigger("GetReminder");
            }
            else
            {
                Trigger("AskReminder");
            }

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }



    }
}

