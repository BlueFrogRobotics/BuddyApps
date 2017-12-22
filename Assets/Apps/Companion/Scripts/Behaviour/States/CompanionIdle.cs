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
		private float mPreviousTime;
		private float mLastBMLTime;
		private bool mHeadPlaying;

		private const int CES_HACK = 10;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "IDLE";
			Debug.Log("state: IDLE");

			CompanionData.Instance.Bored = 0;

			mLastBMLTime = 0F;
			mTimeIdle = 0F;
			mPreviousTime = 0F;

			Interaction.Face.SetEvent(FaceEvent.YAWN);
			Primitive.Speaker.Voice.Play(VoiceSound.YAWN);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = true;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = true;

			//TODO: remove this when BML


			if (mActionManager.ThermalFollow) {
				mActionManager.StopThermalFollow();
			}

			//StartCoroutine(SearchingHeadCo());
			mHeadPlaying = true;
        }



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE \n bored: " + CompanionData.Instance.Bored + "\n interactDesire: " + CompanionData.Instance.InteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.MovingDesire;
			mTimeIdle += Time.deltaTime;


			if(mTimeIdle == 10%20 && BYOS.Instance.Interaction.BMLManager.DonePlaying)
						BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");

			// Do the following every second
			if (mTimeIdle - mPreviousTime > 1F) {
				int lRand = UnityEngine.Random.Range(0, 100);

				if (lRand < (int)mTimeIdle / 10) {
					CompanionData.Instance.Bored += 1;
				}
				mPreviousTime = mTimeIdle;

			}


			if (mTimeIdle > 10F && BYOS.Instance.Interaction.BMLManager.ActiveBML.Count == 0) {
				// Play BML from time to time. Play more when bored
				if (mTimeIdle - mLastBMLTime > UnityEngine.Random.Range(100F - CompanionData.Instance.Bored, 200F)) {
					//BYOS.Instance.Interaction.BMLManager.LaunchByID("happy1");
				}
			} else if (mTimeIdle > 10F) {
				mLastBMLTime = mTimeIdle;
			}


			if (mTimeIdle > 3F) {
				// if no event and no BML and high desires
				if (BYOS.Instance.Interaction.BMLManager.ActiveBML.Count == 0 && (mTimeIdle > 5F) && mDetectionManager.mDetectedElement == Detected.NONE) {
					if (CompanionData.Instance.InteractDesire > 70) {
						Debug.Log("LOOKINGFOR");
						Trigger("LOOKINGFOR");

					} else if (CompanionData.Instance.MovingDesire > 70) {
						Debug.Log("WANDER");
						Trigger("WANDER");
					}
				}

				// Otherwise, react on almost all detectors
				switch (mDetectionManager.mDetectedElement) {
					case Detected.TRIGGER:
						Debug.Log("Vocal triggered idle");
						Trigger("VOCALTRIGGERED");
						break;

					case Detected.TOUCH:
						Debug.Log("User Detected robot touched");
						Trigger("ROBOTTOUCHED");
						break;

					case Detected.KIDNAPPING:
						Trigger("KIDNAPPING");
						break;

					case Detected.BATTERY:
						Trigger("CHARGE");
						break;

					case Detected.HUMAN_RGB:
						Trigger("INTERACT");
						break;

					case Detected.THERMAL:
						Trigger("INTERACT");
						break;

					default:
						break;
				}
			}
		}

		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeIdle = 0F;

			Debug.Log("Idle exit");
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
			Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);

			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].enabled = false;
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].enabled = false;

			mDetectionManager.mDetectedElement = Detected.NONE;
			mHeadPlaying = false;
			//StopCoroutine(SearchingHeadCo());
        }



			
		//This makes the head look right and left on random angles
		private IEnumerator SearchingHeadCo()
		{
			while (mHeadPlaying) {

				switch (UnityEngine.Random.Range(0, 2)) {
					case 0:
						TurnHeadNo(UnityEngine.Random.Range(10F, 30F), UnityEngine.Random.Range(40F, 60F));
						break;
					case 1:
						TurnHeadYes(UnityEngine.Random.Range(- 15F, 15F), UnityEngine.Random.Range(40F, 60F));
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

		//////// CALLBACKS

		void OnRandomMinuteActivation()
		{
			CompanionData.Instance.Bored += 5 + CES_HACK;
		}

		void OnMinuteActivation()
		{
			int lRand = UnityEngine.Random.Range(0, 101);

			if (lRand < CompanionData.Instance.Bored) {
				//CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored / 10;
				//CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored / 10;

				//TODO remove this (CES hack)

				if(UnityEngine.Random.Range(0, 2) == 0)
					CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored * CES_HACK;
				else
					CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored * CES_HACK;

				Interaction.Face.SetEvent(FaceEvent.YAWN);
				Primitive.Speaker.Voice.Play(VoiceSound.YAWN);
			}
		}

	}
}