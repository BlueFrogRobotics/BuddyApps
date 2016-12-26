using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace BuddyApp.Memory
{
	public class MemoryGame : LinkStateMachineBehavior
	{

		private List<FaceEvent> mEvents;
		private int mEventIndex;
		private float mTimer;
		private float mMaxTime;
		private bool mPatternDone;
		private float mTTSTimer;



		public override void Init()
		{
			mOnEnterDone = false;
		}


		public void InitLvl(int level)
		{
			link.isPlayerTurn = false;
			link.currentLevel = link.gameLevels.levels[level];


			Debug.Log("Memory Game Level init link.currentLevel.Count " + link.currentLevel.faces.Count);
			mEvents = link.currentLevel.faces;

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

		public void DoFace()
		{
			if (mTimer > mMaxTime) {
				mTimer = 0;
				Debug.Log("Do face");
				Debug.Log("event index : " + mEventIndex);
				if (mEventIndex < mEvents.Count) {
					Debug.Log("do event " + mEvents[mEventIndex]);
					mFace.SetEvent(mEvents[mEventIndex]);
					//				Face
					mEventIndex++;
				} else {
					mPatternDone = true;
				}
			}
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

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

			if (mOnEnterDone) {
				mTTSTimer += Time.deltaTime;

				if (mFace.IsStable && mTTS.HasFinishedTalking) {
					mTimer += Time.deltaTime;
				}

				if (!mPatternDone && mTTS.HasFinishedTalking && mTTSTimer > 3.0f) {
					DoFace();
				} else if (mPatternDone && mFace.IsStable) {
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