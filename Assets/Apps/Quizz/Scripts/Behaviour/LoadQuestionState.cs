using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Buddy;

namespace BuddyApp.Quizz
{
    public class LoadQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("load question state");
            //mQuizzBehaviour.ActualQuestion = mQuizzBehaviour.Questions.Questions[0];
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
            lLines.Add("!grammar answers_fr;");
            lLines.Add("!start <answer>;");
            lLines.Add("!start <repeat>;");
            lLines.Add("!start <quit>;");
            //lLines.Add("<repeat>;");
            lLines.Add("<repeat> : ([est-ce que] je peux | peut-on | [est-ce qu'] on peut) répeter;");
            lLines.Add("<quit> : quitter | annuler | arrêter | sortir | terminer | annule | quitte | ferme | arrête | arrêt | ce sera tout | rien | fin;");
            string lAllAnswers = "<answer> :";
            for (int i=0; i< mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
            {
                lAllAnswers += mQuizzBehaviour.ActualQuestion.Answers[i];
                if (i < mQuizzBehaviour.ActualQuestion.Answers.Count - 1)
                    lAllAnswers += " | ";
            }
            lAllAnswers += ";";
            lLines.Add(lAllAnswers);
            File.WriteAllLines(BYOS.Instance.Resources.GetPathToRaw("answers_fr.txt"), lLines.ToArray());
            
        }

        private void CompileAnswersGrammar()
        {
            List<string> lGramars = new List<string>();
            lGramars.Add(BYOS.Instance.Resources.GetPathToRaw("answers_fr.txt"));
            Interaction.VoconSTT.CompileGrammars(lGramars);
        }
    }
}