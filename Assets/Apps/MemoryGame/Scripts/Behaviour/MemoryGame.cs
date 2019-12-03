using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BlueQuark;

namespace BuddyApp.MemoryGame
{
	public class MemoryGame : AStateMachineBehaviour
	{
		private List<int> mEvents;
		private int mEventIndex;
		private float mTimer;
		private bool mPatternDone;
		private float mTTSTimer;

		private MemoryGameRandomLevel mGameLevels;
        private EventManager mEventManager;

		public override void Start()
		{
			mGameLevels = GetComponent<MemoryGameRandomLevel>();
            mEventManager = GetComponent<EventManager>();
        }


		public void InitLvl()
		{
			int lMotionsPerLevel = 2;
			if (MemoryGameData.Instance.Difficulty > 1)
				lMotionsPerLevel = 3;
			mEvents = mGameLevels.events.GetRange(0, mGameLevels.mCurrentLevel * lMotionsPerLevel);

			mEventIndex = 0;
			mTimer = 0.0f;
			mTTSTimer = 0.0f;
			mPatternDone = false;

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            Buddy.Actuators.Head.No.SetPosition(0.0f);
        }

		public void DoEvent()
		{
			if (mEventIndex < mEvents.Count)
            {
                mEventManager.DoEvent((MemoryEvent)mEvents[mEventIndex]);
				mEventIndex++;
			}
            else
            {
				mPatternDone = true;
			}
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.ResetTrigger("NextLevel");
			InitLvl();
			Buddy.Vocal.Say(Buddy.Resources.GetRandomString("introlvl") + " " + mGameLevels.mCurrentLevel, true);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mTTSTimer += Time.deltaTime;

            if (!Buddy.Behaviour.Face.IsBusy && !mEventManager.IsMoving() && !Buddy.Vocal.IsBusy)
            {
                mTimer += Time.deltaTime;

                if (mPatternDone)
                {
                    animator.SetTrigger("RobotDone");
                }
                else if (mTTSTimer > 3.0f && mTimer > 1.0f)
                {
                    mTimer = 0;
                    DoEvent();
                }
            }
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}