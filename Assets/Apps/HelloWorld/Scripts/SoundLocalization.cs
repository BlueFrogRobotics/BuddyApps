using BlueQuark;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyApp.HelloWorld
{
    public sealed class SoundLocalization : AStateMachineBehaviour
    {
        private const int SOUND_LOC_SAMPLES = 4;
        private const float HUMAN_DETECTION_TIMER = 1F;

        private bool mLocIsActive;
        private bool mHumanDetectEnabled;
        private float mLastSoundLocTime;
        private float mHumanDetectionTime;
        private int mSoundLoc;
        private string mFinalSpeech;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            // Init variables & Sound Loc
            mLastSoundLocTime = -1;
            mLocIsActive = false;
            mHumanDetectEnabled = false;
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
            Buddy.Sensors.Microphones.EnableSoundLocalization = true;
            mFinalSpeech = Buddy.Resources.GetString("willdobetter");


            // Setting up human detector
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
                new HumanDetectorParameter { SensorMode = SensorMode.VISION });

            // Starting this state
            Buddy.Vocal.Say(Buddy.Resources.GetString("asksound"), (iSpeechOutput) =>
            {
                // Closing eyes
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.CLOSE_EYES, false);
                // Turn the back
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 80F, () => { mLocIsActive = true; });

                Buddy.Actuators.Head.ResetPosition();
                Buddy.Vocal.Say(Buddy.Resources.GetString("whostart"));
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mLocIsActive) {

                if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                    Debug.LogWarning("New sound loc " + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString());
                    mLastSoundLocTime = Time.time;
                    mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
                }

                if (mLastSoundLocTime > 0.200F && (Time.time - mLastSoundLocTime) < 1.2F) {
                    if (!Buddy.Navigation.IsBusy) {
                        Debug.LogError("--- SOUND LOC:" + mSoundLoc + " ---");
                        Buddy.Navigation.Run<DisplacementStrategy>().Rotate(-mSoundLoc, 80F, () =>
                        {
                            Buddy.Sensors.Microphones.EnableSoundLocalization = false;
                            Buddy.Sensors.Microphones.EnableEchoCancellation = true;
                            Debug.LogWarning("motion end ");
                            mHumanDetectEnabled = true;
                            mHumanDetectionTime = Time.time;
                            Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                            mLocIsActive = false;
                        });
                        Buddy.Actuators.Head.No.ResetPosition();
                    }
                }
                else
                    Debug.LogWarning("No motion bcs last sound loc too old: " + Time.time + " " + mLastSoundLocTime + " = " + (Time.time - mLastSoundLocTime));
            }

            // Disable HumanDetection after little timer
            if (mHumanDetectEnabled && (Time.time - mHumanDetectionTime) >= HUMAN_DETECTION_TIMER) {
                Debug.LogWarning("detect end ");
                mHumanDetectEnabled = false;
                Buddy.Vocal.Say(mFinalSpeech, (iSpeechOutput) =>
                {
                    StartCoroutine(HelloWorldUtils.PlayBIAsync(() => { Trigger("TouchAndCaress"); }));
                });
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Sensors.Microphones.EnableSoundLocalization = false;
            Buddy.Sensors.Microphones.EnableEchoCancellation = true;
        }

        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if (!mHumanDetectEnabled)
                return true;

            mFinalSpeech = Buddy.Resources.GetString("here");
            return true;
        }
    }
}
