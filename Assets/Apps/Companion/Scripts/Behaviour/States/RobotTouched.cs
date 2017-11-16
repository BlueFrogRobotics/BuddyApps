using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class RobotTouched : AStateMachineBehaviour
	{
		private int mMouthCounter;
		private int mEyeCounter;
		private float mLastMouthTime;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mMouthCounter = 0;
			mEyeCounter = 0;
			mLastMouthTime = 0F;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Robot Touched " + mDetectionManager.mFacePartTouched;
			Debug.Log("state: Robot Touched: " + mDetectionManager.mFacePartTouched);
			if (mDetectionManager.mFacePartTouched == FaceTouch.OTHER || mDetectionManager.mFacePartTouched == FaceTouch.NONE) {

				if (CompanionData.Instance.InteractDesire > 80) {
					//Interaction.TextToSpeech.Say("Hey! Si on faisait un jeu!", true);
					iAnimator.SetTrigger("PROPOSEGAME");
				} else {
					//Interaction.TextToSpeech.Say("Que puis-je pour vous?", true);
					iAnimator.SetTrigger("VOCALTRIGGERED");
				}


			} else {
				// User touched the eye / face
				if (mDetectionManager.mFacePartTouched == FaceTouch.MOUTH) {
					if (mMouthCounter > 2)
						//TODO: play BML instead
						Interaction.Mood.Set(MoodType.SICK);
					else {
						if (Time.time - mLastMouthTime < 10F)
							mMouthCounter++;
						else
							mMouthCounter = 0;
						mLastMouthTime = Time.time;
					}
				}else if(mDetectionManager.mFacePartTouched == FaceTouch.LEFT_EYE || mDetectionManager.mFacePartTouched == FaceTouch.RIGHT_EYE) {
					if (mMouthCounter > 2)
						//TODO: play BML instead
						Interaction.Mood.Set(MoodType.GRUMPY);
					else {
						if (Time.time - mLastMouthTime < 10F)
							mMouthCounter++;
						else
							mMouthCounter = 0;
						mLastMouthTime = Time.time;
					}
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mFacePartTouched = FaceTouch.NONE;
			mDetectionManager.mDetectedElement = Detected.NONE;
        }

	}
}