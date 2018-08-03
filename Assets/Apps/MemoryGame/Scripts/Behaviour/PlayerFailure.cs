using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.MemoryGame
{
	public class PlayerFailure : AStateMachineBehaviour
	{
		private MemoryGameRandomLevel mGameLevels;
		private bool mRestartSaid;

		public override void Start()
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mRestartSaid = false;
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);

			Debug.Log("Player Failure !");


			Interaction.Mood.Set(MoodType.SAD);
			Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_CURIOUS);
			Interaction.TextToSpeech.Silence(1000, true);
			//link.mAnimationManager.Sigh();
			Toaster.Display<DefeatToast>().With(Dictionary.GetString("failure"));
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("failure"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			// Restart from start
			if (Interaction.TextToSpeech.HasFinishedTalking) {
				if (mRestartSaid)
					animator.SetTrigger("NextLevel");
				else {
					mRestartSaid = true;
					Toaster.Hide();
					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("restart"), true);
					//Debug.Log("failure Current lvl: " + mGameLevels.mCurrentLevel);
				}

				//QuitApp();
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Debug.Log("failure Current lvl: " + mGameLevels.mCurrentLevel);
		}

	}
}
