using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    public class WinState : AStateMachineBehaviour
    {

        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("win state");
            StartCoroutine(Win());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator Win()
        {
            Interaction.TextToSpeech.Say(Dictionary.GetRandomString("win").Replace("[answer]", "" + mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer]));
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            if (mQuizzBehaviour.ActualQuestion.AnswerComplement != "")
                Interaction.TextToSpeech.Say(mQuizzBehaviour.ActualQuestion.AnswerComplement);
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("CheckNumQuestion");
        }
    }
}