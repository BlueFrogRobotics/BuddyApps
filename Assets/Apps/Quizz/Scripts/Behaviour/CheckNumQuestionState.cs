using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace BuddyApp.Quizz
{
    public class CheckNumQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;
        private SoundsManager mSoundsManager;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
            mSoundsManager = GetComponent<SoundsManager>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("check num question state");
            if (mQuizzBehaviour.ActualPlayerId >= mQuizzBehaviour.NumPlayer -1 && mQuizzBehaviour.ActualRound >= QuizzBehaviour.MAX_ROUNDS - 1)
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
            Debug.Log("actual player: " + mQuizzBehaviour.ActualPlayerId + " actual round: " + mQuizzBehaviour.ActualRound);
            mQuizzBehaviour.Beginning = false;
            Interaction.TextToSpeech.Say((Dictionary.GetRandomString("questiontoplayer").Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name)));
            //System.Random random = new System.Random();
            int lId = GetNextQuestionId();
            mQuizzBehaviour.ListQuestionsIdAsked.Add(lId);
            mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[lId];
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("LoadQuestion");
        }

        private IEnumerator WillEndGame()
        {
            mSoundsManager.PlaySound(SoundsManager.Sound.END_GAME);
            while (mSoundsManager.IsPlaying)
                yield return null;
            Interaction.TextToSpeech.SayKey("endgame");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("EndGame");
        }

        private int GetNextQuestionId()
        {
            IEnumerable<int> lRange = Enumerable.Range(0, mQuizzBehaviour.Questions.Questions.Count).Where(i => !mQuizzBehaviour.ListQuestionsIdAsked.Contains(i));

            System.Random lRand = new System.Random();
            int lIndex = lRand.Next(0, mQuizzBehaviour.Questions.Questions.Count - mQuizzBehaviour.ListQuestionsIdAsked.Count);
            return lRange.ElementAt(lIndex);
        }
    }
}