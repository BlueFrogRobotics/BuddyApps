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
            mQuizzBehaviour.OnLanguageChange = OnLanguageChange;
            if (mQuizzBehaviour.ActualPlayerId >= mQuizzBehaviour.NumPlayer - 1 && mQuizzBehaviour.ActualRound >= QuizzBehaviour.MAX_ROUNDS - 1)
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
            mQuizzBehaviour.OnLanguageChange = null;
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

            int lId = GetNextQuestionId();
            mQuizzBehaviour.ListQuestionsIdAsked.Add(lId);
            mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[lId];

            Debug.Log("actual player: " + mQuizzBehaviour.ActualPlayerId + " actual round: " + mQuizzBehaviour.ActualRound);
            Debug.Log("theme de la question: " + mQuizzBehaviour.ActualQuestion.Theme);
            Debug.Log("apres theme");
            mQuizzBehaviour.Beginning = false;
            if (mQuizzBehaviour.ActualPlayerId == mQuizzBehaviour.NumPlayer - 1 && mQuizzBehaviour.ActualRound == QuizzBehaviour.MAX_ROUNDS - 1)
                Interaction.TextToSpeech.Say((Dictionary.GetRandomString("lastquestion")));

            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;

            if (mQuizzBehaviour.Players.Count>1)
                Interaction.TextToSpeech.Say((Dictionary.GetRandomString("questiontoplayer").Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name).Replace("[theme]", mQuizzBehaviour.ThemeToString(mQuizzBehaviour.ActualQuestion.Theme))));
            else
                Interaction.TextToSpeech.Say((Dictionary.GetRandomString("transitiononeplayer").Replace("[n]", ""+(mQuizzBehaviour.ActualRound+1)).Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name).Replace("[theme]", mQuizzBehaviour.ThemeToString(mQuizzBehaviour.ActualQuestion.Theme))));
            
            //System.Random random = new System.Random();
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Trigger("LoadQuestion");
        }

        private IEnumerator WillEndGame()
        {
            mSoundsManager.PlaySound(SoundsManager.Sound.END_GAME);
            Interaction.BMLManager.LaunchByName("Happy02");
            while (!Interaction.BMLManager.DonePlaying)
                yield return null;
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