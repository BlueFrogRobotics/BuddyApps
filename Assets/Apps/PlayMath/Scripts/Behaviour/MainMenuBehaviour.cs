using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class MainMenuBehaviour : MonoBehaviour
	{

		private Animator mPlayMathAnimator;

		public void Start() {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
		}

		public void OnClickSettings() {
			mPlayMathAnimator.SetTrigger("GameSettings");
		}

		public void OnClickBestScores() {
			mPlayMathAnimator.SetTrigger("BestScore");
		}

		public void OnClickPlay() {
			mPlayMathAnimator.SetTrigger("Play");
		}
	}
}

