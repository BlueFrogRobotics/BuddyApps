using UnityEngine;
using System.Collections;
using System;
using Buddy;

namespace BuddyApp.TakePose
{
    public class ExitPose : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Interaction.Mood.Set(MoodType.NEUTRAL);
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking) {
				QuitApp();
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}