using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.RedLightGreenLightGame
{
    public class RLGLAskReplay : AStateMachineBehaviour
    {
        private LevelManager mLevelManager;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mLevelManager = GetComponent<LevelManager>();
            StartCoroutine(AskReplay());
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

        private IEnumerator AskReplay()
        {
            yield return SayKeyAndWait("askrestart");
            Toaster.Display<BinaryQuestionToast>().With(
                Dictionary.GetString("askrestart"), 
                () => StartCoroutine(Restart()),
                () => Trigger("Quit"));

        }

        private IEnumerator Restart()
        {
            mLevelManager.Reset();
            yield return SayAndWait("ok");
            Trigger("Repositionning");
        }

    }

}
