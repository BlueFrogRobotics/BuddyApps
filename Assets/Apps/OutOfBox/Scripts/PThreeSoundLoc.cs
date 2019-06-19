using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PThreeSoundLoc : AStateMachineBehaviour {

        private bool mHumanDetectEnabled;
        private bool mSoundLocEnabled;
        private bool mHumanDetected;
        private float mSoundLoc;
        private bool mRotateDone;
        private float mTimer;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer = 0F;
            mRotateDone = false;
            mHumanDetected = false;
            mSoundLocEnabled = false;
            mHumanDetectEnabled = false;
          
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,new HumanDetectorParameter { SensorMode = SensorMode.VISION });
            Buddy.Vocal.Say("pthreefirststep", (iOut) => { Buddy.Vocal.Say("pthreesecondstep", (iOutLoc) => { StartSourceLoc(); }); });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;

            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION && mSoundLocEnabled && mTimer < 12F) 
            {
                //if (mSoundLoc != Buddy.Sensors.Microphones.SoundLocalization)
                //{
                    
                mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
                Buddy.Actuators.Wheels.SetVelocities(0f, Convert.ToSingle(mSoundLoc));
                mSoundLocEnabled = false;
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.OPEN_EYES, false);
                OutOfBoxUtils.WaitTimeAsync(2F, () => { mHumanDetectEnabled = true; });
                //}
            }
            else if(mTimer > 12F && mSoundLocEnabled)
            {
                //Passer à la suite ou stop l'app parce que plus personne n'est devant le robot
                mSoundLocEnabled = false;
            }

            if (mHumanDetected)
            {
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Vocal.Say("pthreevoila");
                OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseFour;
                Trigger("Base");
            }
            else if(!mHumanDetected && mTimer > 15F && !mSoundLocEnabled)
            {
                Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
                Buddy.Vocal.Say("pthreedobetter");
                OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseFour;
                Trigger("Base");
            }

        }

        private void StartSourceLoc()
        {
            Buddy.Actuators.Wheels.SetVelocities(0F, 180F);
            Buddy.Behaviour.Face.PlayEvent(FacialEvent.FALL_ASLEEP, false);
            OutOfBoxUtils.WaitTimeAsync(2F, () => {
                Buddy.Sensors.Microphones.EnableSoundLocalization = true;
                Buddy.Vocal.Say("pthreewhostart");
                mSoundLocEnabled = true;
            });
        }

        private bool OnHumanDetect(HumanEntity[] iHumanEntity)
        {
            if(!mHumanDetectEnabled)
                return true;
            else
            {
                mHumanDetected = true;
                return false;
            }
        }
    }

}
