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
	public class Nap : AStateMachineBehaviour
	{
		private float mTimeIdle;
		private bool mHeadPlaying;
		private bool mHeadUp;
		private float mTimeSleeping;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
			mCompanion = GetComponent<CompanionBehaviour>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mHeadUp = false;
			mActionTrigger = "";
			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
			mActionManager.CurrentAction = BUDDY_ACTION.NONE;
			mState.text = "Nap";
			Debug.Log("state: Nap");


			mCompanion.mCurrentUser = null;
			mTimeIdle = 0F;

			Interaction.Face.SetEvent(FaceEvent.YAWN);
			Primitive.Speaker.Voice.Play(VoiceSound.YAWN);
			Interaction.Face.LookAt(FaceLookAt.BOTTOM);
			Primitive.Motors.YesHinge.SetPosition(UnityEngine.Random.Range(-30F, -5F), 100F);

            if (iAnimator.GetInteger("Duration") == 0)
                mTimeSleeping = UnityEngine.Random.Range(30F, 200F);
            else
                mTimeSleeping = iAnimator.GetInteger("Duration");

			Debug.Log("Sleeping for: " + mTimeSleeping);
            StartCoroutine(SleepingHeadCo());
			mHeadPlaying = true;
		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "NAP " + (DateTime.Now - BYOS.Instance.StartTime).TotalSeconds + "\n interactDesire: " + CompanionData.Instance.mInteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.mMovingDesire;

			if (Interaction.Face.IsStable && Interaction.Face.EyesOpen) {
				Interaction.Face.SetEvent(FaceEvent.CLOSE_EYES);
			}

			mActionTrigger = mActionManager.NeededAction(COMPANION_STATE.IDLE);
			if (!string.IsNullOrEmpty(mActionTrigger))
				Trigger(mActionTrigger);
			else {

				if (BYOS.Instance.Interaction.BMLManager.DonePlaying) {
					if (!mHeadPlaying) {
						mHeadPlaying = true;
						SleepingHeadCo();
					}
					mTimeIdle += Time.deltaTime;
				}

				// Play BML after 4 seconds every 8 seconds or launch desired action
				if (((int)mTimeIdle) > mTimeSleeping) {
					Debug.Log("mTimeIdle " + mTimeIdle + " > mTimeSleeping " + mTimeSleeping);
					mActionTrigger = mActionManager.DesiredAction(COMPANION_STATE.NAP);
					if (!string.IsNullOrEmpty(mActionTrigger) && mActionTrigger != "NAP") {
						// Otherwise trigger to perform the action
						Debug.Log("Trigger action " + mActionTrigger);
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(2, -5, "endnap", "END_NAP", EmotionalEventType.FULFILLED_DESIRE, InternalMood.RELAXED));
						Trigger(mActionTrigger);
					}
				}

				// Otherwise, react on all detectors
				if (string.IsNullOrEmpty(mActionTrigger) || mActionTrigger == "IDLE" || mActionTrigger == "NAP")
					if (mDetectionManager.mDetectedElement != Detected.NONE) {
						mActionTrigger = mActionManager.LaunchReaction(COMPANION_STATE.NAP, mDetectionManager.mDetectedElement);
						BYOS.Instance.Interaction.InternalState.AddCumulative(new EmotionalEvent(-3, -5, "stoppednap", "STOPPED_NAP", EmotionalEventType.UNFULFILLED_DESIRE, InternalMood.GRUMPY));
						Debug.Log("!!!!!!!!!!!nap trigger " + mActionTrigger);
						Trigger(mActionTrigger);
						mDetectionManager.mDetectedElement = Detected.NONE;
					}

			}
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeIdle = 0F;
            iAnimator.SetInteger("Duration", 0);

			Interaction.Face.SetEvent(FaceEvent.OPEN_EYES);
			Debug.Log("Nap exit");

			mDetectionManager.mDetectedElement = Detected.NONE;
			mHeadPlaying = false;
			StopCoroutine(SleepingHeadCo());
		}


		//This makes the head look right and left on random angles
		private IEnumerator SleepingHeadCo()
		{

			yield return new WaitForSeconds(UnityEngine.Random.Range(0F, 5F));

			if (mHeadUp) {
				Primitive.Motors.YesHinge.SetPosition(UnityEngine.Random.Range(-30F, -5F), 200F);
				mHeadUp = false;
			} else {
				Primitive.Motors.YesHinge.SetPosition(UnityEngine.Random.Range(0F, 20F), 50F);
				mHeadUp = true;
			}

		}


	}
}
