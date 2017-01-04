using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;
using UnityEngine.UI;

namespace BuddyApp.TakePose
{
	public class Hashtag : SpeechStateBehaviour
	{

		private AnimManager mAnimationManager;

		public override void Init()
		{
			mAnimationManager = GetComponentInGameObject<AnimManager>(1);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mMood.Set(MoodType.HAPPY);
			mAnimationManager.Blink();
			SayInLang("shareTwitter");

			BYOS.Instance.NotManager.Display<SimpleNot>().With("#CES2017 #FrenchTech @adoptbuddy",
							   BYOS.Instance.SpriteManager.GetSprite("Ico_Twitter"), Color.blue);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (mTTS.HasFinishedTalking) {
				animator.SetTrigger("Exit");
			}
		}


		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mMood.Set(MoodType.NEUTRAL);
		}

	}
}