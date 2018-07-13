using Buddy;
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
            Interaction.BMLManager.LaunchRandom("happy");
            Primitive.LED.SetBodyLight(LEDColor.ORANGE_HAPPY);
            Interaction.TextToSpeech.SayKey("start");

            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            yield return new WaitForSeconds(1.0F);
            Interaction.TextToSpeech.SayKey("nextaction");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.BMLManager.StopAllBehaviors();
            Interaction.Mood.Set(MoodType.NEUTRAL);
            Primitive.Motors.YesHinge.SetPosition(0, 400);
            Primitive.Motors.NoHinge.SetPosition(0, 400);
            Trigger("Start");
        }
    }
}