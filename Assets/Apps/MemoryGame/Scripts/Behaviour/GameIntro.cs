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
			Interaction.VocalManager.enabled = false;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTTSTimer = 0.0f;
			Primitive.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
			//Interaction.Face.SetEvent(FaceEvent.SMILE);
		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTTSTimer += Time.deltaTime;
			if (mTTSTimer > 2.0f && mTTSTimer < 2.5f) {
				Interaction.TextToSpeech.Say(Dictionary.GetRandomString("intro"), true);
				mTTSTimer = 2.6f;
			}


			if (Interaction.TextToSpeech.HasFinishedTalking && mTTSTimer > 4.0f) {
				animator.SetTrigger("IntroDone");
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}
