using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.Quizz
{
    public class CheckNumQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("check num question state");
            if (mQuizzBehaviour.ActualPlayerId >= mQuizzBehaviour.NumPlayer && mQuizzBehaviour.ActualRound >= QuizzBehaviour.MAX_ROUNDS - 1)
                StartCoroutine(WillEndGame());
            else
                StartCoroutine(WillAskQuestion());

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator WillAskQuestion()
        {
            if (!mQuizzBehaviour.Beginning)
            {
                mQuizzBehaviour.ActualPlayerId++;
                if (mQuizzBehaviour.ActualPlayerId >= mQuizzBehaviour.NumPlayer)
                {
                    mQuizzBehaviour.ActualPlayerId = 0;
                    mQuizzBehaviour.ActualRound++;
                }
            }
            else
            {
                mQuizzBehaviour.ActualPlayerId = 0;
                mQuizzBehaviour.ActualRound = 0;
            }
            Interaction.TextToSpeech.Say((Dictionary.GetRandomString("questiontoplayer").Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name)));
            System.Random random = new System.Random();
            int lId = random.Next(mQuizzBehaviour.Questions.Questions.Count);
            mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[lId];
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("LoadQuestion");
        }

        private IEnumerator WillEndGame()
        {
            Interaction.TextToSpeech.SayKey("endgame");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("EndGame");
        }
    }
}