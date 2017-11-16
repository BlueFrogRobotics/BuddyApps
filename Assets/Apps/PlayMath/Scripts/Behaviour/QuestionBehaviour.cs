using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BuddyApp.PlayMath{
	public class QuestionBehaviour : MonoBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Animator mQuestionAnimator;
        [SerializeField]
        private Result mResult;
        [SerializeField]
        private Text mTitleTop;
        [SerializeField]
        private Text mTitleBottom;

        private Text[] mChoices;
        private int mCountQuestions;
        private DateTime mStartTime;

        //TODO Replace the following with GameParameters value
        private int mTotalQuestions;

		void Start() {
            mChoices = GameObject.Find("UI/Four_Answer/Middle_UI").GetComponentsInChildren<Text>();
            mTotalQuestions = 4;
		}

        public void ResetGame()
        {
            mCountQuestions = 0;
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
            mTitleBottom.text = "QUESTION " + mCountQuestions + " OF " + mTotalQuestions;
            mResult.Last = (mCountQuestions == mTotalQuestions);

            mTitleTop.text = "HOW MANY DOES " + mResult.Equation + " ?";

            //TODO Replace the following with generated Equation choices
            string[] lChoices = {"2","3","4","6"};
            for (int i = 0; i < mChoices.Length; i++)
                mChoices[i].text = lChoices[i];

            mStartTime = DateTime.Now;
        }

        public double ElapsedTimeSinceStart()
        {
            TimeSpan elapsed = DateTime.Now - mStartTime;
            return elapsed.TotalSeconds;
        }

        public void TimeOut()
        {
            ShowResult("TimeOut");
        }

        public void OnClick(BaseEventData data)
        {
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
            TimeSpan elapsed = DateTime.Now - mStartTime;
            mResult.ElapsedTime = elapsed.TotalSeconds;

            mPlayMathAnimator.SetTrigger("Result");
        }
   	}
}
