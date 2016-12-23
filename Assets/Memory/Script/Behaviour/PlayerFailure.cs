using UnityEngine;
using System.Collections;
using BuddyOS;


namespace BuddyApp.Memory
{
	public class PlayerFailure : LinkStateMachineBehavior
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Debug.Log("Player Failure !");


			BYOS.Instance.SoundManager.Play(SoundType.RANDOM_CURIOUS);
			link.tts.Silence(1000, true);
			link.mMood.Set(MoodType.SAD);
			link.animationManager.Sigh();
			link.tts.Say(link.currentLevel.failureSentence, true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (link.tts.HasFinishedTalking) {
				//Application.Quit ();
				//			HomeCmd.Create().Execute();
				Debug.Log("unload SimonGame");
				//link.UnLoadScene();
				//			SceneManager.LoadScene("SimonGame", LoadSceneMode.Additive);
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			link.mMood.Set(MoodType.NEUTRAL);
		}

	}
}