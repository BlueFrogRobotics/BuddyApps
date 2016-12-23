using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace BuddyApp.Memory
{
	public class PlayerGuess : LinkStateMachineBehavior
	{

		static int clickIndex;

		static bool fail;
		static bool success;

		static List<FaceEvent> events;
		static List<FaceEvent> currentEvents;
		MemoryGameLevel currentLevel;

		float waitTimer;
		float endMaxTime;
		static bool resetTimer;

		float randomMoveTimer;
		float randomMoveTimerLimit;
		//bool randomMoveStarted;

		public void Init()
		{
			currentEvents = new List<FaceEvent>();
			events = link.currentLevel.faces;

			fail = false;
			success = false;
			clickIndex = 0;

			waitTimer = 0.0f;
			endMaxTime = 10.0f;
			resetTimer = false;

			randomMoveTimer = 0.0f;
			//randomMoveStarted = false;
			SetRandomTimerLimit();

			link.SetClickFace(ClickFace);
		}

		public static void ClickFace(int value)
		{

			switch (value) {
				case 0:
					Debug.Log("Click left eye from PlayerGuess script");
					currentEvents.Add(FaceEvent.BLINK_LEFT);
					break;
				case 1:
					Debug.Log("Click right eye from Player Guess Script");
					currentEvents.Add(FaceEvent.BLINK_RIGHT);
					break;
				case 2:
					Debug.Log("Click mouth from Player Guess Script");
					currentEvents.Add(FaceEvent.SMILE);
					break;
				default:
					Debug.Log("Click UNKOWN from Player Guess Script");
					fail = true;
					break;
			}
			clickIndex++;
			if (IsSuccess()) {
				if (currentEvents.Count.Equals(events.Count)) {
					success = true;
					resetTimer = true;
				}
			} else {
				fail = true;
			}
		}

		public static bool IsSuccess()
		{
			if (success) {
				return true;
			} else if (currentEvents.Count > events.Count) {
				return false;
			} else {
				for (int i = 0; i < currentEvents.Count; i++) {
					if (!currentEvents[i].Equals(events[i])) {
						Debug.Log("Comparing " + currentEvents[i] + " and " + events[i]);
						return false;
					}
				}
			}
			return true;
		}

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			Debug.Log("Player's turn");

			Init();

			link.tts.Say(link.gameLevels.GetRandomYourTurn(), true);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{

			if (link.tts.HasFinishedTalking) {
				waitTimer += Time.deltaTime;
				randomMoveTimer += Time.deltaTime;
				link.isPlayerTurn = true;
			}

			//if (link.mFace.IsStable && fail) {
			if (fail) {
				Debug.Log("Oups you failed");
				animator.SetTrigger("PlayerFailure");
			}

			//if (link.mFace.IsStable && success) {
			if (success) {

				if (resetTimer) {
					waitTimer = 0.0f;
					resetTimer = false;
				}

				if (waitTimer > 1.5f) {
					Debug.Log("Congrats, you win !");
					animator.SetTrigger("PlayerSuccess");
				}
			}

			if (waitTimer > endMaxTime) {
				Debug.Log("TimeOut game");
				animator.SetTrigger("PlayerFailure");
			}

			if (randomMoveTimer > randomMoveTimerLimit) {
				//			link.animationManager.StartAnimation ();
				RandomAnim();
				randomMoveTimer = 0.0f;
				SetRandomTimerLimit();
				//			randomMoveStarted = true;
			}
		}

		void RandomAnim()
		{
			switch (UnityEngine.Random.Range(0, 3)) {
				case 0:
					link.animationManager.Smile();
					break;
				case 1:
					link.animationManager.Sigh();
					break;
				case 2:
					link.animationManager.Blink();
					break;
				case 3:
					link.animationManager.Smile();
					break;
				default:
					link.animationManager.Blink();
					break;
			}
		}

		void SetRandomTimerLimit()
		{
			randomMoveTimerLimit = UnityEngine.Random.Range(0.5f, 3.0f);
			Debug.Log("randomMoveTimerLimit = " + randomMoveTimerLimit);
		}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			link.isPlayerTurn = false;
			//		link.animationManager.ResetPosition ();
		}

	}
}