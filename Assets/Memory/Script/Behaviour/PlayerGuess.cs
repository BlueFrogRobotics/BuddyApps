using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;

namespace BuddyApp.Memory
{
	public class PlayerGuess : LinkStateMachineBehavior
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

		public override void Init()
		{
			mOnEnterDone = false;
		}

		public void InitGuess()
		{
			mCurrentEvents = new List<int>();
			mEvents = link.currentLevel.events;

			mFail = false;
			mSuccess = false;

			mWaitTimer = 0.0f;
			mEndMaxTime = 20.0f;
			mResetTimer = false;

			mRandomMoveTimer = 0.0f;
			//randomMoveStarted = false;
			SetRandomTimerLimit();

			link.SetClickFace(ClickFace);
		}

		public static void ClickFace(int value)
		{

			switch (value) {
				case 0:
					Debug.Log("Click left eye from PlayerGuess script");
					mCurrentEvents.Add((int)FaceEvent.BLINK_LEFT);
					break;
				case 1:
					Debug.Log("Click right eye from Player Guess Script");
					mCurrentEvents.Add((int)FaceEvent.BLINK_RIGHT);
					break;
				case 2:
					Debug.Log("Click mouth from Player Guess Script");
					mCurrentEvents.Add((int)FaceEvent.SMILE);
					break;
				default:
					Debug.Log("Click UNKOWN from Player Guess Script");
					mFail = true;
					break;
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

		public static bool IsSuccess()
		{
			if (mSuccess) {
				return true;
			} else if (mCurrentEvents.Count > mEvents.Count) {
				return false;
			} else {
				for (int i = 0; i < mCurrentEvents.Count; i++) {
					if (!mCurrentEvents[i].Equals(mEvents[i])) {
						Debug.Log("Comparing " + mCurrentEvents[i] + " and " + mEvents[i]);
						return false;
					}
				}
			}
			return true;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mBuddyMotion = false;
			Debug.Log("Player's turn");
			mOriginHeadPos = 0.0f;
			mNoHinge.SetPosition(mOriginHeadPos);
			mOriginRobotAngle = mWheels.Odometry.z;
			InitGuess();

			mTTS.Say(link.gameLevels.GetRandomYourTurn(), true);
			mWaitTimer = 0.0f;
			mOnEnterDone = true;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (link.mUnloadingScene) {
				Debug.Log("Unloading");
				QuitApp();
			}

			if (mOnEnterDone && !mBuddyMotion) {


				if (Mathf.Abs(mNoHinge.CurrentAnglePosition) > ANGLE_THRESH) {

					if (mNoHinge.CurrentAnglePosition > mOriginHeadPos) {
						Debug.Log("Pushing head left");
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
						mCurrentEvents.Add(8);
						StartCoroutine(ControlBuddy(BuddyMotion.HEAD_LEFT));
					} else {
						Debug.Log("Pushing head right");
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
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
				} else if (Mathf.Abs(mWheels.Odometry.z - mOriginRobotAngle) > ANGLE_THRESH) {
					if (mWheels.Odometry.z > mOriginRobotAngle) {
						Debug.Log("Pushing robot left");
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
						mCurrentEvents.Add(10);
						StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_LEFT));

					} else {
						Debug.Log("Pushing robot right");
						BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_SURPRISED);
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



				if (mTTS.HasFinishedTalking) {
					mWaitTimer += Time.deltaTime;
					mRandomMoveTimer += Time.deltaTime;
					link.isPlayerTurn = true;
				}

				if (mFace.IsStable && mFail) {
					Debug.Log("Oups you failed");
					animator.SetTrigger("PlayerFailure");
				}

				if (mFace.IsStable && mSuccess) {

					if (mResetTimer) {
						mWaitTimer = 0.0f;
						mResetTimer = false;
					}

					if (mWaitTimer > 1.5f) {
						Debug.Log("Congrats, you win !");
						animator.SetTrigger("PlayerSuccess");
					}
				}

				if (mWaitTimer > mEndMaxTime) {
					Debug.Log("TimeOut game");
					animator.SetTrigger("PlayerFailure");
				}


			}
		}


		private IEnumerator ControlBuddy(BuddyMotion iMotion)
		{
			Debug.Log("Moving : " + iMotion);
			mBuddyMotion = true;

			float lTimer = 0.0f;

			// Moving noHinge
			if (iMotion == BuddyMotion.HEAD_LEFT || iMotion == BuddyMotion.HEAD_RIGHT) {
				float lOriginAngle = mNoHinge.CurrentAnglePosition;
				float lTargetAngle;
				if (iMotion == BuddyMotion.HEAD_LEFT) {
					lTargetAngle = lOriginAngle + 45.0f;
				} else {
					lTargetAngle = lOriginAngle - 45.0f;
				}

				// Put the head to the given direction
				mNoHinge.SetPosition(lTargetAngle);
				// Wait for end of motion
				while (Math.Abs(mNoHinge.CurrentAnglePosition - lOriginAngle) < 20.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;
				}

				lTimer = 0.0f;
				Debug.Log("Moving head ok, move back ");
				// Put the head back
				mNoHinge.SetPosition(0.0f);

				// Wait for end of motion
				while (Math.Abs(mNoHinge.CurrentAnglePosition) > 5.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;

				}

			} else if (iMotion == BuddyMotion.HEAD_DOWN) {
				Debug.Log("Motion not available" + iMotion);
			} else if (iMotion == BuddyMotion.HEAD_UP) {
				Debug.Log("Motion not available" + iMotion);



				// Turning wheel
			} else if (iMotion == BuddyMotion.WHEEL_LEFT || iMotion == BuddyMotion.WHEEL_RIGHT) {
				float lTargetAngle = - (90.0f - Math.Abs(mOriginRobotAngle - mWheels.Odometry.z));
				if (iMotion == BuddyMotion.WHEEL_LEFT) {
					lTargetAngle = - lTargetAngle;
				}

				mWheels.TurnAngle(lTargetAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while (mWheels.Status != MovingState.REACHED_GOAL && mWheels.Status != MovingState.MOTIONLESS) {
					yield return null;
				}

				Debug.Log("Moving wheels ok, move back ");

				// Put the robot back
				mWheels.TurnAbsoluteAngle(mOriginRobotAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while (mWheels.Status != MovingState.REACHED_GOAL && mWheels.Status != MovingState.MOTIONLESS) {
					yield return null;
				}

			}

			yield return new WaitForSeconds(1.0f);
			Debug.Log("Motion done");

			mBuddyMotion = false;

		}


		void RandomAnim()
		{
			switch (UnityEngine.Random.Range(0, 3)) {
				case 0:
					link.mAnimationManager.Smile();
					break;
				case 1:
					link.mAnimationManager.Sigh();
					break;
				case 2:
					link.mAnimationManager.Blink();
					break;
				case 3:
					link.mAnimationManager.Smile();
					break;
				default:
					link.mAnimationManager.Blink();
					break;
			}
		}

		void SetRandomTimerLimit()
		{
			mRandomMoveTimerLimit = UnityEngine.Random.Range(0.5f, 3.0f);
			Debug.Log("randomMoveTimerLimit = " + mRandomMoveTimerLimit);
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			link.isPlayerTurn = false;
			//		link.animationManager.ResetPosition ();
		}

	}
}