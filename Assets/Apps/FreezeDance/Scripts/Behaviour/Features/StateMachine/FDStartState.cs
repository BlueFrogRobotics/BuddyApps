using BlueQuark;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.FreezeDance
{
    public class FDStartState : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(PlayBMLEnter());
        }

        private IEnumerator PlayBMLEnter()
        {
            Buddy.Behaviour.Interpreter.RunRandom(Mood.HAPPY);
            Buddy.Vocal.SayKey("start");

            while (Buddy.Vocal.IsBusy)
                yield return null;
            yield return new WaitForSeconds(1.0F);
            Buddy.Vocal.SayKey("nextaction");
            while (Buddy.Vocal.IsBusy)
                yield return null;
            Buddy.Behaviour.Stop();
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Buddy.Actuators.Head.SetPosition(15F, 0F);
            Trigger("Start");
        }
    }
}