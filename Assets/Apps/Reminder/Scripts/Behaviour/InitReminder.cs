using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Reminder
{
    public class InitReminder : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("----- INIT REMINDER DATA -----");
            ReminderData.Instance.AppState = 0;
            ReminderData.Instance.ReminderDate = new DateTime(0001, 01, 01);
            ReminderData.Instance.DateSaved = false;
            ReminderData.Instance.HourSaved = false;
            Debug.Log("----- REMINDER WILL START -----");
            Trigger("StartReminder");
        }
    }
}
