using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Memory
{
	public class PlayerSuccess : LinkStateMachineBehavior
	{

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Debug.Log("Player Success !");

			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			link.tts.Silence(1000, true);
			link.mMood.Set(MoodType.HAPPY);
			link.animationManager.Smile();
			link.tts.Say(link.currentLevel.successSentence, true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (link.tts.HasFinishedTalking) {
				int level = animator.GetInteger("level");
				level += 1;
				bool updated = link.UpdateLevel(level);
				if (updated) {
					animator.SetInteger("level", level);
					animator.SetTrigger("NextLevel");
				} else {
					Debug.Log("End of the game");
					link.UnLoadScene();
				}
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			link.mMood.Set(MoodType.NEUTRAL);
		}
	}
}