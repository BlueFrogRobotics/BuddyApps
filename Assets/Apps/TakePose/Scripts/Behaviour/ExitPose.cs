using UnityEngine;
using System.Collections;
using System;

using BlueQuark;

namespace BuddyApp.TakePose
{
    public class ExitPose : AStateMachineBehaviour
    {
        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.Behaviour.Mood.Set(FacialExpression.NEUTRAL);
        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
            if (!Buddy.Vocal.IsSpeaking)
            {
                QuitApp();
            }
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
        }
    }
}