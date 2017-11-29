using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
	public class QuestionBehaviour : MonoBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Animator mQuestionAnimator;
        [SerializeField]
        private Result mResult;
        [SerializeField]
        private Score mScore;

        private Text mTitleTop;
        private Text mTitleBottom;
        private Text[] mChoices;

		private Generator mEquationGenerator;
        private int mCountQuestions;
        private DateTime mStartTime;

        private GameParameters mGameParams;

        public bool HasAnswer{ get; private set;}
        private TimeSpan mElapsedTime;

        private bool mLaunchSTTOnce;
        private List<string> mSTTChoices;

		void Start() {
            mChoices = GameObject.Find("UI/Four_Answer/Middle_UI").GetComponentsInChildren<Text>();
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mTitleBottom = this.gameObject.transform.Find("Bottom_UI/Title_Bottom").GetComponent<Text>();
            // Disable VocalManager trigger mode
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            // Define VocalManager STT Callback
            BYOS.Instance.Interaction.VocalManager.OnEndReco = SpeechToTextCallback;
            mSTTChoices = new List<string>();
		}

        public void ResetGame()
        {
            mCountQuestions = 0;
            mGameParams = User.Instance.GameParameters;

            if (mGameParams.Table > 0)
                mEquationGenerator = new MultiplicationGenerator(mGameParams);
            else
                mEquationGenerator = new EquationGenerator(mGameParams);

            mEquationGenerator.generate();

            mScore.ResetScore();
        }

        //Ask next question and handle associated text to display
        public void AskNextQuestion()
        {
			Equation lEquation = mEquationGenerator.Equations[mCountQuestions];

			mResult.Equation = lEquation.Text;
			mResult.CorrectAnswer = lEquation.Answer;

            // Is this question the last ?
            mCountQuestions++;
            mTitleBottom.text = String.Format(BYOS.Instance.Dictionary.GetString("questioniteration"), mCountQuestions, mGameParams.Sequence);

			mResult.Last = (mCountQuestions == mEquationGenerator.Equations.Count);

            mTitleTop.text = String.Format(BYOS.Instance.Dictionary.GetString("howmanydoes"), mResult.Equation);
            AnnounceEquation();

            mSTTChoices.Clear();
            for (int i = 0; i < mChoices.Length; i++)
            {
                mChoices[i].text = lEquation.Choices[i];
                mSTTChoices.Add(lEquation.Choices[i]);
            }

            mStartTime = DateTime.Now;
            HasAnswer = false;
            mElapsedTime = TimeSpan.Zero;

            mLaunchSTTOnce = false;
            StartCoroutine(EnableSpeechToText());
        }

        public double ElapsedTimeSinceStart()
        {
            TimeSpan elapsed = DateTime.Now - mStartTime;
            return elapsed.TotalSeconds;
        }

        public void TimeOut()
        {
            HasAnswer = true;
            ShowResult("-");
        }

        public void OnClick(BaseEventData data)
        {
            HasAnswer = true;
            mElapsedTime = DateTime.Now - mStartTime;

            GameObject lSelected = data.selectedObject;
            if (lSelected != null)
            {
                Text lTextComponent = lSelected.GetComponentInChildren<Text>();
                ShowResult(lTextComponent.text);
            }
        }

        private void ShowResult(string answer)
        {
            // Disable STT trigger
            BYOS.Instance.Interaction.VocalManager.EnableTrigger = false;
            mResult.UserAnswer = answer;
            mResult.ElapsedTime = mElapsedTime.TotalSeconds;

            mPlayMathAnimator.SetTrigger("Result");
        }

        private void AnnounceEquation()
        {
            string statement = (string) mTitleTop.text.Clone();

            if (statement.Contains("÷"))
                statement = statement.Replace("÷", BYOS.Instance.Dictionary.GetString("dividedby"));
            if (statement.Contains("×"))
                statement = statement.Replace("×", BYOS.Instance.Dictionary.GetString("xtimesy"));
            if (statement.Contains("-"))
                statement = statement.Replace("-", BYOS.Instance.Dictionary.GetString("minus"));

            BYOS.Instance.Interaction.TextToSpeech.Say(statement);
        }

        private IEnumerator EnableSpeechToText()
        {
            while (!HasAnswer)
            {
                if (!mLaunchSTTOnce)
                {
                    BYOS.Instance.Interaction.VocalManager.StartInstantReco();
                    mLaunchSTTOnce = true;
                }
                yield return null;
            }
        }

        public void SpeechToTextCallback(string iSpeech)
        {
            Debug.Log("SpeechToText : " + iSpeech);

            if (mSTTChoices.Contains(iSpeech))
            {
                HasAnswer = true;
                mElapsedTime = DateTime.Now - mStartTime;
                ShowResult(iSpeech);
            }

            mLaunchSTTOnce = false;
        }
   	}
}
