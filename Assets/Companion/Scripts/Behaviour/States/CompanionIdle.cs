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
        private bool mVocalTrigger;
        private bool mBatteryLow;
        private bool mBatteryVeryLow;
        private bool mFaceDetected;
        private bool mHumanDetected;
        private bool mBuddyMotion;
        private bool mKidnapping;
		private EyesFollowThermal mEyesFollowThermal;

		public override void Start()
        {
            //Perception.Stimuli = BYOS.Instance.SensorManager;
            mState = GetComponentInGameObject<Text>(0);
			mEyesFollowThermal = GetComponent<EyesFollowThermal>();
		}

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mState.text = "IDLE";
            Debug.Log("state: IDLE");

			mEyesFollowThermal.enabled = true;


			mVocalTrigger = false;
            mBatteryLow = false;
            mBatteryVeryLow = false;
            mFaceDetected = false;
            mKidnapping = false;
            mBuddyMotion = false;
            mHumanDetected = false;

            CompanionData.Instance.Bored = 0;
            CompanionData.Instance.InteractDesire = 60;

            Interaction.Mood.Set(MoodType.NEUTRAL);

            mTimeIdle = 0F;
            mPreviousTime = 0F;
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RegisterStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);


			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.POSITION_UPDATE].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.LOW_BATTERY].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Enable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Enable();


			//BYOS.Instance.Speaker.Voice.Play(VoiceSound.YAWN);
		}


        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mState.text = "IDLE \n bored: " + CompanionData.Instance.Bored + "\n interactDesire: " + CompanionData.Instance.InteractDesire
                + "\n wanderDesire: " + CompanionData.Instance.MovingDesire;
            mTimeIdle += Time.deltaTime;

            // Do the following every second
            if (mTimeIdle - mPreviousTime > 1F) {
                int lRand = UnityEngine.Random.Range(0, 100);

                if (lRand < (int)mTimeIdle / 10) {
                    CompanionData.Instance.Bored += 1;
                }
                mPreviousTime = mTimeIdle;

            }



            if (mFaceDetected && (mTimeIdle > 5F)) {
                Debug.Log("IDLE facedetected + time 5");
                // If Buddy sees a face and doesn't want to interact
                if (CompanionData.Instance.InteractDesire < 50 && mBatteryLow) {
                    //Interaction.TextToSpeech.Say("Salut mon ami! [500] Je suis fatigué, puis-je aller me reposer?", true);
                    iAnimator.SetTrigger("ASKCHARGE");

                    //If its battery is too low
                } else if (mBatteryVeryLow) {
                    Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
                    iAnimator.SetTrigger("CHARGE");

                    //Otherwise
                } else {
                    //Interaction.TextToSpeech.Say("Hey, voulez vous jouer avec moi?", true);
                    //Interaction.Mood.Set(MoodType.HAPPY);
                    Interaction.Mood.Set(MoodType.HAPPY);
                    iAnimator.SetTrigger("INTERACT");
                }

            } else if (mHumanDetected && (mTimeIdle > 5F)) {
                Debug.Log("HUMANDETECTED");
                Interaction.Mood.Set(MoodType.HAPPY);
                iAnimator.SetTrigger("INTERACT");

            } else if (mBuddyMotion) {
                //if (CompanionData.Instance.InteractDesire < 10) {
                //	Interaction.Mood.Set(MoodType.ANGRY);
                //	Interaction.TextToSpeech.Say("Pourquoi me pousses-tu? Grrr", true);
                //} else if (CompanionData.Instance.InteractDesire < 50) {
                //	Interaction.Mood.Set(MoodType.GRUMPY);
                //	Interaction.TextToSpeech.Say("Que fais-tu?", true);
                //} else if (CompanionData.Instance.InteractDesire > 70) {
                //	BYOS.Instance.Speaker.Voice.Play(VoiceSound.RANDOM_LAUGH);
                //	Interaction.Mood.Set(MoodType.HAPPY);
                //	Interaction.TextToSpeech.Say("Que fais-tu?", true);
                //}

                Debug.Log("BuddyMotion");

                //iAnimator.SetTrigger("ROBOTTOUCHED");

            } else if (mKidnapping) {
                Debug.Log("KIDNAPPING");
                iAnimator.SetTrigger("KIDNAPPING");

            } else if (mVocalTrigger) {
                // If Buddy is vocally triggered
                if (mBatteryVeryLow) {
                    Interaction.TextToSpeech.Say("Désolé mais je suis très fatigué, je vais me coucher! [800] Bonne nuit!", true);
                    iAnimator.SetTrigger("CHARGE");
                } else {
                    iAnimator.SetTrigger("VOCALTRIGGERED");
                }

            } else if (mBatteryLow) {
                //TODO put in dictionary
                Interaction.TextToSpeech.Say("Je commence à fatigué, je vais faire une petite sièste!", true);
                iAnimator.SetTrigger("CHARGE");

            } else if (mBatteryVeryLow) {
                //TODO put in dictionary
                Interaction.TextToSpeech.Say("Je suis très fatigué, je vais me coucher! [800] Bonne nuit tout le monde!", true);
                iAnimator.SetTrigger("CHARGE");

            } else if ((CompanionData.Instance.InteractDesire > 70) && (mTimeIdle > 5F)) {
                Debug.Log("LOOKINGFOR");
                iAnimator.SetTrigger("LOOKINGFOR");

            } else if ((CompanionData.Instance.MovingDesire > 70) && (mTimeIdle > 5F)) {
                Debug.Log("WANDER");
                iAnimator.SetTrigger("WANDER");
            }

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimeIdle = 0F;
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.RANDOM_ACTIVATION_MINUTE, OnRandomMinuteActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.REGULAR_ACTIVATION_MINUTE, OnMinuteActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.SPHINX_TRIGGERED, OnSphinxActivation);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.LOW_BATTERY, OnLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.VERY_LOW_BATTERY, OnVeryLowBattery);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.HUMAN_DETECTED, OnHumanDetected);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.KIDNAPPING, OnKidnapping);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.POSITION_UPDATE, OnPositionUpdate);
            Perception.Stimuli.RemoveStimuliCallback(StimulusEvent.FACE_DETECTED, OnFaceDetected);



			Perception.Stimuli.Controllers[StimulusEvent.RANDOM_ACTIVATION_MINUTE].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.REGULAR_ACTIVATION_MINUTE].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.SPHINX_TRIGGERED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.HUMAN_DETECTED].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.KIDNAPPING].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.POSITION_UPDATE].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.LOW_BATTERY].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.VERY_LOW_BATTERY].Disable();
			Perception.Stimuli.Controllers[StimulusEvent.FACE_DETECTED].Disable();

		}



        void OnRandomMinuteActivation()
        {
            CompanionData.Instance.Bored += 5;
        }

        void OnMinuteActivation()
        {
            int lRand = UnityEngine.Random.Range(0, 100);

            if (lRand < CompanionData.Instance.Bored) {
                CompanionData.Instance.InteractDesire += CompanionData.Instance.Bored / 10;
                CompanionData.Instance.MovingDesire += CompanionData.Instance.Bored / 10;
                Interaction.Face.SetEvent(FaceEvent.YAWN);
            }
        }

        void OnKidnapping()
        {
            mKidnapping = true;
        }

        void OnPositionUpdate()
        {
            mBuddyMotion = true;
        }

        void OnHumanDetected()
        {
            mHumanDetected = true;
        }

        void OnFaceDetected()
        {
            Debug.Log("IDLE: user detected");
            mFaceDetected = true;
        }

        void OnLowBattery()
        {
            Interaction.Mood.Set(MoodType.TIRED);
            mBatteryLow = true;
        }

        void OnVeryLowBattery()
        {
            Interaction.Mood.Set(MoodType.TIRED);
            mBatteryVeryLow = true;
        }

        void OnSphinxActivation()
        {
            mVocalTrigger = true;
        }
    }
}