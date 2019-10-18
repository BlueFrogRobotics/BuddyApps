using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PThreeSoundLoc : AStateMachineBehaviour
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

            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
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

            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect, new HumanDetectorParameter { HumanDetectionMode = HumanDetectionMode.VISION });
            Buddy.Vocal.SayKey("pthreefirststep", (iOut) => {
                if (!iOut.IsInterrupted)
                    Buddy.Vocal.SayKey("pthreesecondstep",
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
                Buddy.Vocal.SayKey("pthreewhostart");
                mSoundLocEnabled = true;
            }

            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                mLastSoundLoc = Time.time;
                mSoundLoc = Convert.ToSingle(Buddy.Sensors.Microphones.SoundLocalization);
            }

            if (mSoundLocEnabled && mTimer < 12F && mLastSoundLoc > 0.200F && (Time.time - mLastSoundLoc) < 1.2F) {
                Buddy.Sensors.Microphones.EnableSoundLocalization = false;
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                Buddy.Actuators.Head.Yes.SetPosition(25F, 75F);
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(mSoundLoc, 80F, () => { mHumanDetectEnabled = true; });
                mSoundLocEnabled = false;
            } else if (mTimer > 12F && mSoundLocEnabled) {
                //Passer à la suite ou stop l'app parce que plus personne n'est devant le robot
                mSoundLocEnabled = false;
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(180F, 80F);
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                //QuitApp();
                //Trigger("Base");
            }

            if (mHumanDetected) {
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Vocal.SayKey("pthreevoila", (iOut) => { if(!iOut.IsInterrupted)
                        mBehaviour.PhaseDropDown.value = 3;
                });
                
            } else if (!mHumanDetected && mTimer > 15F && !mSoundLocEnabled) {
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Vocal.SayKey("pthreedobetter", (iOut) => { if (!iOut.IsInterrupted)
                        mBehaviour.PhaseDropDown.value = 3;
                });
                
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            StopAllCoroutines();
            Buddy.Navigation.Stop();
            Buddy.Actuators.Wheels.Stop();
            Buddy.Actuators.Head.Stop();
            Buddy.Behaviour.Stop();
            Buddy.Vocal.StopAndClear();
            Buddy.Behaviour.ResetMood();
            Buddy.Sensors.Microphones.SoundLocalizationParameters = null;
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
