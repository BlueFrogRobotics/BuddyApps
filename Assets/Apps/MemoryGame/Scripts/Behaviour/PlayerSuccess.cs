using UnityEngine;
using System.Collections;
using Buddy;
using Buddy.UI;

namespace BuddyApp.MemoryGame
{
	public class PlayerSuccess : AStateMachineBehaviour
	{
		private MemoryGameRandomLevel mGameLevels;
		private bool mQuitApp;

		public override void Start()
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mQuitApp = false;
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
			//link.mAnimationManager.gameObject.SetActive(true);
			//Debug.Log("Player Success !");

			Interaction.Mood.Set(MoodType.HAPPY);
			Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			Interaction.TextToSpeech.Silence(1000, true);
			//link.mAnimationManager.Smile();
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("success"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (Interaction.TextToSpeech.HasFinishedTalking) {
				//Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
				if (mQuitApp) {
					Toaster.Hide();
					Trigger("Replay");
				} else {
					bool updated = UpdateLevel();
					if (updated) {
						//link.mAnimationManager.gameObject.SetActive(false);

						//Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
						animator.SetTrigger("NextLevel");
					} else {
						mQuitApp = true;
						Toaster.Display<VictoryToast>().With(Dictionary.GetString("win"));
						Interaction.TextToSpeech.Say(Dictionary.GetRandomString("win"));
						//link.mAnimationManager.gameObject.SetActive(true);
						//Debug.Log("End of the game");
					}
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Debug.Log("Success Current lvl: " + mGameLevels.mCurrentLevel);
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
