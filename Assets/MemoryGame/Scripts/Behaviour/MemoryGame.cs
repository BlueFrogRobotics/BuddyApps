using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Buddy;

namespace BuddyApp.MemoryGame
{
	public class MemoryGame : AStateMachineBehaviour
	{

		private List<int> mEvents;
		private int mEventIndex;
		private float mTimer;
		private float mMaxTime;
		private bool mPatternDone;
		private float mTTSTimer;

		private bool mBuddyMotion;
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


		public void InitLvl()
		{
			CommonIntegers["isPlayerTurn"] = 0;


			Debug.Log("Memory Game Level " + mGameLevels.mCurrentLevel);
			int lMotionsPerLevel = 2;
			if (MemoryGameData.Instance.Difficulty > 1)
				lMotionsPerLevel = 3;
			mEvents = mGameLevels.events.GetRange(0, mGameLevels.mCurrentLevel * lMotionsPerLevel);

			// right eye = 3
			//Debug.Log((int)FaceEvent.BLINK_RIGHT);

			// left eye = 4
			//Debug.Log((int)FaceEvent.BLINK_LEFT);

			// smile = 0
			//Debug.Log((int)FaceEvent.SMILE);

			mEventIndex = 0;
			mTimer = 0.0f;
			mTTSTimer = 0.0f;
			mMaxTime = 1.0f / mGameLevels.speed;
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
							Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_1);
							StartCoroutine(ControlBuddy(BuddyMotion.HEAD_LEFT));
						} else if (mEvents[mEventIndex] == 9) {
							Debug.Log("Move head right");
							Primitive.Speaker.Voice.Play(VoiceSound.LAUGH_2);
							StartCoroutine(ControlBuddy(BuddyMotion.HEAD_RIGHT));
						} else if (mEvents[mEventIndex] == 10) {
							Debug.Log("Move wheels left");
							Primitive.Speaker.Voice.Play(VoiceSound.CURIOUS_1);
							StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_LEFT));
						} else if (mEvents[mEventIndex] == 11) {
							Debug.Log("Move wheels right");
							Primitive.Speaker.Voice.Play(VoiceSound.CURIOUS_2);
							StartCoroutine(ControlBuddy(BuddyMotion.WHEEL_RIGHT));
						}
					} else {
						//Set sound
						if ((FaceEvent)mEvents[mEventIndex] == FaceEvent.BLINK_LEFT)
							Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_1);
						else if ((FaceEvent)mEvents[mEventIndex] == FaceEvent.BLINK_RIGHT)
							Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_2);
						else if ((FaceEvent)mEvents[mEventIndex] == FaceEvent.SMILE)
							Primitive.Speaker.Voice.Play(VoiceSound.SURPRISED_3);


						//Set face event
						Interaction.Face.SetEvent((FaceEvent)mEvents[mEventIndex]);
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
				float lOriginAngle = Primitive.Motors.NoHinge.CurrentAnglePosition;
				float lTargetAngle;
				if (iMotion == BuddyMotion.HEAD_LEFT) {
					lTargetAngle = lOriginAngle + 45.0f;
				} else {
					lTargetAngle = lOriginAngle - 45.0f;
				}

				// Put the head to the given direction
				Debug.Log("Head target angle " + lTargetAngle);
				Primitive.Motors.NoHinge.SetPosition(lTargetAngle);

				// Wait for end of motion
				while (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition - lOriginAngle) < 20.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;
				}
				lTimer = 0.0f;
				Debug.Log("Moving head ok, move back ");
				// Put the head back
				Primitive.Motors.NoHinge.SetPosition(0.0f);

				// Wait for end of motion
				while (Math.Abs(Primitive.Motors.NoHinge.CurrentAnglePosition) > 5.0f || lTimer > 5.0f) {
					lTimer += Time.deltaTime;
					yield return null;

				}

			} else if (iMotion == BuddyMotion.HEAD_DOWN) {
				Debug.Log("Motion not available" + iMotion);
			} else if (iMotion == BuddyMotion.HEAD_UP) {
				Debug.Log("Motion not available" + iMotion);



				// Turning wheels
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

				Debug.Log("Moving wheels ok, move back ");

				// Put the robot back
				Primitive.Motors.Wheels.TurnAbsoluteAngle(mOriginRobotAngle, 100.0f, 0.02f);

				yield return new WaitForSeconds(0.5f);

				// Wait for end of motion
				while (Primitive.Motors.Wheels.Status != MovingState.REACHED_GOAL &&
					Primitive.Motors.Wheels.Status != MovingState.MOTIONLESS) {
					yield return null;
				}

			}

			yield return new WaitForSeconds(1.0f);
			Debug.Log("Motion done");

			mBuddyMotion = false;

		}


		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mGameLevels = ((MemoryGameRandomLevel)CommonObjects["gameLevels"]);
			animator.ResetTrigger("NextLevel");
			//link.mAnimationManager.gameObject.SetActive(false);
			Interaction.Mood.Set(MoodType.NEUTRAL);
			Primitive.Motors.NoHinge.SetPosition(0.0f);
			mOriginRobotAngle = Primitive.Motors.Wheels.Odometry.z;
			mBuddyMotion = false;
			InitLvl();
			Interaction.TextToSpeech.Say(Dictionary.GetRandomString("introlvl") + " " + mGameLevels.mCurrentLevel, true);

		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			//if (link.mUnloadingScene) {
			//	Debug.Log("Unloading");
			//	QuitApp();
			//}

			mTTSTimer += Time.deltaTime;

			if (Interaction.Face.IsStable && !mBuddyMotion && Interaction.TextToSpeech.HasFinishedTalking) {
				mTimer += Time.deltaTime;
			}

			if (!mPatternDone && Interaction.TextToSpeech.HasFinishedTalking && mTTSTimer > 3.0f) {
				DoEvent();
			} else if (mPatternDone && Interaction.Face.IsStable && !mBuddyMotion) {
				Debug.Log("pattern done, your turn");
				animator.SetTrigger("RobotDone");
			}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
		}
	}
}