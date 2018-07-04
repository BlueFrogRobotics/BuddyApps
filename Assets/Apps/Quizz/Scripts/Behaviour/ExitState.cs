using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    public class ExitState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("exit state");
            StartCoroutine(Exit());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator Exit()
        {
            Interaction.Face.SetExpression(Buddy.MoodType.NEUTRAL);
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("exit"));
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            QuitApp();
        }
    }
}