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

		/*public override void Init()
		{
		}
		*/

		public void InitLvl(int level)
		{
			link.isPlayerTurn = false;
			link.currentLevel = link.gameLevels.levels[level];

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
					link.mFace.SetEvent(mEvents[mEventIndex]);
					//				Face
					mEventIndex++;
				} else {
					mPatternDone = true;
				}
			}
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			InitLvl(animator.GetInteger("level"));

			Debug.Log(link.currentLevel.introSentence);
			link.tts.Silence(1000, true);
			link.tts.Say(link.currentLevel.introSentence, true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			mTTSTimer += Time.deltaTime;

			if (link.mFace.IsStable && link.tts.HasFinishedTalking) {
				mTimer += Time.deltaTime;
			}

			if (!mPatternDone && link.tts.HasFinishedTalking && mTTSTimer > 3.0f) {
				DoFace();
			} else if (mPatternDone && link.mFace.IsStable) {
				Debug.Log("pattern done, your turn");
				animator.SetTrigger("RobotDone");
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

	}
}