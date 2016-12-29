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

			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
			mTTS.Silence(1000, true);
			mMood.Set(MoodType.HAPPY);
			link.mAnimationManager.Smile();
			mTTS.Say(link.currentLevel.successSentence, true);
			mOnEnterDone = true;
        }

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (mOnEnterDone) {
				if (mTTS.HasFinishedTalking) {
					int level = animator.GetInteger("level");
					level += 1;
					bool updated = link.UpdateLevel(level);
					if (updated) {
						animator.SetInteger("level", level);
						link.mAnimationManager.gameObject.SetActive(false);
						animator.SetTrigger("NextLevel");
					} else {
						mTTS.SayKey("win");
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
			mMood.Set(MoodType.NEUTRAL);
		}

	}
}
