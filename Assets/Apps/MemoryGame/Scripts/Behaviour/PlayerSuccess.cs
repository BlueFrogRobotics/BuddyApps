using UnityEngine;
using System.Collections;
using BlueQuark;


namespace BuddyApp.MemoryGame
{
	public class PlayerSuccess : AStateMachineBehaviour
	{
		private MemoryGameRandomLevel mGameLevels;
		private bool mQuitApp;

		public override void Start()
		{
			mGameLevels = GetComponent<MemoryGameRandomLevel>();
		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mQuitApp = false;
			mGameLevels = GetComponent<MemoryGameRandomLevel>();

			Buddy.Behaviour.SetMood(Mood.HAPPY);
			Buddy.Actuators.Speakers.Media.Play(SoundSample.RANDOM_SURPRISED);
			Buddy.Vocal.Say(Buddy.Resources.GetRandomString("success"), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (!Buddy.Vocal.IsBusy)
            {
				if (mQuitApp)
                {
					Buddy.GUI.Toaster.Hide();
					Trigger("Replay");
				}
                else
                {
					if (UpdateLevel())
                    {
						animator.SetTrigger("NextLevel");
					}
                    else
                    {
						mQuitApp = true;
                        Buddy.Behaviour.Mood = Mood.HAPPY;
						Buddy.Vocal.Say(Buddy.Resources.GetRandomString("win"), iOutput =>
                        {
                            Buddy.Behaviour.Mood = Mood.NEUTRAL;
                        });
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
