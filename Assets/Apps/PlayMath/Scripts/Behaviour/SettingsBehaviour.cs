using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using Buddy;

namespace BuddyApp.PlayMath{
	public class SettingsBehaviour : MonoBehaviour
	{

		private Animator mPlayMathAnimator;

		private GameParameters mGameParameters = User.Instance.GameParameters;

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

		public void Start() {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();

			BindOperands();
			BindStars();
		}

		public void OnClickGoToMenu() {
			if (! CheckSettings ()) {
				Utils.LogI(LogInfo.UNAUTHORIZED, "Can't close the view with these settings");
				return;
			}

			mPlayMathAnimator.SetTrigger("BackToMenu");
		}		

		public void OnClickPlay() {
			if (! CheckSettings ()) {
				Utils.LogI(LogInfo.UNAUTHORIZED, "Can't close the view with these settings");
				return;
			}

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
			OnChangeOperands(Operand.ADD, value);
		}		

		public void OnChangedSub(bool value) {
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
			LogValues();
		}

		private void BindStars() {
			for (int i = 0; i < mButtonStars.Length; i++) {
				mButtonStars[i].sprite = i < mGameParameters.Difficulty ? mSpriteStarOn : mSpriteStarOff;
			}
		}

		private void OnChangeOperands(Operand operand, bool value) {
			mGameParameters.SetOperand(operand, value);
			LogValues();
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

		// TODO remove
		private void LogValues() {
			Utils.LogI(LogInfo.SUCCESSED, "gameParameters: " + mGameParameters);
		}
	}
}

