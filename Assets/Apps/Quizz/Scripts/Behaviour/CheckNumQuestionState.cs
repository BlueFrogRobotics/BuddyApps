using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using BlueQuark;

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
            Debug.Log("Quizz: check num question state");

            if (mQuizzBehaviour.ActualPlayerId >= (mQuizzBehaviour.NumPlayer - 1) 
                && mQuizzBehaviour.ActualRound >= (QuizzData.Instance.NbQuestions - 1))
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

            int lId = GetNextQuestionId();
            mQuizzBehaviour.ListQuestionsIdAsked.Add(lId);
            mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[lId];

            Debug.Log("Quizz: actual player: " + mQuizzBehaviour.ActualPlayerId + " actual round: " + mQuizzBehaviour.ActualRound);
            Debug.Log("Quizz: question theme: " + mQuizzBehaviour.ActualQuestion.Theme);
            mQuizzBehaviour.Beginning = false;

            string msg = "";
            if (mQuizzBehaviour.ActualPlayerId == mQuizzBehaviour.NumPlayer - 1 && mQuizzBehaviour.ActualRound == QuizzData.Instance.NbQuestions - 1)
                msg = Buddy.Resources.GetRandomString("lastquestion");
            else if (mQuizzBehaviour.Players.Count>1)
                msg = Buddy.Resources.GetRandomString("questiontoplayer").Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name).Replace("[theme]", mQuizzBehaviour.ThemeToString(mQuizzBehaviour.ActualQuestion.Theme));
            else
                msg = Buddy.Resources.GetRandomString("transitiononeplayer").Replace("[n]", ""+(mQuizzBehaviour.ActualRound+1)).Replace("[name]", mQuizzBehaviour.Players[mQuizzBehaviour.ActualPlayerId].Name).Replace("[theme]", mQuizzBehaviour.ThemeToString(mQuizzBehaviour.ActualQuestion.Theme));


            Buddy.Vocal.Say(msg, (iOutput) => {
                Trigger("LoadQuestion");
            });

            yield return null;
        }

        private IEnumerator WillEndGame()
        {
            mSoundsManager.PlaySound(SoundsManager.Sound.END_GAME);
            yield return new WaitForSeconds(2);

            Buddy.Vocal.SayKey("endgame", (iOutput) => {
                Trigger("EndGame");
            });
            yield return null;
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