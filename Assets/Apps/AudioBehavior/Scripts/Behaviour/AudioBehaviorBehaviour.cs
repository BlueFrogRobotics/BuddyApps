using BlueQuark;

using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using OpenCVUnity;
using UnityEngine.UI;

namespace BuddyApp.AudioBehavior
{
    /* A basic monobehaviour as "AI" behaviour for your app */
    public class AudioBehaviorBehaviour : MonoBehaviour
    {
        private bool mHumanTracking;
        private bool mNewTrigger;
        private bool mGoTowardHuman;
        private bool mYolo;

        private int mSoundLoc;
        private int mLastAverageAmbiant;
        private int mRotation;
        private int mMotion;

        private const float ROTATION_VELOCITY = 15F;

        private float mLastTime;
        private float mTimeHumanDetected;
        private float mAngleLastDetect;
        private float mLastSoundLocTime;
        private float mAngleAtTrigger;
        private float mTimeTrigger;
        private float mTargetOdom;

        private List<int> mAverageAmbiant;
        private bool mVideoMode;
        private Texture2D mCamView;
        private List<OpenCVUnity.Rect> mDetectedBox;

        [SerializeField]
        private Button HideButtons;

        [SerializeField]
        private Button Yolo;

        [SerializeField]
        private Button Display;


        [SerializeField]
        private Button Tracking;

        [SerializeField]
        private Text Latence;


        [SerializeField]
        private InputField Heigh;


        [SerializeField]
        private InputField ImageSize;


        [SerializeField]
        private InputField DownSample;


        [SerializeField]
        private InputField ConfidenceThreshold;

        private float mLastHumanDetectTime;

        public void ToggleYolo()
        {
            mYolo = !mYolo;

            if (mYolo) {
                Buddy.Actuators.Head.Yes.SetPosition(3F);
                // Creation & Settings of parameters that will be used in detection

                Debug.LogWarning("Test yolo 1");
                Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanDetect,
                    new HumanDetectorParameter {
                        HumanDetectionMode = HumanDetectionMode.VISION,
                        YOLO = new YOLOParameter() {
                            RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, int.Parse(Heigh.text)),
                            UseThermal = false,
                            ThermalThreshold = 30.0F,
                            DetectFallenHuman = false,
                            ImageSize = int.Parse(ImageSize.text),
                            DownSample =  int.Parse(DownSample.text),
                            ConfidenceThreshold = int.Parse(ConfidenceThreshold.text)
                        }
                    }
                );
                Debug.LogWarning("test yolo2");
            } else {
                Buddy.Perception.HumanDetector.OnDetect.Clear();
                Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
            }
        }

        public void ToggleDisplay()
        {
            mVideoMode = !mVideoMode;
        }

        public void ToggleHumanTracking()
        {
            mHumanTracking = !mHumanTracking;
            if (!mYolo)
                ToggleYolo();
        }

        void Start()
        {
            Heigh.text = "" + 240;
            ImageSize.text = "" + 32;
            DownSample.text = "" + 1;
            ConfidenceThreshold.text = "" + 70;


            HideButtons.onClick.AddListener(() => Yolo.gameObject.SetActive(!Yolo.gameObject.activeSelf));
            HideButtons.onClick.AddListener(() => Display.gameObject.SetActive(!Display.gameObject.activeSelf));
            HideButtons.onClick.AddListener(() => Tracking.gameObject.SetActive(!Tracking.gameObject.activeSelf));

            AudioBehaviorActivity.Init(null);

            mNewTrigger = false;
            mHumanTracking = false;
            mGoTowardHuman = false;

            Buddy.Vocal.EnableTrigger = true;
            Buddy.Sensors.Microphones.EnableEchoCancellation = false;
            Buddy.Sensors.Microphones.EnableSoundLocalization = true;

            mAverageAmbiant = new List<int>();
            mDetectedBox = new List<OpenCVUnity.Rect> { };

            Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
                SoundLocalizationParameters.DEFAULT_RESOLUTION,
                40);

            Debug.LogWarning("Test 1");

            Buddy.Vocal.OnCompleteTrigger.Clear();
            Buddy.Vocal.OnCompleteTrigger.Add(BuddyTrigged);



            Buddy.Actuators.Head.No.ResetPosition();

            Debug.LogWarning("Test 2");
            ToggleYolo();

            Debug.LogWarning("Test 3");
            mCamView = new Texture2D(Buddy.Sensors.RGBCamera.Width, Buddy.Sensors.RGBCamera.Height);
            // Setting of the callback to use camera data
            Buddy.Sensors.RGBCamera.OnNewFrame.Add((iInput) => OnFrameCaptured(iInput));
        }

        /*
       *   On a human detection this function is called.
       */
        private void OnHumanDetect(HumanEntity[] iHumans)
        {
            Latence.text = "Latence : " + (Time.time - mLastHumanDetectTime);
            mLastHumanDetectTime = Time.time;
            Debug.LogWarning("Detection since previous one: " + (Time.time - mTimeHumanDetected));
            mTimeHumanDetected = Time.time;
            mAngleLastDetect = Buddy.Actuators.Wheels.Odometry.AngleDeg();

            if (mGoTowardHuman || mHumanTracking) {
                double lCentered;
                if (mYolo)
                    lCentered = iHumans[0].Center.x / Buddy.Sensors.RGBCamera.Width;
                else
                    lCentered = iHumans[0].Center.x;
                if (lCentered < 0.6F && lCentered > 0.4F) {
                    Debug.LogWarning("YOLO said human is centered!");
                    mGoTowardHuman = false;
                    Buddy.Actuators.Wheels.Stop();
                    mMotion = 0;
                    // otherwise, try to put the human in the center
                } else if (lCentered < 0.5F && mMotion != 1) {
                    Debug.LogWarning("turn 45 human is at: " + lCentered);
                    mMotion = 1;
                    Buddy.Actuators.Wheels.SetVelocities(0F, ROTATION_VELOCITY);
                } else if (lCentered > 0.5 && mMotion != 2) {
                    Debug.LogWarning("turn -45 human is at: " + lCentered);
                    mMotion = 2;
                    Buddy.Actuators.Wheels.SetVelocities(0F, -ROTATION_VELOCITY);
                }

                // If we find human before sound loc destination, stop navigation and center on human
            } else if (Math.Abs(mTargetOdom - mAngleLastDetect) < 20 && mNewTrigger && Buddy.Navigation.IsBusy) {
                Buddy.Navigation.Stop();
                mGoTowardHuman = true;
            }


            // Display            
            // Clear all old box, from the last detection
            mDetectedBox.Clear();
            // We add each box to a list, to display them later in OnNewFrame
            foreach (HumanEntity lHuman in iHumans)
                mDetectedBox.Add(new OpenCVUnity.Rect(lHuman.BoundingBox.tl(), lHuman.BoundingBox.br()));

        }

        private void BuddyTrigged(SpeechHotword iHotWord)
        {
            mNewTrigger = true;

            // Reset listen + callback of search
            Debug.LogWarning("Buddy Triggered time " + Time.time);

            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);
            Buddy.Vocal.Stop();
            Buddy.Actuators.Wheels.Stop();

            mAngleAtTrigger = Buddy.Actuators.Wheels.Odometry.AngleDeg();
            mTimeTrigger = Time.time;

            Debug.LogWarning("my angle " + mSoundLoc + " my time" + (Time.time - mLastSoundLocTime));
            Debug.LogWarning("os angle " + iHotWord.SoundLocalization);
            Debug.LogWarning("reco score " + iHotWord.RecognitionScore);
            Debug.LogWarning("angle at trigger " + mAngleAtTrigger);
            Debug.LogWarning("time " + DateTime.Now.ToString());
            Debug.LogWarning("Last human time!!!! " + (Time.time - mTimeHumanDetected));

            // Check if human is already present, then just stay in front.
            if (Time.time - mTimeHumanDetected < 0.8F /*) && (Math.Abs(iHotWord.SoundLocalization) < 60 || iHotWord.SoundLocalization == Microphones.NO_SOUND_LOCALIZATION /* TODO NEED FIX TOF || Buddy.Sensors.TimeOfFlightSensors.Back.FilteredValue < 1300F)*/) {
                mGoTowardHuman = true;

                Debug.LogWarning("Human already in front!!!! " + (Time.time - mTimeHumanDetected));
                Buddy.Vocal.Say("Je t'aicoute "/*temps " + lTime*/);
                OnEndSearch();

                // Check if soundloc ok
            } else if (iHotWord.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                mGoTowardHuman = false;
                Buddy.Actuators.Wheels.Stop();
                Debug.LogWarning("Sound loc ok, motion to angle : " + iHotWord.SoundLocalization);
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(iHotWord.SoundLocalization, ROTATION_VELOCITY * 2, OnEndMoveSoundLoc);
                mRotation = iHotWord.SoundLocalization;
                mTargetOdom = iHotWord.SoundLocalization + mAngleAtTrigger;
                Debug.LogWarning("TargetOdom (pre 360): " + mTargetOdom);

                if (mTargetOdom > 180F)
                    mTargetOdom = mTargetOdom - 360F;
                else if (mTargetOdom < -180F)
                    mTargetOdom = mTargetOdom + 360F;

                Debug.LogWarning("TargetOdom: " + mTargetOdom);

                // Otherwise, look for human if yolo
            } else {
                Buddy.Behaviour.SetMood(Mood.THINKING);
                Debug.LogWarning("No source loc");
                Buddy.Vocal.Say("Pas de source loc "/*temps " + lTime*/);
                //Debug.LogWarning("No motion bcs last sound loc too old: " + Time.time + " " + mLastSoundLocTime + " = " + (Time.time - mLastSoundLocTime));

                // find human
                // TODO find with thermal?
                if (mYolo) {
                    Buddy.Actuators.Wheels.SetVelocities(0F, ROTATION_VELOCITY);
                    mGoTowardHuman = true;
                    Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound,
                            new HumanDetectorParameter {
                                HumanDetectionMode = HumanDetectionMode.VISION,
                                YOLO = new YOLOParameter() {
                                    RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, int.Parse(Heigh.text)),
                                    UseThermal = false,
                                    ThermalThreshold = 30.0F,
                                    DetectFallenHuman = false,
                                    ImageSize = int.Parse(ImageSize.text),
                                    DownSample = int.Parse(DownSample.text),
                                    ConfidenceThreshold = int.Parse(ConfidenceThreshold.text)
                                }
                            }
                    );
                } else {
                    //Buddy.Vocal.Say("Mais je t'écoute");
                    OnEndSearch();
                }

            }

        }

        private void OnEndMoveSoundLoc()
        {
            Debug.LogWarning("End motion of soundloc ");
            mGoTowardHuman = true;
            if (Time.time - mTimeHumanDetected < 0.2F || !mYolo) {
                Debug.LogWarning("End motion of soundloc and human seen ");
                OnEndSearch();
                // Go further
            } else {
                Buddy.Behaviour.Face.SetFacialExpression(FacialExpression.THINKING);
                Debug.LogWarning("End motion of soundloc and ...");

                // A human was detected after human trigger
                if (mTimeHumanDetected > mTimeTrigger) {


                    //Debug.LogWarning("human seen during sourceloc rotation, rotate to  " + (-Math.Sign(mRotation)) *
                    //    (Math.Abs(mRotation) - (Math.Abs(Buddy.Actuators.Wheels.Odometry.AngleDeg() - mAngleLastDetect) % 360)));

                    Debug.LogWarning("human seen during rotate of sourceloc, rotate to " + mAngleLastDetect);
                    Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleLastDetect, 80F, (Action<Vector3>)OnEndSearch);
                } else {
                    if (mYolo) {
                        // Keep turning the same way
                        Debug.LogWarning("no human after sourceloc rotate, try to find with yolo, turn " + (-Math.Sign(mRotation)) * ROTATION_VELOCITY);
                        // Wait for stop navigation strategy before rotation
                        //StartCoroutine("DelayedRotate");
                        // Fixed? :
                        Buddy.Actuators.Wheels.SetVelocities(0F, (Math.Sign(mRotation)) * ROTATION_VELOCITY);

                        Buddy.Perception.HumanDetector.OnDetect.Add(OnHumanFound, new HumanDetectorParameter {
                            HumanDetectionMode = HumanDetectionMode.VISION,
                            YOLO = new YOLOParameter() {
                                RegionOfInterest = new OpenCVUnity.Rect(0, 0, 320, int.Parse(Heigh.text)),
                                UseThermal = false,
                                ThermalThreshold = 30.0F,
                                DetectFallenHuman = false,
                                ImageSize = int.Parse(ImageSize.text),
                                DownSample = int.Parse(DownSample.text),
                                ConfidenceThreshold = int.Parse(ConfidenceThreshold.text)
                            }
                        }
                        );
                        //} else if (mThermal) {
                        //    // Keep turning the same way
                        //    Buddy.Actuators.Wheels.SetVelocities(0F, (-mRotation / Math.Abs(mRotation)) * 45F);
                        //    mThermalDetector.mCallback = OnHumanFound;
                    } else {
                        // Go back to initial position
                        Debug.LogWarning("no human seen, rotate to " + (Math.Sign(mRotation)) * (360 - Math.Abs(mRotation)));
                        Buddy.Navigation.Run<DisplacementStrategy>().Rotate(Math.Sign(mRotation) * (360 - Math.Abs(mRotation)), 80F, OnEndMoveNoHuman);
                    }
                }
            }

        }

        //private IEnumerator DelayedRotate()
        //{
        //    yield return new WaitForSeconds(0.1F);
        //    Buddy.Actuators.Wheels.SetVelocities(0F, (Math.Sign(mRotation)) * ROTATION_VELOCITY);
        //}

        private void OnEndSearch(Vector3 obj)
        {
            Buddy.Behaviour.SetMood(Mood.NEUTRAL);
            OnEndSearch();
        }

        private void OnEndMoveNoHuman()
        {
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

            Debug.LogWarning("We did not find a human!!");
            float lTime = Time.time - mTimeTrigger;
            Debug.LogWarning("No motion temps " + lTime);
            Buddy.Vocal.Say("Je ne t'ai pas trouvai ");

            Buddy.Behaviour.SetMood(Mood.NEUTRAL);

            Buddy.Navigation.Run<DisplacementStrategy>().RotateTo(mAngleAtTrigger, 80F, (Action<Vector3>)OnEndSearch);
        }

        private void OnHumanFound(HumanEntity[] obj)
        {
            if (mNewTrigger) {
                Buddy.Actuators.Wheels.ImmediateStop();
                mNewTrigger = false;
                Debug.LogWarning("On Human found nb callbacks " + Buddy.Perception.HumanDetector.OnDetect.Count);
                Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

                Debug.LogWarning("On Human found post nb callbacks " + Buddy.Perception.HumanDetector.OnDetect.Count);

                float lTime = Time.time - mTimeTrigger;
                Debug.LogWarning("Ha te voila! " + lTime);
                Buddy.Vocal.Say("Ha te voila! "/*temps " + lTime*/);
                //if (mThermal)
                //    mThermalDetector.mCallback = OnHumanDetect;
                Buddy.Behaviour.SetMood(Mood.HAPPY, 2F);
                Debug.LogWarning("We found a human!!");
                OnEndSearch();
            }
        }

        private void OnEndSearch()
        {
            Debug.LogWarning("End of search!! no more motion");
            mNewTrigger = false;
            mGoTowardHuman = false;
            Buddy.Actuators.Wheels.Stop();
            Buddy.Perception.HumanDetector.OnDetect.Remove(OnHumanFound);

            Buddy.Actuators.Head.Yes.SetPosition(9F);

            //Buddy.Vocal.Listen(EndListening, SpeechRecognitionMode.GRAMMAR_ONLY);
            EndListening();
        }

        private void EndListening(/*SpeechInput iObj*/)
        {
            // Just to be sure
            Buddy.Actuators.Wheels.Stop();

            if (/*!iObj.IsInterrupted*/ true) {
                float lTime = Time.time - mTimeTrigger;
                Buddy.Vocal.Say("J'ai fini ma recherche "/*temps " + lTime*/, EndSpeaking);
            }
        }

        private void EndSpeaking(SpeechOutput iObj)
        {
            if (!iObj.IsInterrupted) {
                // Just to be sure
                Buddy.Actuators.Wheels.Stop();
                Buddy.Behaviour.SetMood(Mood.NEUTRAL);

                if (mYolo)
                    Buddy.Actuators.Head.Yes.SetPosition(3F);
                else
                    Buddy.Actuators.Head.Yes.SetPosition(-9.9F);
            }
        }

        // On each frame captured by the camera this function is called, with the matrix of pixel.
        private void OnFrameCaptured(RGBCameraFrame iInput)
        {
            // Always clone the input matrix, this avoid to working with the original matrix, when the C++ part wants to modify it.
            Mat lMatSrc = iInput.Mat.clone();

            // Drawing each box were detect something, on the frame.           
            foreach (OpenCVUnity.Rect lBox in mDetectedBox)
                Imgproc.rectangle(lMatSrc, lBox.tl(), lBox.br(), new Scalar(new Color(255, 0, 0)), 3);

            // Flip to avoid mirror effect.
            Core.flip(lMatSrc, lMatSrc, 1);
            // Use matrice format, to scale the texture.
            mCamView = Utils.ScaleTexture2DFromMat(lMatSrc, mCamView);
            // Use matrice to fill the texture.
            Utils.MatToTexture2D(lMatSrc, mCamView);
        }

        private void Update()
        {

            // Reset real time counter if OnHumanDetect is not call since RESET_COUNTER_TIMER seconds.
            if ((Time.time - mTimeHumanDetected) >= 0.2F) {
                // Clear all old box, from the last detection
                mDetectedBox.Clear();
            }

            // Video Mode: Display the camera view with a visual of detection.
            if (mVideoMode) {
                if (!Buddy.GUI.Toaster.IsBusy)
                    Buddy.GUI.Toaster.Display<VideoStreamToast>().With(mCamView, () => { Buddy.GUI.Toaster.Hide(); mVideoMode = false; });
            } else if (Buddy.GUI.Toaster.IsBusy)
                // If the video mode is disable: Buddy's face is display.
                Buddy.GUI.Toaster.Hide();




            if (mAverageAmbiant.Count == 15) {
                mLastAverageAmbiant = (int)mAverageAmbiant.Average();
                Debug.LogWarning("last ambiant average" + mLastAverageAmbiant);

                // Remove values to update every 15 seconds
                mAverageAmbiant.Clear();

                // Change soundloc threshold with ambiant: 55 -> 65, 35 -> 10, 75 -> 85

                int lThresh = 10;
                //if (mLastAverageAmbiant < 62)
                //	lThresh = mLastAverageAmbiant - 10;
                //            else if (mLastAverageAmbiant < 70)
                //                lThresh = mLastAverageAmbiant;
                //            else
                //                lThresh = mLastAverageAmbiant + 20;


                Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
                    SoundLocalizationParameters.DEFAULT_RESOLUTION, lThresh);

                //int lThresh = (int)Math.Pow(mLastAverageAmbiant, 3) / 3000;
                //Buddy.Sensors.Microphones.SoundLocalizationParameters = new SoundLocalizationParameters(
                //Buddy.Sensors.Microphones.SoundLocalizationParameters.Resolution, lThresh);
                Debug.LogWarning("New SoundLoc threshold: " + lThresh);
            } else if (Time.time - mLastTime > 1F) {
                mLastTime = Time.time;
                mAverageAmbiant.Add(Buddy.Sensors.Microphones.AmbientSound);
                //Debug.LogWarning("last ambiant " + Buddy.Sensors.Microphones.AmbiantSound);                
            }


            if (Buddy.Sensors.Microphones.SoundLocalization != Microphones.NO_SOUND_LOCALIZATION) {
                if (mSoundLoc != Buddy.Sensors.Microphones.SoundLocalization) {
                    Debug.LogWarning("New sound loc " + Buddy.Sensors.Microphones.SoundLocalization + "  " + DateTime.Now.ToString());
                    mSoundLoc = Buddy.Sensors.Microphones.SoundLocalization;
                    mLastSoundLocTime = Time.time;
                }
            }
        }
    }
}
