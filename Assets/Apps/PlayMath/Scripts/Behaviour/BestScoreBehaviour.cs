using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.PlayMath{
    public class BestScoreBehaviour : AnimationSyncBehaviour {
		
		private Animator mPlayMathAnimator;

		[SerializeField]
		private GameObject mViewportContent;

		[SerializeField]
		private GameObject mScore1;
		[SerializeField]
		private GameObject mScore2;
		[SerializeField]
		private GameObject mScore3;
		[SerializeField]
		private GameObject mScoreOther;
		[SerializeField]
		private Sprite mSpriteStarEmpty;
		[SerializeField]
		private Sprite mSpriteStarFull;
		[SerializeField]
		private Color mColorStarFull1;
		[SerializeField]
		private Color mColorStarEmpty1;
		[SerializeField]
		private Color mColorStarFull2;
		[SerializeField]
		private Color mColorStarEmpty2;
		[SerializeField]
		private Color mColorStarFull3;
		[SerializeField]
		private Color mColorStarEmpty3;
		[SerializeField]
		private Color mColorStarFullOther;
		[SerializeField]
		private Color mColorStarEmptyOther;

		private string mUserName;
		private int mDifficulty;

		void Start() {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
		}

		public void OnClickGoToMenu() {
			mPlayMathAnimator.SetTrigger("BackToMenu");
		}

		public void DisplayScore() {
			CleanViewport();

			User lUser = User.Instance;
			mDifficulty = lUser.GameParameters.Difficulty;
			mUserName = lUser.Name;

			List<ScoreSummary> lScores = User.Instance.Scores.ScoresByLevels[mDifficulty - 1];
			for (int i = 0; i < lScores.Count; i++)
				DisplayResult(lScores[i], i);
		}

		private void CleanViewport() {
            LayoutElement[] childs = mViewportContent.GetComponentsInChildren<LayoutElement>();
            foreach (LayoutElement child in childs)
				GameObject.Destroy(child.gameObject);
		}

		private void DisplayResult(ScoreSummary scoreSummary, int index) {
			GameObject lDisplay;
			switch (index) {
			case 0:
				lDisplay = Instantiate(mScore1);
				break;
			case 1:
				lDisplay = Instantiate(mScore2);
				break;
			case 2:
				lDisplay = Instantiate(mScore3);
				break;
			default:
				lDisplay = Instantiate(mScoreOther);
				break;
			}

			lDisplay.name = "Result_" + index;

			Text[] lTextComponent = lDisplay.GetComponentsInChildren<Text>();
			foreach (Text text in lTextComponent)
			{
				if (text.gameObject.name == "User_Name")
					text.text = mUserName;
				else if (text.gameObject.name == "User_Score")
					text.text = scoreSummary.CorrectAnswers + "/" + (scoreSummary.BadAnswers + scoreSummary.CorrectAnswers);
				else if (text.gameObject.name == "User_Time")
					text.text = "in " + ((int)scoreSummary.TotalAnswerTime.TotalSeconds) + "s";
			}

			Image[] lImageComponent = lDisplay.GetComponentsInChildren<Image>();
			foreach (Image image in lImageComponent)
			{
				int lStarDifficulty = 0;
				if (image.gameObject.name == "Star_01")
					lStarDifficulty = 1;
				if (image.gameObject.name == "Star_02")
					lStarDifficulty = 2;
				if (image.gameObject.name == "Star_03")
					lStarDifficulty = 3;
				if (image.gameObject.name == "Star_04")
					lStarDifficulty = 4;
				if (image.gameObject.name == "Star_05")
					lStarDifficulty = 5;

				if (lStarDifficulty == 0) {
					continue;
				}

				image.sprite = lStarDifficulty <= this.mDifficulty ? mSpriteStarFull : mSpriteStarEmpty;

				switch (index) {
				case 0:
					image.color = lStarDifficulty <= this.mDifficulty ? mColorStarFull1 : mColorStarEmpty1;
					break;
				case 1:
					image.color = lStarDifficulty <= this.mDifficulty ? mColorStarFull2 : mColorStarEmpty2;
					break;
				case 2:
					image.color = lStarDifficulty <= this.mDifficulty ? mColorStarFull3 : mColorStarEmpty3;
					break;
				default:
					image.color = lStarDifficulty <= this.mDifficulty ? mColorStarFullOther : mColorStarEmptyOther;
					break;
				}
			}

			lDisplay.transform.SetParent(mViewportContent.transform,false);
		}
	}
}
