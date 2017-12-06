using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class ScoreBehaviour : AnimationSyncBehaviour {

        [SerializeField]
		private Animator mPlayMathAnimator;
        [SerializeField]
        private Score mScore;

        [SerializeField]
        private GameObject mGoodResult;
        [SerializeField]
        private GameObject mBadResult;
        [SerializeField]
        private GameObject mViewportContent;

        private Text mTitleTop;
        private Text mShare;

        private int mResultIndex;

        void Start()
        {
            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mShare = this.gameObject.transform.Find("Bottom_UI/Button_Share/Text").GetComponent<Text>();
        }

        public void DisplayScore()
        {
            CleanViewport();

            if (mScore.SuccessPercent() == 1.0)
            {
                mTitleTop.text = BYOS.Instance.Dictionary.GetString("certificatetitle");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("perfectscore");
            }
            else if (mScore.SuccessPercent() >= 0.75)
            {
                if (mScore.BadAnswers == 1)
                    mTitleTop.text = String.Format(BYOS.Instance.Dictionary.GetString("onebadanswertitle").ToUpper());
                else
                    mTitleTop.text = String.Format(BYOS.Instance.Dictionary.GetString("greatscoretitle").ToUpper(),
                        mScore.BadAnswers);

                BYOS.Instance.Interaction.TextToSpeech.SayKey("greatscorespeech");
            }
            else if (mScore.SuccessPercent() >= 0.5)
            {
                mTitleTop.text = BYOS.Instance.Dictionary.GetString("goodscoretitle");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("goodscorespeech");
            }
            else
            {
                mTitleTop.text = BYOS.Instance.Dictionary.GetString("badscoretitle");
                BYOS.Instance.Interaction.TextToSpeech.SayKey("badscorespeech");
            }

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

        public void TranslateUI()
        {
            mShare.text = BYOS.Instance.Dictionary.GetString("sharelabel").ToUpper();
        }
	}
}
