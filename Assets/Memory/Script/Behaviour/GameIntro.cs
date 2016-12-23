using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BuddyOS;

namespace BuddyApp.Memory
{
	namespace BuddyApp.Memory
	{
		public class GameIntro : LinkStateMachineBehavior
		{

			float ttsTimer;

			// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
			override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{

				ttsTimer = 0.0f;

				BYOS.Instance.SoundManager.Play(SoundType.RANDOM_LAUGH);
				link.tts.Silence(1000, true);
				link.tts.Say(link.gameLevels.intro, true);
			}

			// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
			override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{

				ttsTimer += Time.deltaTime;

				if (link.tts.HasFinishedTalking && ttsTimer > 3.0f) {
					animator.SetTrigger("IntroDone");
				}
			}


		}
	}
}