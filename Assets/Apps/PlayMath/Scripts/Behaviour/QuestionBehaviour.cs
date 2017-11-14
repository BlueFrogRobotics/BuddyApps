using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class QuestionBehaviour : MonoBehaviour {

		enum Answer { Answer1, Answer2, Answer3, Answer4 };

		private Animator mPlayMathAnimator;

		void Start() {
			mPlayMathAnimator = GameObject.Find("AIBehaviour").GetComponent<Animator>();
		}

		public void OnClickAnswer1() {
			OnClickAnswer(Answer.Answer1);
		}

		public void OnClickAnswer2() {
			OnClickAnswer(Answer.Answer2);
		}

		public void OnClickAnswer3() {
			OnClickAnswer(Answer.Answer3);
		}

		public void OnClickAnswer4() {
			OnClickAnswer(Answer.Answer4);
		}

		private void OnClickAnswer(Answer answer) {
			mPlayMathAnimator.SetTrigger("Result");
		}
	}
}
