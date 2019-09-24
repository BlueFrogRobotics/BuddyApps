﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBoxV3
{
    public class SoundLoc : AStateMachineBehaviour
    {

        private bool mHumanDetectEnabled;
        private bool mSoundLocEnabled;
        private bool mHumanDetected;
        private float mSoundLoc;
        private bool mRotateDone;
        private float mTimer;
        private bool mStartSL;
        private float mLastSoundLoc;


        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxV3Behaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("TRANSITION"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            OutOfBoxUtilsVThree.DebugColor("SOUNDLOC OOBV3", "red");
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Navigation.Stop();
            Buddy.Actuators.Head.Yes.ResetPosition();
            Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, 40);
            mTimer = 0F;
            mRotateDone = false;
            mHumanDetected = false;
            mSoundLocEnabled = false;
            mHumanDetectEnabled = false;
            mStartSL = false;

            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect, new HumanDetectorParameter { HumanDetectionMode = HumanDetectionMode.VISION});
            Buddy.Vocal.SayKey("soundlocintro", (iOut) => {
                if (!iOut.IsInterrupted)
                    Buddy.Vocal.SayKey("soundloctest",
                        (iOutLoc) => {
                            if (!iOutLoc.IsInterrupted)
                                StartSourceLoc();
                        });
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (mRotateDone)
                mTimer += Time.deltaTime;

            if (mStartSL) {
                mStartSL = false;
                Buddy.Sensors.Microphones.EnableSoundLocalization = true;
                Buddy.Vocal.SayKey("imready");
                mSoundLocEnabled = true;
            }

            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                mLastSoundLoc = Time.time;
                mSoundLoc = Convert.ToSingle(Buddy.Sensors.Microphones.SoundLocalization);
            }

            if (mSoundLocEnabled && mTimer < 12F && mLastSoundLoc > 0.200F && (Time.time - mLastSoundLoc) < 1.2F) {
                Buddy.Sensors.Microphones.EnableSoundLocalization = false;
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(mSoundLoc, 80F, () => { mHumanDetectEnabled = true; });
                mSoundLocEnabled = false;
            } else if (mTimer > 12F && mSoundLocEnabled) {
                //Passer à la suite ou stop l'app parce que plus personne n'est devant le robot
                mSoundLocEnabled = false;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 80F);
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                //QuitApp();
                //Trigger("TRANSITION");
            }

            if (mHumanDetected) {
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Vocal.SayKey("ifoundyou", (iOut) => { if(!iOut.IsInterrupted) Trigger("TRANSITION"); });
                
            } else if (!mHumanDetected && mTimer > 15F && !mSoundLocEnabled) {

                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Behaviour.Mood = Mood.THINKING;
                Buddy.Vocal.SayKey("ihearnoisehere", (iOut) => { Buddy.Behaviour.Mood = Mood.NEUTRAL; if (!iOut.IsInterrupted) Trigger("TRANSITION"); });
                
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Sensors.Microphones.SoundLocalizationParameters = null;
            mBehaviour.PhaseDropDown.value = 4;
        }

        private void StartSourceLoc()
        {
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 80F, () => {
                Buddy.Actuators.Head.No.ResetPosition();
                Buddy.Actuators.Head.Yes.ResetPosition();
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.CLOSE_EYES, false);
                mRotateDone = true;
                mStartSL = true;
            });
        }

        private bool OnHumanDetect(HumanEntity[] iHumanEntity)
        {
            if (!mHumanDetectEnabled)
                return true;
            else {
                mHumanDetected = true;
                return false;
            }
        }
    }

}
