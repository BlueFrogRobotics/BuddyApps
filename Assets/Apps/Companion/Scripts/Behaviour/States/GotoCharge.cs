using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{

	public class GotoCharge : AStateMachineBehaviour
	{
		private bool mSpeechTriggered;
		private bool mVeryLowBattery;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			Debug.Log("state: Goto charge");
			mState.text = "Goto charge: " +	BYOS.Instance.Primitive.Battery.EnergyLevel;
			Interaction.Mood.Set(MoodType.TIRED);
            //Interaction.TextToSpeech.Say("igotocharge", true);


        }

		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (mDetectionManager.mDetectedElement == Detected.TRIGGER) {
				//Interaction.TextToSpeech.Say("Que puis-je pour toi?", true);
				iAnimator.SetTrigger("VOCALTRIGGERED");
				if (Primitive.Battery.EnergyLevel < 10) {
					Interaction.TextToSpeech.Say("batterytoolow", true);
					mSpeechTriggered = false;
                } else {
					iAnimator.SetTrigger("ASKCHARGE");
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
        }
	}
}
