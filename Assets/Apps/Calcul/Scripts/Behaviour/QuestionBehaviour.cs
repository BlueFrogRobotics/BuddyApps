using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using BlueQuark;

namespace BuddyApp.Calcul
{
    public class QuestionBehaviour : MonoBehaviour
    {
        private Result mResult;
        private Score mScore;

		private Generator mEquationGenerator;
        private int mCountQuestions;
        private DateTime mStartTime;

        private GameParameters mGameParams;

        public bool HasAnswer{ get; private set;}

        void Start()
        {
            mGameParams = User.Instance.GameParameters;
            mScore = GetComponent<CalculBehaviour>().Score;

            Buddy.Vocal.DefaultInputParameters = new SpeechInputParameters
            {
                Grammars = new[] { "playmath_question" }
            };
        }

        /// <summary>Resets parameter to start a new game.</summary>
        public void ResetGame()
        {
            mCountQuestions = 0;

            if (mGameParams.Table > 0)
                mEquationGenerator = new MultiplicationGenerator(mGameParams);
            else
                mEquationGenerator = new EquationGenerator(mGameParams);

            mEquationGenerator.generate();

            mScore.ResetScore();
        }

        /// <summary>Initializes the state for a new question in the game.</summary>
        public void InitState()
        {
            HasAnswer = false;
            mStartTime = DateTime.Now;

            Buddy.Vocal.OnEndListening.Clear();
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.OnEndListening.Add(OnEndListening);

            Buddy.GUI.Screen.OnTouch.Add(OnStopListening);

            mResult = new Result();
        }

        //Ask next question and handle associated text to display
        public void AskNextQuestion()
        {
            Equation lEquation = mEquationGenerator.Equations[mCountQuestions];

            mResult.Equation = lEquation.Text;
            mResult.CorrectAnswer = lEquation.Answer;

            mCountQuestions++;

            // Is this question the last ?
            mResult.Last = (mCountQuestions == mEquationGenerator.Equations.Count);

            string title = String.Format(Buddy.Resources.GetString("howmanydoes"), mResult.Equation);
            Buddy.GUI.Header.DisplayLightTitle(title.ToUpper());

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                for (int i = 0; i < lEquation.Choices.Length; i++)
                {
                    string choice = lEquation.Choices[i];

                    TVerticalListBox lBox = iBuilder.CreateBox();
                    lBox.SetLabel("<size=60>" + lEquation.Choices[i] + "</size>");
                    //lBox.LeftButton.SetLabel("*");
                    lBox.SetCenteredLabel(true);
                    Sprite sprite = Buddy.Resources.Get<Sprite>("os_grey_star", Context.OS);
                    lBox.LeftButton.SetIcon(sprite);
                    //lBox.LeftButton.Hide();
                    lBox.OnClick.Add(() => { iBuilder.Select(lBox); OnClick(choice); });
                }
            });            

            string text = CalculBehaviour.GetVocalEquation(title);
            Buddy.Vocal.Say(text, (iOutput) =>
            {
                Buddy.Vocal.Listen();
            });
        }

        public double ElapsedTimeSinceStart()
        {
            TimeSpan elapsed = DateTime.Now - mStartTime;

            return elapsed.TotalSeconds;
        }

        public void TimeOut()
        {
            StoreResult("-");
            HasAnswer = true;
        }

        public void OnClick(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                StoreResult(result);
                HasAnswer = true;
            }
        }

        private void StoreResult(string answer)
        {
            mResult.UserAnswer = answer;
            mResult.ElapsedTime = ElapsedTimeSinceStart();
            mScore.AddResult(mResult);
            Debug.Log("Answer " + answer);

            Clear();
        }

        public void Clear()
        {
            // Hide GUI
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Dialoger.Hide();
            Buddy.GUI.Screen.OnTouch.Remove(OnStopListening);

            // Ends vocal recognition coroutine
            Buddy.Vocal.StopAndClear();
            Buddy.Vocal.OnEndListening.Clear();
        }

        public void OnEndListening(SpeechInput iSpeech)
        {
            // in case an anwser has already been given by touch screen
            if (HasAnswer)
                return;

            if (!string.IsNullOrEmpty(iSpeech.Utterance))
            {
                // extract response from any sentence
                string answer = ExtractNumber(iSpeech.Utterance);

                if (answer != "")
                {
                    StoreResult(answer);
                    HasAnswer = true;
                    return;
                }
            }
            // if not utterance or no number in it, listen again
            Buddy.Vocal.Listen();
        }

        private string ExtractNumber(string iSpeech)
        {
            // retrieve any integer first occurence
            string match = Regex.Match(iSpeech, @"-?\d+").Value;

            // times to times, negative sign are textually given in the answer
            if (iSpeech.Contains("minus") || iSpeech.Contains("moins"))
            {
                match = "-" + match;
            }

            return match;
        }

        /// <summary>Stops listening on touch.</summary>
        private void OnStopListening(Touch[] iTouch)
        {
            Buddy.Vocal.OnEndListening.Clear();
        }
    }
}
