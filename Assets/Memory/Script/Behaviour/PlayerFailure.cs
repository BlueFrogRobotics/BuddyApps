using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.App;
using System;

namespace BuddyApp.Memory
{
	public class PlayerFailure : LinkStateMachineBehavior
	{


		public override void Init()
		{
			Debug.Log("Init playerFailure");
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{


			link.mAnimationManager.gameObject.SetActive(true);
			Debug.Log("Player Failure !");


			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			mTTS.Silence(1000, true);
			mMood.Set(MoodType.SAD);
			link.mAnimationManager.Sigh();
			mTTS.Say(mDictionary.GetRandomString("failure"), true);
			mTTS.Silence(1000);
			mTTS.Say(mDictionary.GetRandomString("restart"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (link.mUnloadingScene) {
				Debug.Log("Unloading");
				QuitApp();
			}

			// Restart from start
			if (mTTS.HasFinishedTalking) {
				//link.ResetLevel();
				Debug.Log("failure Current lvl: " + link.gameLevels.mCurrentLevel);
				animator.SetTrigger("NextLevel");


				//Application.Quit ();
				//QuitApp();
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("failure Current lvl: " + link.gameLevels.mCurrentLevel);
		}

	}
}
