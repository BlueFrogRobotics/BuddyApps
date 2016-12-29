using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


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

		private bool mHeadMotion;

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
							StartCoroutine(MoveHeadLeft(true));
						} else if (mEvents[mEventIndex] == 9) {
							Debug.Log("Move head right");
							StartCoroutine(MoveHeadLeft(false));
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

			yield return new WaitForSeconds(1.5f);
			
			Debug.Log("Moving head back ok");

			mHeadMotion = false;

		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			link.mAnimationManager.gameObject.SetActive(false);
			mHeadMotion = false;
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

			// TODO: add wheel is stable ( = is at starting position)

			if (mOnEnterDone) {
				mTTSTimer += Time.deltaTime;

				if (mFace.IsStable && !mHeadMotion && mTTS.HasFinishedTalking) {
					mTimer += Time.deltaTime;
				}

				if (!mPatternDone && mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
					DoEvent();
				} else if (mPatternDone && mFace.IsStable && !mHeadMotion) {
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