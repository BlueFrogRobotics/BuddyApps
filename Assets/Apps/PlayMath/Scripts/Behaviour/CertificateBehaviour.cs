using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class CertificateBehaviour : MonoBehaviour {

		private Animator mPlayMathAnimator;

		void Start() {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
		}

		public void OnClickGoToMenu() {
			mPlayMathAnimator.SetTrigger("BackToMenu");
		}

		public void OnClickPlay() {
			mPlayMathAnimator.SetTrigger("Play");
		}
	}
}
