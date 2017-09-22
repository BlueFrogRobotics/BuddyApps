using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Buddy;

namespace BuddyApp.MemoryGame
{
	public class PlayerGuess : AStateMachineBehaviour
	{
		static bool mFail;
		static bool mSuccess;

		static List<int> mEvents;
		static List<int> mCurrentEvents;

		public const float ANGLE_THRESH = 10.0F;


		float mWaitTimer;
		float mEndMaxTime;
		static bool mResetTimer;

		float mRandomMoveTimer;
		float mRandomMoveTimerLimit;
		private bool mBuddyMotion;
		private float mOriginHeadPos;
		private float mOriginRobotAngle;
		private MemoryGameRandomLevel mGameLevels;

		public enum BuddyMotion : int
		{
			HEAD_LEFT,
			HEAD_RIGHT,
			HEAD_DOWN,
			HEAD_UP,
			WHEEL_LEFT,
			WHEEL_RIGHT,
			WHEEL_BACK,
			WHEEL_FORWARD
		}

		public override void Start()
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
		}

		public void InitGuess()
		{
			mCurrentEvents = new List<int>();
			mEvents = mGameLevels.events.GetRange(0, mGameLevels.mCurrentLevel * 2);

			mFail = false;
			mSuccess = false;

			mWaitTimer = 0.0f;
			mEndMaxTime = 20.0f;
			mResetTimer = false;

			mRandomMoveTimer = 0.0f;
			//randomMoveStarted = false;
			SetRandomTimerLimit();

		}

		public void LeftEyeClicked()
		{
			Utils.LogI(LogContext.APP, "Click left eye from PlayerGuess script");
			mCurrentEvents.Add((int)FaceEvent.BLINK_LEFT);

			Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_1);

			if (IsSuccess()) {
				if (mCurrentEvents.Count.Equals(mEvents.Count)) {
					mSuccess = true;
					mResetTimer = true;
				}
			} else {
				mFail = true;
			}
		}

		public void RightEyeClicked()
		{
			Utils.LogI(LogContext.APP, "Click right eye from PlayerGuess script");
			mCurrentEvents.Add((int)FaceEvent.BLINK_RIGHT);

			Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_2);

			if (IsSuccess()) {
				if (mCurrentEvents.Count.Equals(mEvents.Count)) {
					mSuccess = true;
					mResetTimer = true;
				}
			} else {
				mFail = true;
			}
		}

		public void MouthClicked()
		{
			Utils.LogI(LogContext.APP, "Click mouth from Player Guess Script");
			mCurrentEvents.Add((int)FaceEvent.SMILE);

			Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_3);

			if (IsSuccess()) {
				if (mCurrentEvents.Count.Equals(mEvents.Count)) {
					mSuccess = true;
					mResetTimer = true;
				}
			} else {
				mFail = true;
			}
		}

		//public static void ClickFace(int value)
		//{

		//	switch (value) {
		//		case 0:
		//			Utils.LogI(LogContext.APP,"Click left eye from PlayerGuess script");
		//			mCurrentEvents.Add((int)FaceEvent.BLINK_LEFT);
		//			break;
		//		case 1:
		//			Utils.LogI(LogContext.APP,"Click right eye from Player Guess Script");
		//			mCurrentEvents.Add((int)FaceEvent.BLINK_RIGHT);
		//			break;
		//		case 2:
		//			Utils.LogI(LogContext.APP,"Click mouth from Player Guess Script");
		//			mCurrentEvents.Add((int)FaceEvent.SMILE);
		//			break;
		//		default:
		//			Utils.LogI(LogContext.APP,"Click UNKOWN from Player Guess Script");
		//			mFail = true;
		//			break;
		//	}
		//	if (IsSuccess()) {
		//		if (mCurrentEvents.Count.Equals(mEvents.Count)) {
		//			mSuccess = true;
		//			mResetTimer = true;
		//		}
		//	} else {
		//		mFail = true;
		//	}
		//}

		public static bool IsSuccess()
		{
			if (mSuccess) {
				Utils.LogI(LogContext.APP, "Success true");
				return true;
			} else if (mCurrentEvents.Count > mEvents.Count) {
				Utils.LogI(LogContext.APP, "Comparing mCurrentEvents.Count: " + mCurrentEvents.Count + " and mEvents.Count: " + mEvents.Count);
				return false;
			} else {
				for (int i = 0; i < mCurrentEvents.Count; i++) {
					if (!mCurrentEvents[i].Equals(mEvents[i])) {
						Utils.LogI(LogContext.APP, "Comparing " + mCurrentEvents[i] + " and " + mEvents[i]);
						return false;
					}
				}
			}
			return true;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
			mBuddyMotion = false;
			Utils.LogI(LogContext.APP, "Player's turn");
			mOriginHeadPos = 0.0f;
			Primitive.Motors.NoHinge.SetPosition(mOriginHeadPos);
			mOriginRobotAngle = Primitive.Motors.Wheels.Odometry.z;
			InitGuess();


			Interaction.Face.OnClickLeftEye.Add(LeftEyeClicked);
			Interaction.Face.OnClickRightEye.Add(RightEyeClicked);
			Interaction.Face.OnClickMouth.Add(MouthClicked);

			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("yourturn"), true);
			mWaitTimer = 0.0f;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (!mBuddyMotion) {


				if (Mathf.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition) > ANGLE_THRESH) {

					if (Primitive.Motors.NoHinge.CurrentAnglePosition > mOriginHeadPos) {
						Utils.LogI(LogContext.APP, "Pushing head left");
						Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_1);
						mCurrentEvents.Add(8);
						StartCoroutine(ControlBuddy(BuddyMotion.HEAD_LEFT));
					} else {
						Utils.LogI(LogContext.APP, "Pushing head right");
						Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_2);
						mCurrentEvents.Add(9);
						StartCoroutine(ControlBuddy(BuddyMotion.HEAD_RIGHT));
					}

					if (IsSuccess()) {
						if (mCurrentEvents.Count.Equals(mEvents.Count)) {
							mSuccess = true;
							mResetTimer = true;
						}
					} else {
						mFail = true;
					}
				} else if (Mathf.Abs(Primitive.Motors.Wheels.Odometry.z - mOriginRobotAngle) > ANGLE_THRESH) {
					if (Primitive.Motors.Wheels.Odometry.z > mOriginRobotAngle) {
						Utils.LogI(LogContext.APP, "Pushing robot left");
						Primitive.Speaker.Voice.Play(VoiceSound.CURIOUS_1);
						mCurrentEvents.Add(10);
						StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_LEFT));

					} else {
						Utils.LogI(LogContext.APP, "Pushing robot right");
						Primitive.Speaker.Voice.Play(VoiceSound.CURIOUS_2);
						mCurrentEvents.Add(11);
						StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_RIGHT));
					}

					if (IsSuccess()) {
						if (mCurrentEvents.Count.Equals(mEvents.Count)) {

							mSuccess = true;
							mResetTimer = true;
						}
					} else {
						mFail = true;
					}
				}



				if (Interaction.TextToSpeech.HasFinishedTalking) {
					mWaitTimer += Time.deltaTime;
					mRandomMoveTimer += Time.deltaTime;
					CommonIntegers["isPlayerTurn"] = 1;
				}

				if (Interaction.Face.IsStable && mFail) {
					Utils.LogI(LogContext.APP, "Oups you failed");
					animator.SetTrigger("PlayerFailure");
				}

				if (Interaction.Face.IsStable && mSuccess) {

					if (mResetTimer) {
						mWaitTimer = 0.0f;
						mResetTimer = false;
					}

					if (mWaitTimer > 1.5f) {
						Utils.LogI(LogContext.APP, "Congrats, you win !");
						animator.SetTrigger("PlayerSuccess");
					}
				}

				if (mWaitTimer > mEndMaxTime) {
					Utils.LogI(LogContext.APP, "TimeOut game");
					animator.SetTrigger("PlayerFailure");
				}


			}
		}


		private IEnumerator ControlBuddy(BuddyMotion iMotion)
		{
			Utils.LogI(LogContext.APP, "Moving : " + iMotion);
			mBuddyMotion = true;

			float lTimer = 0.0f;

			// Moving noHinge
			if (iMotion == BuddyMotion.HEAD_LEFT || iMotion == BuddyMotion.HEAD_RIGHT) {
				float lOriginAngle = Primitive.Motors.NoHinge.CurrentAnglePosition;
				float lTargetAngle;
				if (iMotion == BuddyMotion.HEAD_LEFT) {
					lTargetAngle = lOriginAngle + 45.0f;
				} else {
					lTargetAngle = lOriginAngle - 45.0f;
				}

				// Put the head to the given direction
				Primitive.Motors.NoHinge.SetPosition(lTargetAngle);
				// Wait for end of motion
				while (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition - lOriginAngle) < 20.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;
				}

				lTimer = 0.0f;
				Utils.LogI(LogContext.APP, "Moving head ok, move back ");
				// Put the head back
				Primitive.Motors.NoHinge.SetPosition(0.0f);

				// Wait for end of motion
				while (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition) > 5.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;

				}

			} else if (iMotion == BuddyMotion.HEAD_DOWN) {
				Utils.LogI(LogContext.APP, "Motion not available" + iMotion);
			} else if (iMotion == BuddyMotion.HEAD_UP) {
				Utils.LogI(LogContext.APP, "Motion not available" + iMotion);



				// Turning wheel
			} else if (iMotion == BuddyMotion.WHEEL_LEFT || iMotion == BuddyMotion.WHEEL_RIGHT) {
				float lTargetAngle = -(90.0f - Math.Abs(mOriginRobotAngle - Primitive.Motors.Wheels.Odometry.z));
				if (iMotion == BuddyMotion.WHEEL_LEFT) {
					lTargetAngle = -lTargetAngle;
				}

				Primitive.Motors.Wheels.TurnAngle(lTargetAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while (Primitive.Motors.Wheels.Status != MovingState.REACHED_GOAL && Primitive.Motors.Wheels.Status != MovingState.MOTIONLESS) {
					yield return null;
				}

				//Utils.LogI(LogContext.APP,"Moving wheels ok, move back ");

				// Put the robot back
				Primitive.Motors.Wheels.TurnAbsoluteAngle(mOriginRobotAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while (Primitive.Motors.Wheels.Status != MovingState.REACHED_GOAL && Primitive.Motors.Wheels.Status != MovingState.MOTIONLESS) {
					yield return null;
				}

			}

			yield return new WaitForSeconds(1.0f);
			//Utils.LogI(LogContext.APP,"Motion done");

			mBuddyMotion = false;

		}


		void RandomAnim()
		{
			//switch (UnityEngine.Random.Range(0, 3)) {
			//	case 0:
			//		link.mAnimationManager.Smile();
			//		break;
			//	case 1:
			//		link.mAnimationManager.Sigh();
			//		break;
			//	case 2:
			//		link.mAnimationManager.Blink();
			//		break;
			//	case 3:
			//		link.mAnimationManager.Smile();
			//		break;
			//	default:
			//		link.mAnimationManager.Blink();
			//		break;
			//}
		}

		void SetRandomTimerLimit()
		{
			mRandomMoveTimerLimit = UnityEngine.Random.Range(0.5f, 3.0f);
			Utils.LogI(LogContext.APP, "randomMoveTimerLimit = " + mRandomMoveTimerLimit);
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Interaction.Face.OnClickLeftEye.Remove(LeftEyeClicked);
			Interaction.Face.OnClickRightEye.Remove(RightEyeClicked);
			Interaction.Face.OnClickMouth.Remove(MouthClicked);
			CommonIntegers["isPlayerTurn"] = 0;
			//		link.animationManager.ResetPosition ();
		}

	}
}