using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using BlueQuark;

namespace BuddyApp.Quizz
{
    public class LoadQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;
        private string mGrammarPath;
        private string mGrammarName;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mGrammarName = "answers_" + mQuizzBehaviour.Lang;
            mGrammarPath = Buddy.Resources.AppRawDataPath + mGrammarName + ".txt";
            //Debug.Log("Quizz: grammar file " + mGrammarPath);
            ////mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[0];
            SaveText();
            CompileAnswersGrammar();
            Trigger("AskQuestion");
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private void SaveText()
        {
            List<string> lLines = new List<string>();
            lLines.Add("#BNF+EMV2.1;");
            lLines.Add("!grammar answers_" + mQuizzBehaviour.Lang + ";");
            lLines.Add("!start <answer>;");
            string lAllAnswers = "<answer> :";
            for (int i = 0; i < mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
            {
                lAllAnswers += mQuizzBehaviour.RemoveSpecialCharacters(mQuizzBehaviour.ActualQuestion.Answers[i]);
                if (i < mQuizzBehaviour.ActualQuestion.Answers.Count - 1)
                    lAllAnswers += " | ";
                Debug.Log("Quizz: answer " + i + " " + lAllAnswers);
            }
            lAllAnswers += ";";
            lLines.Add(lAllAnswers);
            File.WriteAllLines(mGrammarPath, lLines.ToArray());
            Debug.Log("Quizz: write " + lLines.ToArray() + " in grammar file" + mGrammarPath);

        }

        private void CompileAnswersGrammar()
        {
            Buddy.Vocal.CompileGrammar(mGrammarName);
        }
    }
}