﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;


namespace BuddyApp.CalculGame
{
	public class QuitGame : SpeechStateBehaviour
	{

		private float mTimer;

		List<string> quitWords;

		private AnimManager mAnimationManager;

		public override void Init()
		{
			mAnimationManager = GetComponentInGameObject<AnimManager>(0);

			if (BYOS.Instance.LanguageManager.CurrentLang == Language.FRA) {
				mSynonymesFile = "calculs_dialogs_fr.xml";
			} else {
				mSynonymesFile = "calculs_dialogs_en.xml";
			}
		}

		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTimer = 0.0f;

			quitWords = new List<string>();
			FillListSyn("QuitGame", quitWords);

			mMood.Set(MoodType.SAD);
			mAnimationManager.Sigh();
			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			mTTS.Silence(500, true);
			mTTS.Say(RdmStr(quitWords), true);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTimer += Time.deltaTime;

			if (mTTS.HasFinishedTalking && mTimer > 2.0f) {
				//link.UnLoadScene ();
			}
		}

	}
}