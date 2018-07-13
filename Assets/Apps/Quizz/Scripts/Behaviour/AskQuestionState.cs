using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;

namespace BuddyApp.Quizz
{
    public class AskQuestionState : AStateMachineBehaviour
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
            Debug.Log("ask question state");
            Interaction.VocalManager.UseVocon = true;
            Interaction.VocalManager.ClearGrammars();
            Interaction.VocalManager.AddGrammar("commands", Buddy.LoadContext.APP);
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

            //mSoundsManager.PlaySound(SoundsManager.Sound.QUIZZ_BUZZER);
            //while(mSoundsManager.IsPlaying)
            //    yield return null;
            Interaction.VocalManager.StartInstantReco();
        }

        private IEnumerator RepeatQuestion()
        {
            Interaction.TextToSpeech.SayKey("canrepeat");
            while (!Interaction.TextToSpeech.HasFinishedTalking)
                yield return null;
            yield return AskQuestion();
        }

        private void EventVocon(VoconEvent iEvent)
        {
            Debug.Log(iEvent);
        }

        private void VoconBest(VoconResult iBestResult)
        {
            Debug.Log("le best result: " + iBestResult.Utterance + " confidence: " + iBestResult.Confidence + " start rule: " + iBestResult.StartRule);
            if (iBestResult.Utterance == null || iBestResult.Utterance == "" || iBestResult.Confidence == 0)
            {
                //Interaction.VocalManager.StopRecognition();
                Interaction.VocalManager.StartInstantReco();
            }
            else
            {
                int lAnswerId = -1;
                if (iBestResult.StartRule == "answers_" + mQuizzBehaviour.Lang + "#answer" /*&& iBestResult.Utterance.Contains(mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer])*/)
                {
                    for (int i = 0; i < mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
                    {
                        if (iBestResult.Utterance.Contains(mQuizzBehaviour.ActualQuestion.Answers[i]))
                            lAnswerId = i;
                    }
                    //Trigger("Win");
                }
                else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#first")
                    lAnswerId = 0;
                else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#second")
                    lAnswerId = 1;
                else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#third")
                    lAnswerId = 2;
                if (lAnswerId != -1)
                {
                    if (mQuizzBehaviour.ActualQuestion.GoodAnswer == lAnswerId)
                        Trigger("Win");
                    else
                        Trigger("Lose");
                }
                else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#repeat")
                {
                    StartCoroutine(RepeatQuestion());
                }
                else if (iBestResult.StartRule == "commands_" + mQuizzBehaviour.Lang + "#quit" && iBestResult.Confidence > 6000)
                {
                    Trigger("Quit");
                }
                else
                    StartCoroutine(AskQuestion());//Trigger("Lose");
                Debug.Log("mdr");
            }
        }

    }
}