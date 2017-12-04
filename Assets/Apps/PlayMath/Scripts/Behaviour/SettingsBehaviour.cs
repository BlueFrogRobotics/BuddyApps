using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
    public class SettingsBehaviour : AnimationSyncBehaviour	{
        [SerializeField]
		private Animator mPlayMathAnimator;

		private GameParameters mGameParameters;

		[SerializeField]
		private Sprite mSpriteStarOn;
		[SerializeField]
		private Sprite mSpriteStarOff;

		[SerializeField]
		private Image[] mButtonStars;

		[SerializeField]
		private Toggle mToggleAdd;
		[SerializeField]
		private Toggle mToggleSub;
		[SerializeField]
		private Toggle mToggleMulti;
		[SerializeField]
		private Toggle mToggleDiv;

        private Text mTitleTop;
        private Text mDifficultyLabel;
        private Text mResetScores;

		public void Start() {
			mGameParameters = User.Instance.GameParameters;

            mTitleTop = this.gameObject.transform.Find("Top_UI/Title_Top").GetComponent<Text>();
            mDifficultyLabel = this.gameObject.transform.Find("Middle_UI/Content/Text_level").GetComponent<Text>();
            mResetScores = this.gameObject.transform.Find("Bottom_UI/Button_Reset/Text").GetComponent<Text>();

            TranslateUI();

			BindOperands();
			BindStars();
		}

        private void TranslateUI()
        {
            mTitleTop.text = BYOS.Instance.Dictionary.GetString("settingstitle").ToUpper();
            mDifficultyLabel.text = BYOS.Instance.Dictionary.GetString("difficultylabel").ToUpper();
            mResetScores.text = BYOS.Instance.Dictionary.GetString("resetscoreslabel").ToUpper();

            Text lText = mToggleAdd.gameObject.transform.Find("Label").GetComponent<Text>();
            lText.text = BYOS.Instance.Dictionary.GetString("additionlabel").ToUpper();

            lText = mToggleSub.gameObject.transform.Find("Label").GetComponent<Text>();
            lText.text = BYOS.Instance.Dictionary.GetString("subtractionlabel").ToUpper();

            lText = mToggleMulti.gameObject.transform.Find("Label").GetComponent<Text>();
            lText.text = BYOS.Instance.Dictionary.GetString("multiplylabel").ToUpper();

            lText = mToggleDiv.gameObject.transform.Find("Label").GetComponent<Text>();
            lText.text = BYOS.Instance.Dictionary.GetString("divisionlabel").ToUpper();
        }

        public void OnClickResetScores() {
            User.Instance.ResetScores();
            User.SaveUser();
        }

		public void OnClickGoToMenu() {
			if (! CheckSettings()) {
				Utils.LogI(LogInfo.UNAUTHORIZED, "Can't close the view with these settings");
				return;
			}

            User.SaveUser();

			mPlayMathAnimator.SetTrigger("BackToMenu");
		}		

		public void OnClickPlay() {
			if (! CheckSettings()) {
				Utils.LogI(LogInfo.UNAUTHORIZED, "Can't close the view with these settings");
				return;
			}
            mGameParameters.Table = 0;
            mGameParameters.Sequence = 4;
			
            User.SaveUser();

			mPlayMathAnimator.SetTrigger("Play");
		}

		public void OnClickStar1() {
			OnChangeDifficulty(1);
		}

		public void OnClickStar2() {
			OnChangeDifficulty(2);
		}

		public void OnClickStar3() {
			OnChangeDifficulty(3);
		}

		public void OnClickStar4() {
			OnChangeDifficulty(4);
		}

		public void OnClickStar5() {
			OnChangeDifficulty(5);
		}

		public void OnChangedAdd(bool value) {
			// if difficulty > 2 then ADD = on and SUB = on
			if ((!value) && this.mGameParameters.Difficulty > 2) {
				mToggleAdd.isOn = true;
				return;
			}

			OnChangeOperands(Operand.ADD, value);
		}		

		public void OnChangedSub(bool value) {
			// if difficulty > 2 then ADD = on and SUB = on
			if ((!value) && this.mGameParameters.Difficulty > 2) {
				mToggleSub.isOn = true;
				return;
			}

			OnChangeOperands(Operand.SUB, value);
		}

		public void OnChangedMutli(bool value) {
			OnChangeOperands(Operand.MULTI, value);
		}

		public void OnChangedDiv(bool value) {
			OnChangeOperands(Operand.DIV, value);
		}

		private void OnChangeDifficulty(int newDifficulty) {
			mGameParameters.Difficulty = newDifficulty;
			BindStars();

			// if difficulty > 2 then ADD = on and SUB = on
			if (this.mGameParameters.Difficulty > 2) {
				if (!mToggleAdd.isOn) {
					mToggleAdd.isOn = true;
					mGameParameters.SetOperand(Operand.ADD, true);
				}

				if (!mToggleSub.isOn) {
					mToggleSub.isOn = true;
					mGameParameters.SetOperand(Operand.SUB, true);
				}
			}
		}

		private void BindStars() {
			for (int i = 0; i < mButtonStars.Length; i++) {
				mButtonStars[i].sprite = i < mGameParameters.Difficulty ? mSpriteStarOn : mSpriteStarOff;
			}
		}

		private void OnChangeOperands(Operand operand, bool value) {
			mGameParameters.SetOperand(operand, value);
		}

		private void BindOperands() {
			mToggleAdd.isOn = mGameParameters.CheckOperand(Operand.ADD);
			mToggleSub.isOn = mGameParameters.CheckOperand(Operand.SUB);
			mToggleMulti.isOn = mGameParameters.CheckOperand(Operand.MULTI);
			mToggleDiv.isOn = mGameParameters.CheckOperand(Operand.DIV);
		}

		private bool CheckSettings() {
			if (mGameParameters.CheckOperand(Operand.NONE)) {
				return false;
			}

			return true;
		}
	}
}

