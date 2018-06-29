﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    public class AskQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("ask question state");
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.AddGrammar("answers", Buddy.LoadContext.APP);
            Interaction.VocalManager.OnVoconBest = VoconBest;
            Interaction.VocalManager.OnVoconEvent = EventVocon;
            StartCoroutine(AskQuestion());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private IEnumerator AskQuestion()
        {
            Interaction.TextToSpeech.Say(mQuizzBehaviour.ActualQuestion.Question);
            string lAnswers = "";
            for (int i = 0; i < mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
            {
                if (i == mQuizzBehaviour.ActualQuestion.Answers.Count - 1)
                {
                    lAnswers += " ";
                    lAnswers += Dictionary.GetString("or");
                    lAnswers += " ";
                }
                else
                    lAnswers += ", ";
                lAnswers += " " + mQuizzBehaviour.ActualQuestion.Answers[i];
            }
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.TextToSpeech.Say(lAnswers);
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            Interaction.VocalManager.StartInstantReco();
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log("le best result: " + iBestResult.Utterance + " confidence: " + iBestResult.Confidence+" start rule: "+ iBestResult.StartRule);
            if (iBestResult.StartRule == "answers_fr#answer" && iBestResult.Utterance.Contains(mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer]))
            {
                Trigger("Win");
            }
            else
                Trigger("Lose");
        }

    }
}