using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class Follow : AStateMachineBehaviour
	{
		private float mTimeThermal;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mTimeThermal = 0;
			mState.text = "Follow";
			Debug.Log("state: follow");

			Debug.Log("wander: " + CompanionData.Instance.MovingDesire);
		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			if (Interaction.TextToSpeech.HasFinishedTalking && !mActionManager.ThermalFollow) {
				Debug.Log("CompanionWander start follow");
				mActionManager.StartThermalFollow();
			}

			if (Time.time - mTimeThermal > 2.0F) {
				//Buddy is alone now -> scared
				Interaction.Mood.Set(MoodType.SCARED);
			}else if (Time.time - mTimeThermal > 8.0F) {
				//Buddy is alone sad
				Interaction.Mood.Set(MoodType.SAD);
			} else if (Time.time - mTimeThermal > 15.0F) {
				//Buddy is alone sad
				mActionManager.StopThermalFollow();
				Trigger("IDLE");
				// TODO: Maybe Trigger("LOOKINGFOR");
			}

			// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
			switch (mDetectionManager.mDetectedElement) {
					case Detected.TRIGGER:
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.TOUCH:
						Trigger("ROBOTTOUCHED");
						break;

					case Detected.KIDNAPPING:
						Trigger("KIDNAPPING");
						break;

					case Detected.BATTERY:
						Interaction.Mood.Set(MoodType.TIRED);
						break;

					// If thermal signature, nothing
					case Detected.THERMAL:
						mTimeThermal = Time.time;
						break;

					//case Detected.HUMAN_RGB & Detected.THERMAL:
					//	// TODO: check false positive level
					//	mTrigged = true;
					//	if (CompanionData.Instance.InteractDesire > CompanionData.Instance.MovingDesire) {
					//		mTrigged = true;
					//		Trigger("INTERACT");
					//	}
					//	break;

					default:
						break;
				}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
		}

	}
}