using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLVictory : AStateMachineBehaviour
	{

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			//TODO take real level
			int lLvl = 1;
			Interaction.Mood.Set(MoodType.HAPPY);
			Interaction.Face.SetEvent(FaceEvent.SMILE);
			Interaction.TextToSpeech.SayKey("victory");
			Interaction.TextToSpeech.Silence(1000);
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("wonlevel") + lLvl, true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			Trigger("Repositionning");
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

			Interaction.Mood.Set(MoodType.NEUTRAL);
		}

    }

}
