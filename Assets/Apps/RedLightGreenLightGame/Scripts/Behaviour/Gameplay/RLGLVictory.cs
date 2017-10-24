using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLVictory : AStateMachineBehaviour
	{
        //TODO take real level
        int mLevel = 0;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            StartCoroutine(Congratulation());
			
			//Interaction.TextToSpeech.Silence(1000);
			
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
			
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        private IEnumerator Congratulation()
        {
            yield return SayKeyAndWait("victory");
            Interaction.Mood.Set(MoodType.HAPPY);
            Interaction.Face.SetEvent(FaceEvent.SMILE);
            Toaster.Display<VictoryToast>().With("niveau "+mLevel+" fini");
            yield return new WaitForSeconds(2);
            Toaster.Hide();
            yield return SayAndWait(Dictionary.GetRandomString("wonlevel") + mLevel);
            yield return new WaitForSeconds(1);
            Interaction.Mood.Set(MoodType.NEUTRAL);
            mLevel++;
            yield return SayAndWait(Dictionary.GetRandomString("seriousbegin") + mLevel+". Let's go");
            Trigger("Repositionning");
        }

    }

}
