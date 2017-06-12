using Buddy;
using Buddy.Features.Stimuli;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
	public class BuddyInArms : AStateMachineBehaviour
	{
		private float mTimeInArmns;
		private bool mVocalTriggered;

		public override void Start()
		{
			mState = GetComponentInGameObject<Text>(0);
			//mSensorManager = BYOS.Instance.SensorManager;
			mSensorManager = GetComponent<StimuliManager>();
		}

		public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mState.text = "Buddy in Arms";
			Debug.Log("state: Buddy in Arms");
			mVocalTriggered = false;
			mTimeInArmns = 0F;
			if (CompanionData.Instance.InteractDesire > 40) {
				mTTS.Say("J'aime être dans tes bras!", true);
				CompanionData.Instance.InteractDesire -= 10;
				mMood.Set(MoodType.HAPPY);
			} else {
				mTTS.Say("Laisse moi rouler!!!", true);
				CompanionData.Instance.InteractDesire -= 20;
				mMood.Set(MoodType.GRUMPY);
			}


			mSensorManager.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StartListenning();


		}


		public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mTimeInArmns += Time.deltaTime;

			if (mVocalTriggered)
				mTTS.Say("Que puis-je pour toi?", true);
			iAnimator.SetTrigger("VOCALTRIGGERED");
			if (mTimeInArmns > 5F)
				iAnimator.SetTrigger("INTERACT");
		}


		public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
		{
			mSensorManager.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
			//mSensorManager.mStimuliControllers[StimulusEvent.SPHINX_TRIGGERED].StopListenning();
		}

		void OnSphinxActivation()
		{
			mVocalTriggered = true;
		}
	}
}