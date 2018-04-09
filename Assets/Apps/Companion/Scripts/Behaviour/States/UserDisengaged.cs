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
			mDesireManager = GetComponent<DesireManager>();
		}

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			mState.text = "User disengaged";
            Debug.Log("state: User disengaged");

            mTimeState = 0F;
			Interaction.TextToSpeech.SayKey("userquit");

            Interaction.Mood.Set(MoodType.NEUTRAL);
			mDesireManager.MultiplyDesires(0.5F);
			CompanionData.Instance.mMovingDesire += 20;
		}

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimeState += Time.deltaTime;
			

            if (CompanionData.Instance.mMovingDesire > 50 & CompanionData.Instance.CanMoveHead && CompanionData.Instance.CanMoveBody) {
                Debug.Log("User disengaged -> wander: " + CompanionData.Instance.mMovingDesire);
                iAnimator.SetTrigger("WANDER");
            } else if (mTimeState > 3F) {
                iAnimator.SetTrigger("IDLE");
            }

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
			mDetectionManager.mDetectedElement = Detected.NONE;
		}
    }
}