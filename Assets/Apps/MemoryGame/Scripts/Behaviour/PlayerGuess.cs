using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BlueQuark;

namespace BuddyApp.MemoryGame
{
	public class PlayerGuess : AStateMachineBehaviour
	{
		static bool mFail;
		static bool mSuccess;

		static List<int> mEvents;
		static List<int> mCurrentEvents;

		public const float ANGLE_THRESH = 10.0F;
        public const float MIN_TIME_BTW_TOUCH = 1.0F;

		float mWaitTimer;
		float mEndMaxTime;
        float mLastTouchTime;

		private bool mBuddyMotion;
		private MemoryGameRandomLevel mGameLevels;
        private EventManager mEventManager;

		public override void Start()
		{
			mGameLevels = GetComponent<MemoryGameRandomLevel>();
            mEventManager = GetComponent<EventManager>();
            mCurrentEvents = new List<int>();
        }

		public void InitGuess()
		{
			mCurrentEvents.Clear();
			mEvents = mGameLevels.events.GetRange(0, mGameLevels.mCurrentLevel * 2);

			mFail = false;
			mSuccess = false;

			mWaitTimer = 0.0f;
			mEndMaxTime = 20.0f;
            mLastTouchTime = Time.time;
		}

        private void SensorTouched(MemoryEvent evt)
        {
            if (mEventManager.IsMotionEvent(evt))
            {
                // Wait between motion events
                if ((Time.time - mLastTouchTime) <= MIN_TIME_BTW_TOUCH)
                    return;
            }
            mCurrentEvents.Add((int)evt);

            mEventManager.DoEvent(evt);

            if (IsSuccess())
            {
                if (mCurrentEvents.Count.Equals(mEvents.Count))
                {
                    mSuccess = true;
                }
            }
            else
            {
                mFail = true;
            }
            mLastTouchTime = Time.time;
        }

        public static bool IsSuccess()
		{
			if (mSuccess)
            {
				return true;
			}
            else if (mCurrentEvents.Count > mEvents.Count)
            {
				return false;
			}
            else
            {
				for (int i = 0; i < mCurrentEvents.Count; i++)
                {
					if (!mCurrentEvents[i].Equals(mEvents[i]))
                    {
						return false;
					}
				}
			}
			return true;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			InitGuess();

			Buddy.Behaviour.Face.OnTouchLeftEye.Add(() => { SensorTouched(MemoryEvent.LEFT_EYE); });
            Buddy.Behaviour.Face.OnTouchRightEye.Add(() => { SensorTouched(MemoryEvent.RIGHT_EYE); });
            Buddy.Behaviour.Face.OnTouchMouth.Add(() => { SensorTouched(MemoryEvent.MOUTH); });

            Buddy.Sensors.TouchSensors.RightHead.OnTouch.Add(() => { SensorTouched(MemoryEvent.RIGHT_HEAD); });
            Buddy.Sensors.TouchSensors.BackHead.OnTouch.Add(() => { SensorTouched(MemoryEvent.BACK_HEAD); });
            Buddy.Sensors.TouchSensors.LeftHead.OnTouch.Add(() => {SensorTouched(MemoryEvent.LEFT_HEAD);});

            Buddy.Sensors.TouchSensors.LeftShoulder.OnTouch.Add(() => { SensorTouched(MemoryEvent.TURN_LEFT); });
            Buddy.Sensors.TouchSensors.RightShoulder.OnTouch.Add(() => { SensorTouched(MemoryEvent.TURN_RIGHT); });

            Buddy.Vocal.Say(Buddy.Resources.GetRandomString("yourturn"), true);
			mWaitTimer = 0.0f;
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            if (!Buddy.Behaviour.Face.IsBusy && !mEventManager.IsMoving() && !Buddy.Vocal.IsBusy)
            {
                mWaitTimer += Time.deltaTime;
                if (mWaitTimer > 1.0f)
                {
                    if (mFail)
                        animator.SetTrigger("PlayerFailure");
                    else if (mSuccess)
                        animator.SetTrigger("PlayerSuccess");
                    else if (mWaitTimer > mEndMaxTime)
                        animator.SetTrigger("PlayerFailure");
                }
			}
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            Buddy.Behaviour.Face.OnTouchLeftEye.Clear();
			Buddy.Behaviour.Face.OnTouchRightEye.Clear();
            Buddy.Behaviour.Face.OnTouchMouth.Clear();

            Buddy.Sensors.TouchSensors.RightHead.OnTouch.Clear();
            Buddy.Sensors.TouchSensors.BackHead.OnTouch.Clear();
            Buddy.Sensors.TouchSensors.LeftHead.OnTouch.Clear();

            Buddy.Sensors.TouchSensors.LeftShoulder.OnTouch.Clear();
            Buddy.Sensors.TouchSensors.RightShoulder.OnTouch.Clear();
		}
	}
}