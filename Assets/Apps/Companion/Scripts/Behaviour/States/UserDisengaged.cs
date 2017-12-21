using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
    public class UserDisengaged : AStateMachineBehaviour
    {
        private float mTimeState;

        public override void Start()
        {
            mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "User disengaged";
            Debug.Log("state: User disengaged");

            mTimeState = 0F;
			Interaction.TextToSpeech.SayKey("userquit");

            Interaction.Mood.Set(MoodType.NEUTRAL);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimeState += Time.deltaTime;
			
            if (CompanionData.Instance.InteractDesire < 0) {
                CompanionData.Instance.InteractDesire = 0;
            } else {
                CompanionData.Instance.InteractDesire -= 10;
            }

            if (CompanionData.Instance.MovingDesire > 50) {
                Debug.Log("User disengaged -> wander: " + CompanionData.Instance.MovingDesire);
                iAnimator.SetTrigger("WANDER");
            } else if (mTimeState > 10F) {
                iAnimator.SetTrigger("IDLE");
            }

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			mDetectionManager.mDetectedElement = Detected.NONE;
		}
    }
}