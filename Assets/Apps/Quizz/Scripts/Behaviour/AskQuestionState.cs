using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.Quizz
{
    public class AskQuestionState : AStateMachineBehaviour
    {
        private QuizzBehaviour mQuizzBehaviour;
        private bool mAnswerGiven;

        public override void Start()
        {
            mQuizzBehaviour = GetComponent<QuizzBehaviour>();
        }

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mAnswerGiven = false;

            Debug.Log("Quizz: ask question state");
            mQuizzBehaviour.LastStateId = 2;

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            string[] grammars = { "commands", "answers" };
            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = grammars,
            };
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            if (QuizzData.Instance.DisplayQuestions) 
            {
                ShowToast();
                Buddy.GUI.Screen.OnTouch.Add(OnStopListening);
            }
            
            StartCoroutine(AskQuestion());
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.Vocal.OnEndListening.Clear();

            if (QuizzData.Instance.DisplayQuestions) 
            {
                Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);
                Buddy.GUI.Toaster.Hide();
                Buddy.GUI.Dialoger.Hide();
                Buddy.GUI.Header.HideTitle();
            }
        }

        private IEnumerator AskQuestion()
        {
            string lAnswers = "";
            for (int i = 0; i < mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
            {
                if (i == mQuizzBehaviour.ActualQuestion.Answers.Count - 1)
                {                    
                    lAnswers += Buddy.Resources.GetString("or");
                    lAnswers += " ";
                }
                else
                    lAnswers += ", ";
                lAnswers += " " + mQuizzBehaviour.ActualQuestion.Answers[i] + " [50] ";
            }

            string msg = mQuizzBehaviour.ActualQuestion.Question + " [200] " + lAnswers;
            Buddy.Vocal.Say(msg, (iOutput) => {
                if (!mAnswerGiven)
                    Buddy.Vocal.Listen();
            });
            yield return null;
        }

        private IEnumerator RepeatQuestion()
        {
            Buddy.Vocal.SayKey("canrepeat");
            yield return new WaitUntil(() => !Buddy.Vocal.IsBusy);
            yield return AskQuestion();
        }

        private void OnEndListening (SpeechInput iSpeechInput)
        {
            if (string.IsNullOrEmpty(iSpeechInput.Utterance))
            {
                //Buddy.Vocal.StopRecognition();
                Buddy.Vocal.Listen();
            }
            else
            {
                int lAnswerId = -1;
                if (iSpeechInput.Rule == "answers_" + mQuizzBehaviour.Lang + "#answer" /*&& iSpeechInput.Utterance.Contains(mQuizzBehaviour.ActualQuestion.Answers[mQuizzBehaviour.ActualQuestion.GoodAnswer])*/)
                {
                    for (int i = 0; i < mQuizzBehaviour.ActualQuestion.Answers.Count; i++)
                    {
                        if (iSpeechInput.Utterance.Trim().Contains(mQuizzBehaviour.RemoveSpecialCharacters(mQuizzBehaviour.ActualQuestion.Answers[i]).Trim()))
                            lAnswerId = i;
                    }
                }
                else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#first")
                    lAnswerId = 0;

                else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#second")
                    lAnswerId = 1;
                else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#third")
                    lAnswerId = 2;
                if (lAnswerId != -1)
                {
                    CheckAnswer(lAnswerId);
                }
                else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#repeat")
                {
                    StartCoroutine(RepeatQuestion());
                }
                else if (iSpeechInput.Rule == "commands_" + mQuizzBehaviour.Lang + "#quit")
                {
                    Trigger("Exit");
                }
                else
                {
                    Buddy.Vocal.SayKey("wronganswer", (iOutput) => {
                        StartCoroutine(AskQuestion());
                    });
                }
                    
            }
        }

        public void CheckAnswer(int lAnswerId)
        {
            mAnswerGiven = true;

            if (mQuizzBehaviour.ActualQuestion.GoodAnswer == lAnswerId)
                Trigger("Win");
            else
                Trigger("Lose");
        }

        public void ShowToast()
        {
            Buddy.GUI.Header.DisplayParametersButton(false);

            if (mQuizzBehaviour.ActualQuestion.Answers.Count > 2)
            {
                Buddy.GUI.Header.DisplayLightTitle(mQuizzBehaviour.ActualQuestion.Question);
                Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
                {
                    foreach (string answer in mQuizzBehaviour.ActualQuestion.Answers)
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        int i = mQuizzBehaviour.ActualQuestion.Answers.IndexOf(answer);
                        lBox.OnClick.Add(() => { iBuilder.Select(lBox); CheckAnswer(i); });
                        lBox.SetLabel(answer);                        
                        lBox.LeftButton.SetLabel((i+1).ToString());
                        lBox.SetCenteredLabel(true);
                        lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                    }
                });
            }
            else if (mQuizzBehaviour.ActualQuestion.Answers.Count == 2)
            {
                Buddy.GUI.Dialoger.Display<ParameterToast>().With((iBuilder) => { 
                    TText lText = iBuilder.CreateWidget<TText>();
                    lText.SetLabel(mQuizzBehaviour.ActualQuestion.Question);
                }, () => {
                    CheckAnswer(0);
                }, mQuizzBehaviour.ActualQuestion.Answers[0],
                () => {
                    CheckAnswer(1);
                }, mQuizzBehaviour.ActualQuestion.Answers[1]);
            }
        }

        private void OnStopListening(Touch[] iTouch)
        {
            Buddy.Vocal.OnEndListening.Clear();
        }

    }
}