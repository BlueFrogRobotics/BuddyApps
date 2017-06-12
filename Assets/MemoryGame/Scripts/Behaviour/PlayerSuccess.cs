using UnityEngine;
using System.Collections;
using Buddy;

namespace BuddyApp.MemoryGame
{
	public class PlayerSuccess : AStateMachineBehaviour
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
			Debug.Log("Player Success !");

			//BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			mTTS.Silence(1000, true);
			mMood.Set(MoodType.HAPPY);
			//link.mAnimationManager.Smile();
			mTTS.Say(mDictionary.GetRandomString("success"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (mTTS.HasFinishedTalking) {
				Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
				bool updated = UpdateLevel();
				if (updated) {
					//link.mAnimationManager.gameObject.SetActive(false);

					Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
					animator.SetTrigger("NextLevel");
				} else {
					mTTS.Say(mDictionary.GetRandomString("win"));
					//link.mAnimationManager.gameObject.SetActive(true);
					Debug.Log("End of the game");
					QuitApp();
				}
			}

		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
		}

		public bool UpdateLevel()
		{
			mGameLevels.mCurrentLevel++;
			if (mGameLevels.mCurrentLevel > (mGameLevels.NbLevels)) {
				// no next level to load
				return false;
			}
			return true;
		}

	}
}
