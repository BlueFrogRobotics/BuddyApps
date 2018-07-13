using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.FreezeDance
{
    public class FDEngagement : AStateMachineBehaviour
    {

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartCoroutine(EngagementSentence());
        }

        private IEnumerator EngagementSentence()
        {
            Interaction.TextToSpeech.SayKey("getready");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("Start");
        }
    }
}