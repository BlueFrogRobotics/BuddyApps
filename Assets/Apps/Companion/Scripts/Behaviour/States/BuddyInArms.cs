using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class BuddyInArms : AStateMachineBehaviour
	{
		private float mTimeInArmns;
		private bool mVocalTriggered;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
        }

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			// TODO: lock wheels? + react according to mood
			mActionManager.CurrentAction = BUDDY_ACTION.TOUCH_INTERACT;
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "Buddy in Arms";
			Debug.Log("state: Buddy in Arms");
			mTimeInArmns = 0F;
			if (CompanionData.Instance.mInteractDesire > 40) {
				Interaction.TextToSpeech.SayKey("ilikearm", true);
				CompanionData.Instance.mInteractDesire -= 10;
				Interaction.Mood.Set(MoodType.HAPPY);
			} else {
				Interaction.TextToSpeech.SayKey("letmeroll", true);
				CompanionData.Instance.mInteractDesire -= 20;
				Interaction.Mood.Set(MoodType.GRUMPY);
			}
		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeInArmns += Time.deltaTime;

			if (mDetectionManager.mDetectedElement == Detected.TRIGGER || mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH) {
				//Interaction.TextToSpeech.Say("Que puis-je pour toi?", true);
				iAnimator.SetTrigger("VOCALTRIGGERED");
			} else if (mTimeInArmns > 5F)
				// TODO: do something? to define
				Interaction.Mood.Set(MoodType.SICK);
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}
	}
}