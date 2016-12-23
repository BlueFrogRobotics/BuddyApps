using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;


namespace BuddyApp.CalculGame
{
	public class CalculGameIntro : SpeechStateBehaviour
	{

		private float mTTSTimer;

		// list dialogs strings
		private List<string> introWords;

		private AnimManager mAnimationManager;

		public override void Init()
		{

			mAnimationManager = GetComponentInGameObject<AnimManager>(0);

			//Set the number of games:

			CommonIntegers["nbLevels"] = 3;


			if (BYOS.Instance.VocalActivation.CurrentLanguage == Language.FRA) {
				mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_fr.xml").text;
			} else {
				mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_en.xml").text;
			}

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// init timer
			mTTSTimer = 0.0f;

			// init dialog list
			introWords = new List<string>();

			// fill dialog list from xml file
			FillListSyn("Intro", introWords);

			// play sound, anim and start talking
			BYOS.Instance.SoundManager.Play(SoundType.RANDOM_LAUGH);
			mAnimationManager.Smile();
			mTTS.Silence(1000, true);
			mTTS.Say(RdmStr(introWords), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTTSTimer += Time.deltaTime;

			// once the tts is done, set trigger to go to the next state
			if (mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
				animator.SetTrigger("IntroDone");
			}
		}

		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}