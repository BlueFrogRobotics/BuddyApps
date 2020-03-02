using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using BlueQuark;

namespace BuddyApp.BrainTraining
{
    public class CheckNumQuestionState : AStateMachineBehaviour
    {
        private BrainTrainingBehaviour mBrainTrainingBehaviour;

        public override void Start()
        {
            mBrainTrainingBehaviour = GetComponent<BrainTrainingBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (mBrainTrainingBehaviour.ListQuestionsIdAsked.Count >= (BrainTrainingData.Instance.NbQuestions - 1))
                Trigger("EndGame");
            else
                SetNextQuestion();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        private void SetNextQuestion()
        {
            int lId = GetNextQuestionId();
            mBrainTrainingBehaviour.ListQuestionsIdAsked.Add(lId);
            mBrainTrainingBehaviour.ActualQuestion = mBrainTrainingBehaviour.Questions.Questions[lId];

            Trigger("LoadQuestion");
        }

        private int GetNextQuestionId()
        {
            IEnumerable<int> lRange = Enumerable.Range(0, mBrainTrainingBehaviour.Questions.Questions.Count).Where(i => !mBrainTrainingBehaviour.ListQuestionsIdAsked.Contains(i));

            System.Random lRand = new System.Random();
            int lIndex = lRand.Next(0, mBrainTrainingBehaviour.Questions.Questions.Count - mBrainTrainingBehaviour.ListQuestionsIdAsked.Count);
            return lRange.ElementAt(lIndex);
        }
    }
}