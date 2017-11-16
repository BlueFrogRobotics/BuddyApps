using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.PlayMath{
	public class ScoreBehaviour : MonoBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Score mScore;
        [SerializeField]
        private Text mTitleTop;
        [SerializeField]
        private GameObject mGoodResult;
        [SerializeField]
        private GameObject mBadResult;
        [SerializeField]
        private GameObject mViewportContent;

        private int mResultIndex;

        public void DisplayScore()
        {
            CleanViewport();

            if (mScore.SuccessPercent() >= 0.75)
                mTitleTop.text = "NICE SCORE !";
            else if (mScore.SuccessPercent() >= 0.5)
                mTitleTop.text = "NOT BAD !";
            else
                mTitleTop.text = "I KNOW YOU CAN DO BETTER !";

            mResultIndex = 1;
            foreach (Result lResult in mScore.Results)
                DisplayResult(lResult);
        }

		public void OnClickGoToMenu() {
			mPlayMathAnimator.SetTrigger("BackToMenu");
		}

		public void OnClickReplay() {
			mPlayMathAnimator.SetTrigger("Play");
		}

        private void CleanViewport()
        {
            Image[] childs = mViewportContent.GetComponentsInChildren<Image>();
            foreach (Image child in childs)
                GameObject.Destroy(child.gameObject);
        }

        private void DisplayResult(Result lResult)
        {
            GameObject lDisplay;
            if (lResult.isCorrect())
                lDisplay = Instantiate(mGoodResult);
            else
                lDisplay = Instantiate(mBadResult);

            lDisplay.name = "Result_" + mResultIndex++;

            Text[] lTextComponent = lDisplay.GetComponentsInChildren<Text>();
            foreach (Text text in lTextComponent)
            {
                if (text.gameObject.name == "Answer")
                    text.text = "YOUR ANSWER : " + lResult.UserAnswer;
                else if (text.gameObject.name == "Question")
                    text.text = lResult.Equation + " = " + lResult.CorrectAnswer;
                else
                    Debug.LogError("Unexpected gameObject name in Score display");
            }

            lDisplay.transform.SetParent(mViewportContent.transform,false);
        }
	}
}
