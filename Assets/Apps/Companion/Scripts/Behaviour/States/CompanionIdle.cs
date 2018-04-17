using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Buddy;
using UnityEngine.UI;

namespace BuddyApp.Companion
{

	/// <summary>
	/// This class is used when the robot is in default mode
	/// It will then go wander, interact, look for someone or charge according to the stimuli
	/// </summary>
	public class CompanionIdle : AStateMachineBehaviour
	{
		private float mTimeIdle;
		private bool mHeadPlaying;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mActionTrigger = "";
			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			mState.text = "IDLE";
			Debug.Log("state: IDLE");

			mTimeIdle = 0F;

			BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");

			//Interaction.Face.SetEvent(FaceEvent.YAWN);
			//Primitive.Speaker.Voice.Play(VoiceSound.YAWN);

			StartCoroutine(SearchingHeadCo());
			mHeadPlaying = true;
		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE " + (DateTime.Now - BYOS.Instance.StartTime).TotalSeconds + "\n interactDesire: " + CompanionData.Instance.mInteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.mMovingDesire;

			if (BYOS.Instance.Interaction.BMLManager.DonePlaying) {
				if (!mHeadPlaying) {
					mHeadPlaying = true;
					SearchingHeadCo();
				}
				mTimeIdle += Time.deltaTime;
			}

			// Play BML after 4 seconds every 8 seconds or launch desired action
			if (((int)mTimeIdle) % 8 == 4 && BYOS.Instance.Interaction.BMLManager.DonePlaying) {
				mActionTrigger = mActionManager.DesiredAction(COMPANION_STATE.IDLE);

				// if no desired action
				if (string.IsNullOrEmpty(mActionTrigger) || mActionTrigger == "IDLE") {


					// if IDLE for a while and tired
					if (mTimeIdle > 100 + 10 * BYOS.Instance.Interaction.InternalState.Energy) {
						BYOS.Instance.Interaction.Mood.Set(MoodType.TIRED);
						mActionTrigger = "NAP";
						Trigger("NAP");
					} else {
						//if no desired action, play BML
						Debug.Log("Play neutral BML IDLE");
						mHeadPlaying = false;
						StopCoroutine(SearchingHeadCo());
						BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");
					}


				} else {
					// Otherwise trigger to perform the action
					Debug.Log("Trigger action " + mActionTrigger);
					Trigger(mActionTrigger);
				}
			}

			// Otherwise, react on all detectors
			if (string.IsNullOrEmpty(mActionTrigger) || mActionTrigger == "IDLE")
				if (mDetectionManager.mDetectedElement != Detected.NONE) {
					mActionTrigger = mActionManager.LaunchReaction(COMPANION_STATE.IDLE, mDetectionManager.mDetectedElement);
					Debug.Log("!!!!!!!!!!!Idle trigger " + mActionTrigger);
					Trigger(mActionTrigger);
				}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeIdle = 0F;

			Debug.Log("Idle exit");

			mDetectionManager.mDetectedElement = Detected.NONE;
			mHeadPlaying = false;
			StopCoroutine(SearchingHeadCo());
		}


		//This makes the head look right and left on random angles
		private IEnumerator SearchingHeadCo()
		{
			while (mHeadPlaying) {

				switch (UnityEngine.Random.Range(0, 2)) {
					case 0:
						TurnHeadNo(UnityEngine.Random.Range(20F, 30F), UnityEngine.Random.Range(60F, 150F));
						break;
					case 1:
						TurnHeadYes(UnityEngine.Random.Range(-15F, 40F), UnityEngine.Random.Range(60F, 150F));
						break;
				}
				yield return new WaitForSeconds(2.0F);
			}

		}

		private void TurnHeadNo(float iHeadNo, float iSpeed)
		{
			if (Primitive.Motors.NoHinge.CurrentAnglePosition > 0F)
				iHeadNo = -iHeadNo;

			Primitive.Motors.NoHinge.SetPosition(iHeadNo);
		}

		private void TurnHeadYes(float iHeadYes, float iSpeed)
		{
			Primitive.Motors.YesHinge.SetPosition(iHeadYes, iSpeed);
		}

	}
}
