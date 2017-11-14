using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class ScoreBehaviour : MonoBehaviour {

		private Animator mPlayMathAnimator;

		void Start () {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
		}

		public void OnClickGoToMenu() {
			mPlayMathAnimator.SetTrigger("BackToMenu");
		}

		public void OnClickReplay() {
			mPlayMathAnimator.SetTrigger("Play");
		}
	}
}
