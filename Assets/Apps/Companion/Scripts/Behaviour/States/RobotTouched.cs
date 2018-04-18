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
		private float mLastSpeechTime;

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
			mLastSpeechTime = 0F;
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

			mState.text = "Robot  Touched " + mDetectionManager.mFacePartTouched;

			if (mDetectionManager.mDetectedElement == Detected.MOUTH_TOUCH || mDetectionManager.mDetectedElement == Detected.TRIGGER)
				Trigger("VOCALCOMMAND");


			else {
				if (mDetectionManager.mFacePartTouched == FaceTouch.LEFT_EYE || mDetectionManager.mFacePartTouched == FaceTouch.RIGHT_EYE) {
					Debug.Log("Robot touched eye");
					mLastTouchTime = Time.time;
					//React
					mActionManager.EyeReaction();
					mEyeCounter++;

					mLastPartTouched = mDetectionManager.mFacePartTouched;

					if (mEyeCounter % 5 == 2 && Interaction.TextToSpeech.HasFinishedTalking && Time.time - mLastSpeechTime > 5F) {
						mLastSpeechTime = Time.time;
						Interaction.TextToSpeech.SayKey("stoppokeeye");
					}



				} else if (mDetectionManager.mFacePartTouched == FaceTouch.OTHER) {
					Debug.Log("Robot touched other");

					Debug.Log("Robot touched other " + mDetectionManager.mFacePartTouched.ToString() + " " + mDetectionManager.mDetectedElement.ToString());
					mLastTouchTime = Time.time;
					//React
					mActionManager.HeadReaction();
					mFaceCounter++;

					mLastPartTouched = mDetectionManager.mFacePartTouched;
					if (mFaceCounter % 5 == 2 && Interaction.TextToSpeech.HasFinishedTalking && Time.time - mLastSpeechTime > 5F) {
						mLastSpeechTime = Time.time;
						Interaction.TextToSpeech.SayKey("ilikecaress");
					}
				}

				// TO DO:
				// after no touch for a while, go to user detected?

				if (Interaction.BMLManager.DonePlaying && Interaction.SpeechToText.HasFinished && Interaction.TextToSpeech.HasFinishedTalking) {

					if (mLastPartTouched == FaceTouch.OTHER) {
						if (Time.time - mLastTouchTime > 4F) {
							Debug.Log("Robot touched ask again");
							// ask touch again if not enough positivity
							if (Interaction.InternalState.Positivity < 3 && Time.time - mLastSpeechTime > 10F) {
								mLastSpeechTime = Time.time;
								Interaction.TextToSpeech.SayKey("morecaress");

								// else go back to User detected
							} else
								Trigger("INTERACT");

						} else {
							// Say something from time to time according to mood
						}

					} else if (mLastPartTouched == FaceTouch.RIGHT_EYE || mLastPartTouched == FaceTouch.LEFT_EYE) {


						if (Time.time - mLastTouchTime > 4F) {
							// thanks to stop poking according to mood
						}

					}

					if (Time.time - mLastTouchTime > 10F) {
						// if nothing for 7s
						if (mFaceCounter > 2 && mEyeCounter < 2)
							Trigger("FOLLOW");
						else
							Trigger("INTERACT");
					}

				}

				mDetectionManager.mFacePartTouched = FaceTouch.NONE;
				mDetectionManager.mDetectedElement = Detected.NONE;
				//Debug.Log("Reset values: " + mDetectionManager.mFacePartTouched.ToString() + " " + mDetectionManager.mDetectedElement.ToString());
			}

		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
		}

	}
}