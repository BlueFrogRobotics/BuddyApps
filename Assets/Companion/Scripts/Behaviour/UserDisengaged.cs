using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
    public class UserDisengaged : AStateMachineBehaviour
    {
        private float mTimeState;
        //private float mTimeHumanDetected;
        //private bool mVocalTriggered;
        //private bool mNeedCharge;
        //private bool mReallyNeedCharge;
        //private bool mKidnapping;

        public override void Start()
        {
            //Perception.Stimuli = BYOS.Instance.SensorManager;
            mState = GetComponentInGameObject<Text>(0);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mState.text = "User disengaged";
            Debug.Log("state: User disengaged");

            mTimeState = 0F;
            //mTimeHumanDetected = 0F;
            //mVocalTriggered = false;

            //Perception.Stimuli.RegisterStimuliCallback(StimuliEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            //Perception.Stimuli.RegisterStimuliCallback(StimuliEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            //Perception.Stimuli.RegisterStimuliCallback(StimuliEvent.HUMAN_DETECTED, OnHumanDetected);
            //Perception.Stimuli.RegisterStimuliCallback(StimuliEvent.KIDNAPPING, OnKidnapping);
            //Perception.Stimuli.RegisterStimuliCallback(StimuliEvent.FACE_DETECTED, OnHumanDetected);


            //Perception.Stimuli.mStimuliControllers[StimuliEvent.SPHINX_TRIGGERED].StartListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.VERY_LOW_BATTERY].StartListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.HUMAN_DETECTED].StartListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.KIDNAPPING].StartListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.FACE_DETECTED].StartListenning();

            Interaction.Mood.Set(MoodType.NEUTRAL);

        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //	mTimeHumanDetected += Time.deltaTime;
            mTimeState += Time.deltaTime;

            //	// 0) If trigger vocal or kidnapping or low battery, go to corresponding state
            //	if (mVocalTriggered) {
            //		iAnimator.SetTrigger("VOCALTRIGGERED");

            //	} else if (mKidnapping) {
            //		Interaction.Mood.Set(MoodType.HAPPY);
            //		iAnimator.SetTrigger("KIDNAPPING");

            //	} else if (mReallyNeedCharge) {
            //		iAnimator.SetTrigger("CHARGE");

            //		// 1) If no more human detected for a while, go back to IDLE or go to sad buddy
            //	} else if (mTimeHumanDetected > 59F) {
            //		iAnimator.SetTrigger("SADBUDDY");


            //		// 2) after a while
            //	} else if (mTimeState > 45F) {
            //		Interaction.Mood.Set(MoodType.GRUMPY);
            //		Interaction.TextToSpeech.Say("Au cas où tu ne l'aurais pas remarqué, je cherche à attirer ton attention!", true);

            //	} else if (mTimeState > 60F) {
            //		iAnimator.SetTrigger("SADBUDDY");

            //		// 3) Otherwise, do crazy stuff
            //	} else {
            //		// TODO -> move / dance / make noise
            //	}

            if (CompanionData.Instance.InteractDesire < 0) {
                CompanionData.Instance.InteractDesire = 0;
            } else {
                CompanionData.Instance.InteractDesire -= 10;
            }

            if (CompanionData.Instance.MovingDesire > 50) {
                Debug.Log("User disengaged -> wander: " + CompanionData.Instance.MovingDesire);
                iAnimator.SetTrigger("WANDER");
            } else if (mTimeState > 10F) {
                iAnimator.SetTrigger("IDLE");
            }

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            //Perception.Stimuli.RemoveStimuliCallback(StimuliEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            //Perception.Stimuli.RemoveStimuliCallback(StimuliEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            //Perception.Stimuli.RemoveStimuliCallback(StimuliEvent.HUMAN_DETECTED, OnHumanDetected);
            //Perception.Stimuli.RemoveStimuliCallback(StimuliEvent.KIDNAPPING, OnKidnapping);
            //Perception.Stimuli.RemoveStimuliCallback(StimuliEvent.FACE_DETECTED, OnHumanDetected);

            //Perception.Stimuli.mStimuliControllers[StimuliEvent.SPHINX_TRIGGERED].StopListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.VERY_LOW_BATTERY].StopListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.HUMAN_DETECTED].StopListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.KIDNAPPING].StopListenning();
            //Perception.Stimuli.mStimuliControllers[StimuliEvent.FACE_DETECTED].StopListenning();

        }

        //void OnSphinxActivation()
        //{
        //	mVocalTriggered = true;
        //}

        //void OnVeryLowBattery()
        //{
        //	Interaction.Mood.Set(MoodType.TIRED);
        //	mReallyNeedCharge = true;
        //}

        //void OnHumanDetected()
        //{
        //	mTimeHumanDetected = 0F;
        //}

        //void OnKidnapping()
        //{
        //	mKidnapping = true;
        //}


    }
}