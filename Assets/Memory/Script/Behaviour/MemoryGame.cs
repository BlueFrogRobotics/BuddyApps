using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BuddyOS;

namespace BuddyApp.Memory
{
	public class MemoryGame : LinkStateMachineBehavior
	{

		private List<int> mEvents;
		private int mEventIndex;
		private float mTimer;
		private float mMaxTime;
		private bool mPatternDone;
		private float mTTSTimer;

		private bool mBuddyMotion;
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


		public void InitLvl(int level)
		{
			link.isPlayerTurn = false;
			link.currentLevel = link.gameLevels.levels[level];


			Debug.Log("Memory Game Level init link.currentLevel.Count " + link.currentLevel.events.Count);
			mEvents = link.currentLevel.events;

			// right eye = 3
			Debug.Log((int)FaceEvent.BLINK_RIGHT);

			// left eye = 4
			Debug.Log((int)FaceEvent.BLINK_LEFT);

			// smile = 0
			Debug.Log((int)FaceEvent.SMILE);

			mEventIndex = 0;
			mTimer = 0.0f;
			mTTSTimer = 0.0f;
			mMaxTime = 1.0f / link.currentLevel.speed;
			mPatternDone = false;
		}

		public void DoEvent()
		{
			if (mTimer > mMaxTime) {
				mTimer = 0;
				Debug.Log("Do Event");
				Debug.Log("event index : " + mEventIndex);
				if (mEventIndex < mEvents.Count) {
					Debug.Log("do event " + mEvents[mEventIndex]);


					if (mEvents[mEventIndex] > 7) {
						if (mEvents[mEventIndex] == 8) {
							Debug.Log("Move head left");
							StartCoroutine(ControlBuddy(BuddyMotion.HEAD_LEFT));
						} else if (mEvents[mEventIndex] == 9) {
							Debug.Log("Move head right");
							StartCoroutine(ControlBuddy(BuddyMotion.HEAD_RIGHT));
						} else if (mEvents[mEventIndex] == 10) {
							Debug.Log("Move wheels left");
							StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_LEFT));
						} else if (mEvents[mEventIndex] == 11) {
							Debug.Log("Move wheels right");
							StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_RIGHT));
						}
					} else {
						mFace.SetEvent((FaceEvent)mEvents[mEventIndex]);
						//Face
					}
					mEventIndex++;
				} else {
					mPatternDone = true;
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
				Debug.Log("Head target angle " + lTargetAngle);
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



				// Turning wheels
			} else if (iMotion == BuddyMotion.WHEEL_LEFT || iMotion == BuddyMotion.WHEEL_RIGHT) {
				float lTargetAngle = -(90.0f - Math.Abs(mOriginRobotAngle - mWheels.Odometry.z));
				if (iMotion == BuddyMotion.WHEEL_LEFT) {
					lTargetAngle = -lTargetAngle;
				}

				mWheels.TurnAngle(lTargetAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while ( mWheels.Status != MovingState.REACHED_GOAL && mWheels.Status != MovingState.MOTIONLESS ) {
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


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			link.mAnimationManager.gameObject.SetActive(false);

			mNoHinge.SetPosition(0.0f);
			mOriginRobotAngle = mWheels.Odometry.z;
			mBuddyMotion = false;
			InitLvl(animator.GetInteger("level"));
			Debug.Log("pre currentLevel intro Sentence");
			Debug.Log(link.currentLevel.introSentence);
			mTTS.Silence(1000, true);
			mTTS.Say(link.currentLevel.introSentence, true);
			mOnEnterDone = true;

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (link.mUnloadingScene) {
				Debug.Log("Unloading");
				QuitApp();
			}

			if (mOnEnterDone) {
				mTTSTimer += Time.deltaTime;

				if (mFace.IsStable && !mBuddyMotion && mTTS.HasFinishedTalking) {
					mTimer += Time.deltaTime;
				}

				if (!mPatternDone && mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
					DoEvent();
				} else if (mPatternDone && mFace.IsStable && !mBuddyMotion) {
					Debug.Log("pattern done, your turn");
					animator.SetTrigger("RobotDone");
				}
			}
		}


		protected override void OnExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}