using Buddy;
using Buddy.Features.Stimuli;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class RobotTouched : AStateMachineBehaviour
	{
		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mSensorManager = GetComponent<StimuliManager>();
			mState = GetComponentInGameObject<Text>(0);
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Robot Touched";
			Debug.Log("state: Robot Touched");
			if (CompanionData.Instance.InteractDesire > 80) {
				//mTTS.Say("Hey! Si on faisait un jeu!", true);
				iAnimator.SetTrigger("PROPOSEGAME");
			} else {
				mTTS.Say("Que puis-je pour toi?", true);
				iAnimator.SetTrigger("VOCALTRIGGERED");
			}
		}

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}

	}
}