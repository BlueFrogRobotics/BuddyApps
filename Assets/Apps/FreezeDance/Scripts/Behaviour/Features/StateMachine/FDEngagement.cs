using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlueQuark;

namespace BuddyApp.FreezeDance
{
    public class FDEngagement : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            string txt = Buddy.Resources.GetRandomString("start");
            txt += Buddy.Resources.GetRandomString("gamestart");
            txt += Buddy.Resources.GetRandomString("getready");
            Buddy.Vocal.Say(txt, (iOutput) =>
            {
                Trigger("Start");
            });
        }
    }
}