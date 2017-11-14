﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.PlayMath{
	public class SettingsBehaviour : MonoBehaviour
	{

		private Animator mPlayMathAnimator;

		public void Start() {
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

