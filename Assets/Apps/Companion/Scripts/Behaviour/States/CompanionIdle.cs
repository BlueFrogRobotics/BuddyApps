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
		private string mActionTrigger;

		private const int CES_HACK = 10;
		private DesireManager mDesireManager;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			mDetectionManager = GetComponent<DetectionManager>();
			mDesireManager = GetComponent<DesireManager>();
			mActionManager = GetComponent<ActionManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{

			mActionManager.StopAllActions();
			mDetectionManager.mDetectedElement = Detected.NONE;
			mState.text = "IDLE";
			Debug.Log("state: IDLE");
			mActionTrigger = "";

			mTimeIdle = 0F;

			Interaction.Face.SetEvent(FaceEvent.YAWN);
			Primitive.Speaker.Voice.Play(VoiceSound.YAWN);


			//TODO: remove this when BML
			StartCoroutine(SearchingHeadCo());
			mHeadPlaying = true;
		}



		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "IDLE \n bored: " + CompanionData.Instance.Bored + "\n interactDesire: " + CompanionData.Instance.InteractDesire
				+ "\n wanderDesire: " + CompanionData.Instance.MovingDesire;

			if (BYOS.Instance.Interaction.BMLManager.DonePlaying) {
				if (!mHeadPlaying) {
					mHeadPlaying = true;
					SearchingHeadCo();
				}
				mTimeIdle += Time.deltaTime;
			}

			// Play BML after 4 seconds every 8 seconds or launch desired action
			if (((int)mTimeIdle) % 8 == 4 && BYOS.Instance.Interaction.BMLManager.DonePlaying) {
				mActionTrigger = mActionManager.LaunchDesiredAction("IDLE");
				if (string.IsNullOrEmpty(mActionTrigger)) {
					//if no desired action, play BML
					Debug.Log("Play neutral BML IDLE");
					mHeadPlaying = false;
					StopCoroutine(SearchingHeadCo());
					BYOS.Instance.Interaction.BMLManager.LaunchRandom("neutral");
				} else {
					// Otherwise trigger to perform the action
					Trigger(mActionTrigger);
				}
			}

			// Otherwise, react on almost all detectors
			if (string.IsNullOrEmpty(mActionTrigger)) {
				if (mDetectionManager.mDetectedElement == Detected.TRIGGER || mDetectionManager.mDetectedElement == Detected.TOUCH || mDetectionManager.mDetectedElement == Detected.THERMAL ||
					mDetectionManager.mDetectedElement == Detected.KIDNAPPING || mDetectionManager.mDetectedElement == Detected.BATTERY || mDetectionManager.mDetectedElement == Detected.HUMAN_RGB) {

					Trigger(mActionManager.LaunchReaction("IDLE", mDetectionManager.mDetectedElement));

				}
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

		//////// CALLBACKS

		//void OnRandomMinuteActivation()
		//{
		//	CompanionData.Instance.Bored += (5 + CES_HACK);
		//}

		//void OnMinuteActivation()
		//{
		//	//int lRand = UnityEngine.Random.Range(0, 101);

		//	//if (lRand < CompanionData.Instance.Bored) {
		//	//CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored / 10;
		//	//CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored / 10;

		//	//TODO remove this (CES hack)

		//	if (UnityEngine.Random.Range(0, 2) == 0)
		//		CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored;
		//	else
		//		CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored;

		//	Interaction.Face.SetEvent(FaceEvent.YAWN);
		//	Primitive.Speaker.Voice.Play(VoiceSound.YAWN);
		//	//}
		//}

	}
}
