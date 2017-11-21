﻿using System;
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

        private int mCountQuestions;
        private DateTime mStartTime;

        private GameParameters mGameParams;

        public bool HasAnswer{ get; private set;}
        private TimeSpan mElapsedTime;

		void Start() {
            mChoices = GameObject.Find("UI/Four_Answer/Middle_UI").GetComponentsInChildren<Text>();
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mTitleBottom = this.gameObject.transform.Find("Bottom_UI/Title_Bottom").GetComponent<Text>();
		}

        public void ResetGame()
        {
            mCountQuestions = 0;
            mGameParams = User.Instance.GameParameters;

            mScore.ResetScore();
        }

        //Generate a new equation and handle associated text to display
        public void GenerateEquation()
        {
            if (mQuestionAnimator.GetBool("InitGame"))
            {
                ResetGame();
                mQuestionAnimator.SetBool("InitGame", false);
            }
            //TODO Replace the following with generated equation and associated answer
            mResult.Equation = "(6+2)/4";
            mResult.CorrectAnswer = "2";

            // Is this question the last ?
            mCountQuestions++;
            mTitleBottom.text = String.Format(BYOS.Instance.Dictionary.GetString("questioniteration"), mCountQuestions, mGameParams.Sequence);
            mResult.Last = (mCountQuestions == mGameParams.Sequence);

            mTitleTop.text = String.Format(BYOS.Instance.Dictionary.GetString("howmanydoes"), mResult.Equation);

            //TODO Replace the following with generated Equation choices
            string[] lChoices = {"2","3","4","6"};
            for (int i = 0; i < mChoices.Length; i++)
                mChoices[i].text = lChoices[i];

            mStartTime = DateTime.Now;
            HasAnswer = false;
            mElapsedTime = TimeSpan.Zero;
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
            mResult.UserAnswer = answer;
            mResult.ElapsedTime = mElapsedTime.TotalSeconds;

            mPlayMathAnimator.SetTrigger("Result");
        }
   	}
}
