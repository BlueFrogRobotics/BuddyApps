using UnityEngine;
using Buddy;
using System;

namespace BuddyApp.MemoryGame
{
	public class GameIntro : AStateMachineBehaviour
	{
		private float mTTSTimer;

		public override void Start()
		{
			BYOS.Instance.VocalManager.enabled = false;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTTSTimer = 0.0f;

			BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
			mTTS.Silence(1000, true);
			mTTS.Say(mDictionary.GetRandomString("intro"), true);
		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//if (link.mUnloadingScene) {
			//    Debug.Log("Unloading");
			//    QuitApp();
			//}

			mTTSTimer += Time.deltaTime;

			if (mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
				animator.SetTrigger("IntroDone");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}
