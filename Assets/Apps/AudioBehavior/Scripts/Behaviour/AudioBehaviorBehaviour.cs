using BlueQuark;

using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

namespace BuddyApp.AudioBehavior
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class AudioBehaviorBehaviour : MonoBehaviour
    {
        /*
         * Data of the application. Save on disc when app is quitted
         */

        ThermalDetector mThermalDetector;

        private AudioBehaviorData mAppData;
        private float mLastSoundLocTime;
        private int mSoundLoc;
        private int mLastAverageAmbiant;
        private List<int> mAverageAmbiant;
        private float mLastTime;
        private float mTimeHumanDetected;
        private float mAngleLastDetect;
        private bool mGoTowardHuman;
        private bool mYolo;
        private float mAngleAtTrigger;
        private float mTimeTrigger;
        private int mRotation;
        private float mTargetOdom;
        private bool mThermal;
        private bool mHumanTracking;
        private int mMotion;

        public void ToggleYolo()
        {
            mYolo = !mYolo;

            if (mYolo) {
                mThermal = false;
                Buddy.Actuators.Head.Yes.SetPosition(5F);
                // Creation & Settings of parameters that will be used in detection
                Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
                    new HumanDetectorParameter {
                        SensorMode = SensorMode.VISION,
                    }
                );
            } else {
                Buddy.Perception.HumanDetector.OnDetect.Clear();
                Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
            }
        }

        public void ToggleThermal()
        {
            mThermal = !mThermal;

            if (mThermal) {
                mYolo = false;
                Buddy.Actuators.Head.Yes.SetPosition(5F);
                // Creation & Settings of parameters that will be used in detection
                mThermalDetector.mCallback = OnHumanDetect;
                mThermalDetector.Start();
            } else {
                mThermalDetector.Stop();
                Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
            }
        }

        public void ToggleHumanTracking()
        {
            mHumanTracking = !mHumanTracking;

            if (!mThermal && !mYolo)
                ToggleThermal();
        }

        void Start()
        {

            mThermalDetector = new ThermalDetector();

            /*
			* You can setup your App activity here.
			*/
            AudioBehaviorActivity.Init(null);

            mGoTowardHuman = false;
            mYolo = false;
            Buddy.Vocal.EnableTrigger = true;
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
            Buddy.Sensors.Microphones.EnableSoundLocalization = true;
            mAverageAmbiant = new List<int>();
            Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
            //    Buddy.Sensors.Microphones.SoundLocalizationParameters.Algorithm,
                Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution,
                40);
            Buddy.Sensors.Microphones.BeamformingParameters = new BeamformingParameters(6);
            Buddy.Vocal.OnCompleteTrigger.Add(BuddyTrigged);
            Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
        }

        /*
       *   On a human detection this function is called.
       */
        private void OnHumanDetect(HumanEntity[] iHumans)
        {
            mTimeHumanDetected = Time.time;
            mAngleLastDetect = Buddy.Actuators.Wheels.Odometry.AngleDeg();

            if (mGoTowardHuman || mHumanTracking) {
                double lCentered;
                if (mYolo)
                    lCentered = iHumans[0].Center.x / Buddy.Sensors.RGBCamera.Width;
                else
                    lCentered = iHumans[0].Center.x;
                if (lCentered < 0.6F && lCentered > 0.4F) {
                    mGoTowardHuman = false;
                    Buddy.Actuators.Wheels.Stop();
                    mMotion = 0;
                    // otherwise, try to put the human in the center
                } else if (lCentered < 0.5F && mMotion != 1) {
                    Debug.LogWarning("turn 45 human is at: " + lCentered);
                    mMotion = 1;
                    Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
                } else if (lCentered > 0.5 && mMotion != 2) {
                    Debug.LogWarning("turn -45 human is at: " + lCentered);
                    mMotion = 2;
                    Buddy.Actuators.Wheels.SetVelocities(0F, -45F);
                }

                // If we find human before sound loc destination, stop navigation and center on human
            } else if (Math.Abs(mTargetOdom - mAngleLastDetect) < 20) {
                Buddy.Navigation.Stop();
                mGoTowardHuman = true;
            }
        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            // Reset listen + callback of search

            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
            if (mThermal)
                mThermalDetector.mCallback = OnHumanDetect;
            Buddy.Vocal.Stop();
            Buddy.Actuators.Wheels.Stop();

            mAngleAtTrigger = Buddy.Actuators.Wheels.Odometry.AngleDeg();
            mTimeTrigger = Time.time;

            // Restart Yolo
            //if (mYolo) {
            //    mThermal = false;
            //    Buddy.Actuators.Head.Yes.SetPosition(5F);
            //    // Creation & Settings of parameters that will be used in detection
            //    Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
            //        new HumanDetectorParameter {
            //            SensorMode = SensorMode.VISION,
            //        }
            //    );
            //}

            Debug.LogWarning("my angle " + mSoundLoc);
            Debug.LogWarning("os angle " + iHotWord.SoundLocalization);
            Debug.LogWarning("reco score " + iHotWord.RecognitionScore);
            Debug.LogWarning("angle at trigger " + mAngleAtTrigger);
            Debug.LogWarning("time " + DateTime.Now.ToString());

            // Check if human already present, then just stay in front.
            if (Time.time - mTimeHumanDetected < 0.5F /*&& Math.Abs(iHotWord.SoundLocalization) < 40*/) {
                mGoTowardHuman = true;
                OnEndSearch();

                // Check if soundloc ok
            } else if (iHotWord.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                mGoTowardHuman = false;
                Debug.LogWarning("motion to last sound loc : " + iHotWord.SoundLocalization);
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(iHotWord.SoundLocalization, 80F, OnEndMoveSoundLoc);
                mRotation = iHotWord.SoundLocalization;
                mTargetOdom = iHotWord.SoundLocalization + mAngleAtTrigger;

                if (mTargetOdom > 180F)
                    mTargetOdom = mTargetOdom - 360F;
                else if (mTargetOdom < -180F)
                    mTargetOdom = mTargetOdom + 360F;

                // Otherwise, look for human if yolo
            } else {
                Buddy.Behaviour.SetMood(Mood.THINKING);
                Debug.LogWarning("No motion bcs last sound loc too old: " + Time.time + " " + mLastSoundLocTime + " = " + (Time.time - mLastSoundLocTime));
                Buddy.Vocal.Say("Je ne te vois pas");

                // find human
                // TODO find with thermal?
                if (mYolo) {
                    Buddy.Actuators.Wheels.SetVelocities(0F, 45F);
                    mGoTowardHuman = true;
                    Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound,
                    new HumanDetectorParameter {
                        SensorMode = SensorMode.VISION,
                    }
                );

                } else if (mThermal) {
                    mThermalDetector.mCallback = OnHumanFound;
                } else {
                    Buddy.Vocal.Say("Mais je t'écoute");
                    OnEndSearch();
                }

            }

        }

        private void OnEndMoveSoundLoc()
        {
            Debug.LogWarning("End motion of soundloc ");
            mGoTowardHuman = true;
            if (Time.time - mTimeHumanDetected < 0.5F || !mYolo && !mThermal) {
                Debug.LogWarning("End motion of soundloc and human seen ");
                OnEndSearch();
                // Go further
            } else {
                Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.THINKING);
                Debug.LogWarning("End motion of soundloc and ...");

                // A human was detected after human trigger
                if (mTimeHumanDetected > mTimeTrigger) {


                    Debug.LogWarning("human seen, rotate  " + (-mRotation / Math.Abs(mRotation)) *
                        (Math.Abs(mRotation) - (Math.Abs(Buddy.Actuators.Wheels.Odometry.AngleDeg() - mAngleLastDetect) % 360)));

                    Debug.LogWarning("human seen, rotate to " + mAngleLastDetect);
                    Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleLastDetect, 80F, OnEndSearch);

                } else {
                    if (mYolo) {
                        // Keep turning the same way
                        Buddy.Actuators.Wheels.SetVelocities(0F, (-mRotation / Math.Abs(mRotation)) * 45F);
                        Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound,
                        new HumanDetectorParameter {
                            SensorMode = SensorMode.VISION,
                        }
                        );
                    } else if (mThermal) {
                        // Keep turning the same way
                        Buddy.Actuators.Wheels.SetVelocities(0F, (-mRotation / Math.Abs(mRotation)) * 45F);
                        mThermalDetector.mCallback = OnHumanFound;
                    } else {
                        // Go back to initial position
                        Debug.LogWarning("no human seen, rotate to " + mRotation / Math.Abs(mRotation) * (360 - Math.Abs(mRotation)));
                        Buddy.Navigation.Run<DisplacementStrategy>().Rotate(mRotation / Math.Abs(mRotation) * (360 - Math.Abs(mRotation)), 80F, OnEndMoveNoHuman);
                    }
                }
            }

        }

        private void OnEndSearch(Vector3 obj)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            OnEndSearch();
        }

        private void OnEndMoveNoHuman()
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

            Debug.LogWarning("We did not found a human!!");
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            Buddy.Vocal.Say("Je ne t'ai pas trouvai mais je t'aicoute");
            Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleAtTrigger, 80F, OnEndSearch);
        }

        private void OnHumanFound(HumanEntity[] obj)
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
            if (mThermal)
                mThermalDetector.mCallback = OnHumanDetect;
            Buddy.Behaviour.SetMood(Mood.HAPPY, 2F);
            Debug.LogWarning("We found a human!!");
            Buddy.Vocal.Say("Ha te voila! Je t'aicoute");
            OnEndSearch();
        }

        private void OnEndSearch()
        {
            //Buddy.Perception.HumanDetector.OnDetect.Clear();
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
            if (mThermal)
                mThermalDetector.mCallback = OnHumanDetect;

            Buddy.Actuators.Head.Yes.SetPosition(9F);
            Debug.LogWarning("motion end ");
            // We can disable Yolo for now

            Buddy.Vocal.Listen(EndListening, SpeechRecognitionMode.GRAMMAR_ONLY);
        }

        private void EndListening(SpeechInput iObj)
        {
            if (!iObj.IsInterrupted) {
                Buddy.Sensors.Microphones.EnableSoundLocalization = false;
                Buddy.Sensors.Microphones.EnableBeamforming = false;
                Buddy.Sensors.Microphones.EnableEchoCancellation = true;
                Buddy.Vocal.Say("Je te raiponds aprais t'avoir aicoutai", EndSpeaking);
            }
        }

        private void EndSpeaking(SpeechOutput iObj)
        {
            if (!iObj.IsInterrupted) {
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);
                Buddy.Sensors.Microphones.EnableEchoCancellation = false;
                Buddy.Sensors.Microphones.EnableSoundLocalization = true;
                Buddy.Sensors.Microphones.EnableBeamforming = true;
                Debug.LogWarning("BeamForming Anabled ");

                if (mYolo)
                    Buddy.Actuators.Head.Yes.SetPosition(5F);
                else
                    Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
            }
        }

        private void Update()
        {
            if (mAverageAmbiant.Count == 30) {
                mLastAverageAmbiant = (int)mAverageAmbiant.Average();
                Debug.LogWarning("last ambiant average" + mLastAverageAmbiant);

                // Remove values to update every 30 seconds
                mAverageAmbiant.Clear();

                // Change soundloc threshold with ambiant: 55 -> 40, 35 -> 30, 75 -> 55
                //lThresh = (mLastAverageAmbiant - 60) / 2 + 40;
                Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
                Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, 10 + mLastAverageAmbiant / 2);
                Debug.LogWarning("New SoundLoc threshold: " + (10 + mLastAverageAmbiant / 2));
            } else if (Time.time - mLastTime > 1F) {
                mLastTime = Time.time;
                mAverageAmbiant.Add(Buddy.Sensors.Microphones.AmbiantSound);
                //Debug.LogWarning("last ambiant " + Buddy.Sensors.Microphones.AmbiantSound);                
            }


            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                if (mSoundLoc != Buddy.Sensors.Microphones.SoundLocalization) {
                    Debug.LogWarning("New sound loc " + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString());
                    mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
                }
                mLastSoundLocTime = Time.time;
            }
        }
    }
}