using UnityEngine;
using Buddy;
using System;

namespace BuddyApp.MemoryGame
{
	public class DifficultyChanged : AStateMachineBehaviour
	{
		private float mTTSTimer;
		private bool mSpoke;

		public override void Start()
		{
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.ResetTrigger("IntroDone");
			mTTSTimer = 0.0f;
			mSpoke = false;

			CommonObjects["gameLevels"] = new MemoryGameRandomLevel(MemoryGameData.Instance.Difficulty, MemoryGameData.Instance.FullBody);
			Interaction.TextToSpeech.Silence(0, false);
		}


		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTTSTimer += Time.deltaTime;
			if (mTTSTimer < 1f)
				Interaction.TextToSpeech.Silence(0, false);
			if (Interaction.TextToSpeech.HasFinishedTalking && mTTSTimer > 1f) {
				if (mSpoke) {
					animator.SetTrigger("IntroDone");
				} else {
					string lDifStr = "";

					if (MemoryGameData.Instance.Difficulty == 0) {
						lDifStr = BYOS.Instance.Dictionary.GetString("easy");
					} else if (MemoryGameData.Instance.Difficulty == 1) {
						lDifStr = BYOS.Instance.Dictionary.GetString("medium");

					} else if (MemoryGameData.Instance.Difficulty == 2) {
						lDifStr = BYOS.Instance.Dictionary.GetString("hard");
					}


					Utils.LogI(LogContext.APP, "fullbody =" + MemoryGameData.Instance.FullBody);

					string lFullBodyStr = "";
					if (MemoryGameData.Instance.FullBody)
						lFullBodyStr = Dictionary.GetRandomString("andfullbody");

					Interaction.TextToSpeech.Say(Dictionary.GetRandomString("difficulty") + " " + lDifStr
						+ " " + lFullBodyStr, false);

					//Utils.LogI(LogContext.APP, "saying =" + Dictionary.GetRandomString("difficulty") + " " + lDifStr
					//	+ " " + lFullBodyStr);
					mSpoke = true;
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}
