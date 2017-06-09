using UnityEngine;
using System.Collections;
using Buddy;
using System;

namespace BuddyApp.MemoryGame
{
	public class PlayerFailure : AStateMachineBehaviour
	{
		private MemoryGameRandomLevel mGameLevels;

		public override void Start()
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{


			//link.mAnimationManager.gameObject.SetActive(true);
			Debug.Log("Player Failure !");


			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			mTTS.Silence(1000, true);
			mMood.Set(MoodType.SAD);
			//link.mAnimationManager.Sigh();
			mTTS.Say(mDictionary.GetRandomString("failure"), true);
			mTTS.Silence(1000);
			mTTS.Say(mDictionary.GetRandomString("restart"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//if (link.mUnloadingScene) {
			//	Debug.Log("Unloading");
			//	QuitApp();
			//}

			// Restart from start
			if (mTTS.HasFinishedTalking) {
				//link.ResetLevel();
				Debug.Log("failure Current lvl: " + mGameLevels.mCurrentLevel);
				animator.SetTrigger("NextLevel");


				//Application.Quit ();
				//QuitApp();
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("failure Current lvl: " + mGameLevels.mCurrentLevel);
		}

	}
}
