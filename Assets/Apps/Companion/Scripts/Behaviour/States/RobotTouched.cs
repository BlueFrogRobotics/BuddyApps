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
		private int mFaceCounter;
		private int mEyeCounter;
		private float mLastTouchTime;
		private FaceTouch mLastPartTouched;

		public override void Start()
		{
			//mSensorManager = BYOS.Instance.SensorManager;

			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			//mLastMouthTime = 0F;
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mActionManager.CurrentAction = BUDDY_ACTION.TOUCH_INTERACT;
			mState.text = "Robot Touched " + mDetectionManager.mFacePartTouched;
			Debug.Log("state: Robot Touched: " + mDetectionManager.mFacePartTouched);
			mLastTouchTime = 0F;
			mFaceCounter = 0;
			mEyeCounter = 0;
			mLastPartTouched = mDetectionManager.mFacePartTouched;



		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			// When we arrive here, it means face was touch but not mouth.
			// TODO: if Buddy is not positive +3, ask for more after some time.
			// if no more after some time, go away (desiredAction?).
			// Otherwise, just keep reacting, say some stuff...

			mState.text = "Robot Touched " + mDetectionManager.mFacePartTouched;

			if (mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH || mDetectionManager.mDetectedElement == Detected.TRIGGER)
				Trigger("VOCALCOMMAND");


			if (mDetectionManager.mFacePartTouched == FaceTouch.LEFT_EYE || mDetectionManager.mFacePartTouched == FaceTouch.RIGHT_EYE) {
				mLastTouchTime = Time.time;
				//React
				mActionManager.EyeReaction();
				mEyeCounter++;

				mLastPartTouched = mDetectionManager.mFacePartTouched;
				// TODO: ask to stop poking
			} else if (mDetectionManager.mFacePartTouched == FaceTouch.OTHER) {
				mLastTouchTime = Time.time;
				//React
				mActionManager.HeadReaction();
				mFaceCounter++;

				mLastPartTouched = mDetectionManager.mFacePartTouched;
				// TODO: then ask for more after some time if positivity < 3
			}

			// TO DO:
			// after no touch for a while, go to user detected?

			if (!mActionManager.ActiveAction() && Interaction.SpeechToText.HasFinished) {

				if (mLastPartTouched == FaceTouch.OTHER) {
					if (mLastTouchTime > 5F) {
						// TODO ask touch again if not enough positivity
						// else go back to User detected
						Trigger("INTERACT");

					} else {
						// Say something from time to time according to mood
					}

				} else if (mLastPartTouched == FaceTouch.RIGHT_EYE || mLastPartTouched == FaceTouch.LEFT_EYE) {
					
					} if (mLastTouchTime > 4F){
						// thanks to stop poking according to mood
					}
					else if (mEyeCounter == 2) {
					// ask to stop according to mood
				}

			}

			if (mLastTouchTime > 10F) {
				// if nothing for 10s
				if (mFaceCounter > 2 && mEyeCounter < 2)
					Trigger("FOLLOW");
				else
					Trigger("INTERACT");
			}

			mDetectionManager.mFacePartTouched = FaceTouch.NONE;
			mDetectionManager.mDetectedElement = Detected.NONE;



		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

	}
}