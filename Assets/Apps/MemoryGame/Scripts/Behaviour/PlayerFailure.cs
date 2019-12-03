using UnityEngine;
using BlueQuark;


namespace BuddyApp.MemoryGame
{
	public class PlayerFailure : AStateMachineBehaviour
	{
		private MemoryGameRandomLevel mGameLevels;
		private bool mRestartSaid;

		public override void Start()
		{
			mGameLevels = GetComponent<MemoryGameRandomLevel>();
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mRestartSaid = false;
			mGameLevels = GetComponent<MemoryGameRandomLevel>();

			Debug.Log("Player Failure !");


			Buddy.Behaviour.SetMood(Mood.SAD);
			Buddy.Actuators.Speakers.Media.Play(SoundSample.RANDOM_CURIOUS);
            Buddy.Vocal.Say(Buddy.Resources.GetRandomString("failure"), iOutput =>
            {
                Buddy.Behaviour.Mood = Mood.NEUTRAL;
            });
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			// Restart from start
			if (!Buddy.Vocal.IsBusy) {
				if (mRestartSaid)
					animator.SetTrigger("NextLevel");
				else {
					mRestartSaid = true;
					Buddy.GUI.Toaster.Hide();
					Buddy.Vocal.Say(Buddy.Resources.GetRandomString("restart"), true);
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//Debug.Log("failure Current lvl: " + mGameLevels.mCurrentLevel);
		}

	}
}
