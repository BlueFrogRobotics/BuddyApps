using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLGameOver : AStateMachineBehaviour
    {
        //TODO take real level
        int mLevel = 0;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            StartCoroutine(GameOver());

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

        private IEnumerator GameOver()
        {
            Interaction.Mood.Set(MoodType.SAD);
            yield return SayKeyAndWait("gameover");
            Toaster.Display<DefeatToast>().With("game over");
            yield return new WaitForSeconds(2);
            Toaster.Hide();
            Interaction.Mood.Set(MoodType.HAPPY);
            Interaction.Face.SetEvent(FaceEvent.SMILE);
            yield return new WaitForSeconds(1);
            yield return SayAndWait(Dictionary.GetRandomString("lastlevel") + mLevel);
            mLevel++;
            Trigger("AskReplay");
        }

    }

}
