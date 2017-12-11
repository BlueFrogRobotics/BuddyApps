﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Buddy;

namespace BuddyApp.PlayMath{
    public class QuestionBehaviour : AnimationSyncBehaviour {

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
        private bool mAnnounced;

        private GameParameters mGameParams;

        public bool HasAnswer{ get; private set;}
        private TimeSpan mElapsedTime;

        private bool mLaunchSTTOnce;
        private List<string> mSTTChoices;

        private Dictionary mDictionary;
        private VocalManager mVocalManager;
        private TextToSpeech mTTS;

        private bool mTriggerOnce;

		void Start() {

            mDictionary = BYOS.Instance.Dictionary;
            mVocalManager = BYOS.Instance.Interaction.VocalManager;
            mTTS = BYOS.Instance.Interaction.TextToSpeech;

            mChoices = GameObject.Find("UI/Four_Answer/Middle_UI").GetComponentsInChildren<Text>();
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mTitleBottom = this.gameObject.transform.Find("Bottom_UI/Title_Bottom").GetComponent<Text>();
            // Disable VocalManager trigger mode
            mVocalManager.EnableTrigger = false;
            // Define VocalManager STT Callback
            mVocalManager.OnEndReco = SpeechToTextCallback;
            // Define VocalManager STT Error Callback
            mVocalManager.OnError = ErrorCallback;
            mSTTChoices = new List<string>();
		}

        public void InitState()
        {
            mTriggerOnce = true;
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

            mCountQuestions++;
            mTitleBottom.text = String.Format(mDictionary.GetString("questioniteration"), mCountQuestions, mGameParams.Sequence);

            // Is this question the last ?
			mResult.Last = (mCountQuestions == mEquationGenerator.Equations.Count);

            mTitleTop.text = String.Format(mDictionary.GetString("howmanydoes"), mResult.Equation);
            AnnounceEquation();

            mSTTChoices.Clear();
            for (int i = 0; i < mChoices.Length; i++)
            {
                mChoices[i].text = lEquation.Choices[i];
                mSTTChoices.Add(lEquation.Choices[i]);
            }

            mAnnounced = false;
            HasAnswer = false;
            StartCoroutine(WaitAnnouncement());
        }

        public double ElapsedTimeSinceStart()
        {
            TimeSpan elapsed = TimeSpan.Zero;

            if (mAnnounced)
                elapsed = DateTime.Now - mStartTime;

            return elapsed.TotalSeconds;
        }

        public void TimeOut()
        {
            HasAnswer = true;
            ShowResult("-");
        }

        public void OnClick(BaseEventData data)
        {
            GameObject lSelected = data.selectedObject;
            if (lSelected != null && mTriggerOnce)
            {
                HasAnswer = true;
                mElapsedTime = DateTime.Now - mStartTime;
                mTriggerOnce = false;
                Text lTextComponent = lSelected.GetComponentInChildren<Text>();
                ShowResult(lTextComponent.text);
            }
        }

        private void ShowResult(string answer)
        {
            mResult.UserAnswer = answer;
            mResult.ElapsedTime = mElapsedTime.TotalSeconds;

            mPlayMathAnimator.SetTrigger("Result");
        }

        private void AnnounceEquation()
        {
            string statement = (string) mTitleTop.text.Clone();

            if (statement.Contains("÷"))
                statement = statement.Replace("÷", mDictionary.GetString("dividedby"));
            if (statement.Contains("×"))
                statement = statement.Replace("×", mDictionary.GetString("xtimesy"));
            if (statement.Contains("-"))
                statement = statement.Replace("-", mDictionary.GetString("minus"));

            mTTS.Say(statement);
        }

        private IEnumerator EnableSpeechToText()
        {
            mLaunchSTTOnce = false;
            while (!HasAnswer)
            {
                if (!mLaunchSTTOnce)
                {
                    if (!mVocalManager.RecognitionFinished)
                    {
                        // Recognition not finished yet, waiting until it ends cleanly
                        yield return new WaitUntil(() => mVocalManager.RecognitionFinished);
                    }
                    else
                    {
                        // Initiating Vocal Manager instance reco
                        mLaunchSTTOnce = true;
                        mVocalManager.StartInstantReco(false);
                    }
                }
                yield return null;
            }
        }

        public void SpeechToTextCallback(string iSpeech)
        {
            // times to times, negative sign are textually given in the answer
            iSpeech = iSpeech.Replace("moins", "-");
            iSpeech = iSpeech.Replace("minus", "-");

            // if the result is negative, remove potential space between '-' and the number
            if (iSpeech.Contains("-"))
                iSpeech = iSpeech.Replace(" ","");

            if (mSTTChoices.Contains(iSpeech))
            {
                HasAnswer = true;
                mElapsedTime = DateTime.Now - mStartTime;
                // Pause before annoncing the result (STT notification "I hear...")
                BYOS.Instance.Interaction.TextToSpeech.Silence(1000, true);
                ShowResult(iSpeech);
            }
            else
                Debug.LogWarning("Speech : given answer not in offered choices, restarting vocal reco asap");

            mLaunchSTTOnce = false;
        }

        public void ErrorCallback(STTError iError)
        {
            StartCoroutine(InterruptBuddySpeech());
        }

        private IEnumerator InterruptBuddySpeech()
        {
            yield return new WaitUntil(() => mTTS.IsSpeaking);
            mTTS.Stop();
            mLaunchSTTOnce = false;
        }

        private IEnumerator WaitAnnouncement()
        {
            yield return new WaitUntil(() => mTTS.HasFinishedTalking);
            
            mStartTime = DateTime.Now;
            mElapsedTime = TimeSpan.Zero;

            mAnnounced = true;

            StartCoroutine(EnableSpeechToText());
        }

        public void OnClickGoToMenu() {
            if (mTriggerOnce)
            {
                mTriggerOnce = false;
                mPlayMathAnimator.SetTrigger("BackToMenu");
            }
        }
   	}
}
