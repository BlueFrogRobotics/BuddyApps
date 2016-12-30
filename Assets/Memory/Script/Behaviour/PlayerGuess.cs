using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


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
		private bool mHeadMotion;
		private float mOriginHeadPos;

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
			mHeadMotion = false;
			Debug.Log("Player's turn");
			mNoHinge.SetPosition(0.0f);
			mOriginHeadPos = 0.0f;
			InitGuess();

			mTTS.Say(link.gameLevels.GetRandomYourTurn(), true);
			mWaitTimer = 0.0f;
			mOnEnterDone = true;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (mOnEnterDone && !mHeadMotion) {

				
				if (Mathf.Abs(mNoHinge.CurrentAnglePosition) > ANGLE_THRESH) {

					if (mNoHinge.CurrentAnglePosition > mOriginHeadPos) {
						Debug.Log("Pushing head left");
						mCurrentEvents.Add(8);
						StartCoroutine(MoveHeadLeft(true));
					} else {
						Debug.Log("Pushing head right");
						mCurrentEvents.Add(9);
						StartCoroutine(MoveHeadLeft(false));
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

				//if (randomMoveTimer > randomMoveTimerLimit) {
				//	//			link.animationManager.StartAnimation ();
				//	RandomAnim();
				//	randomMoveTimer = 0.0f;
				//	SetRandomTimerLimit();
				//	//			randomMoveStarted = true;
				//}
			}
		}


		private IEnumerator MoveHeadLeft(bool iLeft)
		{
			Debug.Log("Moving head left: " + iLeft);
			mHeadMotion = true;
			float lOriginAngle = mNoHinge.CurrentAnglePosition;
			float lTargetAngle;

			if (iLeft) {
				lTargetAngle = lOriginAngle + 45.0f;
			} else {
				lTargetAngle = lOriginAngle - 45.0f;
			}
			// Put the head to the given direction
			mNoHinge.SetPosition(lTargetAngle);

			// Wait for end of motion
			while (Math.Abs(mNoHinge.CurrentAnglePosition - lOriginAngle) < 10.0f) {
				yield return null;
			}


			Debug.Log("Moving head ok, move back ");
			// Put the head back
			mNoHinge.SetPosition(0.0f);

			// Wait for end of motion
			while (Math.Abs(mNoHinge.CurrentAnglePosition) > 5.0f) {
				yield return null;

			}
			Debug.Log("Moving head back ok");

			yield return new WaitForSeconds(1);
			mHeadMotion = false;

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