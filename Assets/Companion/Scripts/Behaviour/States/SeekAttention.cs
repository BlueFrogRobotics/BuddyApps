using Buddy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuddyApp.Companion
{
    public class SeekAttention : AStateMachineBehaviour
    {
        private float mTimeState;
        private float mTimeHumanDetected;
        private bool mVocalTriggered;
        private bool mNeedCharge;
        private bool mReallyNeedCharge;
        private bool mKidnapping;
        private bool mGrumpy;

        public override void Start()
        {
            //mSensorManager = BYOS.Instance.SensorManager;

            mState = GetComponentInGameObject<Text>(0);
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mState.text = "Seek Attention";
            Debug.Log("state: Seek Attention" + BYOS.Instance.Primitive.Battery.EnergyLevel);

            mTimeState = 0F;
            mTimeHumanDetected = 0F;
            mVocalTriggered = false;
            mReallyNeedCharge = false;
            mNeedCharge = false;
            mGrumpy = false;

            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);

			
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Enable();

			Interaction.TextToSpeech.Say("Helloooooo", true);
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimeHumanDetected += Time.deltaTime;
            mTimeState += Time.deltaTime;

            // 0) If trigger vocal or kidnapping or low battery, go to corresponding state
            if (mVocalTriggered) {
                iAnimator.SetTrigger("VOCALTRIGGERED");

            } else if (mKidnapping) {
                Interaction.Mood.Set(MoodType.HAPPY);
                iAnimator.SetTrigger("KIDNAPPING");

            } else if (mReallyNeedCharge) {
                iAnimator.SetTrigger("CHARGE");

                // 1) If no more human detected for a while, go back to IDLE or go to sad buddy
            } else if (mTimeHumanDetected > 59F) {
                iAnimator.SetTrigger("SADBUDDY");


                // 2) after a while
            } else if (mTimeState > 45F && !mGrumpy) {
                Interaction.Mood.Set(MoodType.GRUMPY);
                mGrumpy = true;
                Interaction.TextToSpeech.Say("Au cas où tu ne l'aurais pas remarqué, je cherche à attirer ton attention!", true);

            } else if (mTimeState > 60F) {
                iAnimator.SetTrigger("SADBUDDY");

                // 3) Otherwise, do crazy stuff
            } else {
                // TODO -> move / dance / make noise
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnHumanDetected);


			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Disable();

		}

        void OnSphinxActivation()
        {
            mVocalTriggered = true;
        }

        void OnVeryLowBattery()
        {
            Interaction.Mood.Set(MoodType.TIRED);
            Debug.Log("SeekAttention really need charge" + BYOS.Instance.Primitive.Battery.EnergyLevel);
            mReallyNeedCharge = true;
        }

        void OnHumanDetected()
        {
            mTimeHumanDetected = 0F;
        }

        void OnKidnapping()
        {
            mKidnapping = true;
        }


    }
}