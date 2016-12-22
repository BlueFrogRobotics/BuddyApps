using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;


namespace BuddyApp.CalculGame
{
	public class FailCalculGame : SpeechStateBehaviour
	{


		private List<string> tooBadWords;
		private List<string> goodAnswerWords;

		private AnimManager mAnimationManager;

		public override void Init()
		{
			mAnimationManager = GetComponentInGameObject<AnimManager>(0);



			if (BYOS.Instance.VocalActivation.CurrentLanguage == Language.FRA) {
				mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_fr.xml").text;
			} else {
				mSynonymesFile = Resources.Load<TextAsset>("calculs_dialogs_en.xml").text;
			}

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			tooBadWords = new List<string>();
			goodAnswerWords = new List<string>();

			FillListSyn("TooBad", tooBadWords);
			FillListSyn("GoodAnswerWas", goodAnswerWords);


			BYOS.Instance.SoundManager.Play(SoundType.RANDOM_CURIOUS);
			mMood.Set(MoodType.SAD);
			mAnimationManager.Sigh();
			mTTS.Silence(1000, true);
			mTTS.Say(RdmStr(tooBadWords), true);
			mTTS.Say(RdmStr(goodAnswerWords) + CommonIntegers["correctResult"], true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mTTS.HasFinishedTalking) {
				animator.SetTrigger("NextLevel");
			}
		}


		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}