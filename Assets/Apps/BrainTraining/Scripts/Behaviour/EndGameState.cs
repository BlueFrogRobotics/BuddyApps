using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BrainTraining
{
    public class EndGameState : AStateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.OnEndListening.Clear();

            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.Interpreter.StopAndClear();

            string text = Buddy.Resources.GetRandomString("end");

            Buddy.Vocal.Say(text, (iOutput) => {
                Trigger("Exit");
            });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //}
    }
}