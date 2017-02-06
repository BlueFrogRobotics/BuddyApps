using UnityEngine;
using System.Collections;
using BuddyOS;
using BuddyOS.App;

namespace BuddyApp.Memory
{
	public class PlayerSuccess : LinkStateMachineBehavior
	{


		public override void Init()
		{
			mOnEnterDone = false;
		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			link.mAnimationManager.gameObject.SetActive(true);
			Debug.Log("Player Success !");

			//BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			mTTS.Silence(1000, true);
			mMood.Set(MoodType.HAPPY);
			link.mAnimationManager.Smile();
			mTTS.Say(mDictionary.GetRandomString("success"), true);
			mOnEnterDone = true;
        }

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (link.mUnloadingScene) {
				Debug.Log("Unloading");
				QuitApp();
			}

			if (mOnEnterDone) {
				if (mTTS.HasFinishedTalking) {
					Debug.Log("Success Current lvl: " + link.gameLevels.mCurrentLevel);
					bool updated = link.UpdateLevel();
					if (updated) {
						//link.mAnimationManager.gameObject.SetActive(false);

						Debug.Log("Success Current lvl: " + link.gameLevels.mCurrentLevel);
						animator.SetTrigger("NextLevel");
					} else {
						mTTS.Say(mDictionary.GetRandomString("win"));
						link.mAnimationManager.gameObject.SetActive(true);
						Debug.Log("End of the game");
						QuitApp();
					}
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("Success Current lvl: " + link.gameLevels.mCurrentLevel);
		}

	}
}
