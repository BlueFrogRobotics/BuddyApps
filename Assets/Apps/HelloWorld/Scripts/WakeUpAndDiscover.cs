using BlueQuark;

using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace BuddyApp.HelloWorld
{
    public sealed class WakeUpAndDiscover : AStateMachineBehaviour
    {
        private bool mHumanDetectEnabled;

        private bool mDiscoveringEnabled;

        private float mTotalRotationAbsolute;

        private float mLastAngularPosition;

        private float mHeadNoAngle;

        private int mHuman;

        private float mHalfFov;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Init
            mHuman = 0;
            mHeadNoAngle = -80F;
            mDiscoveringEnabled = false;
            mHumanDetectEnabled = false;
            mLastAngularPosition = Buddy.Actuators.Wheels.Angle;
            mTotalRotationAbsolute = 0F;
            mHalfFov = 30F;

            // Creation & Settings of parameters that will be used in detection
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
                new HumanDetectorParameter { HumanDetectionMode = HumanDetectionMode.VISION });

            // Head to the ground
            Debug.LogError("-- ASK HEAD NO RESET");
            Buddy.Actuators.Head.No.ResetPosition();
            Buddy.Actuators.Head.Yes.SetPosition(-9F, HelloWorldUtils.YES_VELOCITY, (iPos) =>
            {
                // Asleep
                Buddy.Behaviour.Face.PlayEvent(FacialEvent.FALL_ASLEEP, false);

                StartCoroutine(HelloWorldUtils.WaitTimeAsync(2F, () =>
                // OnEndWaiting
                {
                    // Lifting head
                    Buddy.Actuators.Head.Yes.SetPosition(5F, HelloWorldUtils.YES_VELOCITY);

                    Buddy.Behaviour.Face.PlayEvent(FacialEvent.AWAKE, null, (iFacialEvent) =>
                    // OnEndFacialEvent
                    {
                        Buddy.Vocal.Say(Buddy.Resources.GetString("hello"), (iSpeechOutput) =>
                        {
                            // Play BI here
                            StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                            // OnEndBI
                            {
                                // Run discovering after speech
                                Buddy.Vocal.Say(Buddy.Resources.GetString("seearound"), (iOut) => { StartDiscovering(mHeadNoAngle, HelloWorldUtils.NO_VELOCITY); });
                            }));
                        });
                    });
                }));
            });
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!mDiscoveringEnabled)
                return;

            if (mTotalRotationAbsolute >= 0) {
                // Update odometry info
                mTotalRotationAbsolute += Math.Abs(HelloWorldUtils.WrapAngle(Buddy.Actuators.Wheels.Angle - mLastAngularPosition));
                mLastAngularPosition = Buddy.Actuators.Wheels.Angle;

                Debug.LogWarning("TOTAL ANGLE:" + mTotalRotationAbsolute);

                // Check if the rotation is finished
                if (mTotalRotationAbsolute > 360F && mTotalRotationAbsolute >= 0) {
                    TransitionToSoundLoc();
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            Buddy.Perception.HumanDetector.OnDetect.RemoveP(OnHumanDetect);
        }

        private void TransitionToSoundLoc()
        {
            mDiscoveringEnabled = false;
            mHumanDetectEnabled = false;
            Buddy.Actuators.Wheels.Stop();
            Buddy.Navigation.Stop();
            StopAllCoroutines();
            Buddy.Actuators.Head.No.ResetPosition();
            string lFinalSpeech = null;
            // One person
            if (mHuman > 0 && mHuman < 2)
                lFinalSpeech = Buddy.Resources.GetString("oneperson");
            // Several
            else if (mHuman > 2)
                lFinalSpeech = Buddy.Resources.GetString("surrounded");
            // Nobody
            else
                lFinalSpeech = Buddy.Resources.GetString("alone");
            lFinalSpeech += "[100]" + Buddy.Resources.GetString("soundloc");
            Buddy.Vocal.Say(lFinalSpeech, (iSpeechOutput) => { Trigger("SoundLocTrigger"); });
        }

        /*
        **  On a human detection this function is called.
        */
        private bool OnHumanDetect(HumanEntity[] iHumans)
        {
            if (!mHumanDetectEnabled && mTotalRotationAbsolute >= 0)
                return true;

            // The target is the biggest human detect. (The closest one)
            double lMaxArea = iHumans.Max(h => h.BoundingBox.area());
            // Get biggest box
            HumanEntity lBiggestHuman = iHumans.First(h => h.BoundingBox.area() == lMaxArea);
            // Value between -0.5 & 0.5, that represent target's position, (abscissa) were 0.5 is the center on the image.
            float lHorizontalRatio = (float)(lBiggestHuman.Center.x / Buddy.Sensors.RGBCamera.Width) - 0.5F;

            //when biggest human is centered do BI / repositionnement, etc..
            if (lHorizontalRatio > -0.2F && lHorizontalRatio < 0.2F) {
                if (mHuman < 100)
                    mHuman++;
                HelloWorldUtils.DebugColor("HUMAN: " + mHuman, "blue");
                PauseDiscovering();
                // Center position front of detected human - just using angle no value for now
                float lHeadLastAngle = Buddy.Actuators.Head.No.Angle;
                // Alignement with detected human
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lHeadLastAngle, HelloWorldUtils.ANGULAR_VELOCITY, () =>
                {
                    StartCoroutine(HelloWorldUtils.PlayBIAsync(() =>
                    {
                        // Update total angle performed according to alignement movement
                        if (lHeadLastAngle > 0) {
                            HelloWorldUtils.DebugColor("TOTAL ANGLE - HeadLastA:" + mTotalRotationAbsolute + " - " + lHeadLastAngle + " = " + (mTotalRotationAbsolute - lHeadLastAngle), "blue");
                            mTotalRotationAbsolute -= lHeadLastAngle;
                        }
                        else {
                            HelloWorldUtils.DebugColor("TOTAL ANGLE + HeadLastA:" + mTotalRotationAbsolute + " + " + Mathf.Abs(lHeadLastAngle) + " = " + (mTotalRotationAbsolute + Mathf.Abs(lHeadLastAngle)), "blue");
                            mTotalRotationAbsolute += Mathf.Abs(lHeadLastAngle);
                        }
                        StartDiscovering(-mHeadNoAngle, HelloWorldUtils.NO_VELOCITY, true);
                    }));
                });
                Debug.LogError("-- ASK HEAD NO RESET");
                // To avoid replacement of the hardware command, execute this with delay
                StartCoroutine(HelloWorldUtils.WaitTimeAsync(0.100F, () => { Buddy.Actuators.Head.No.ResetPosition(); }));
            }
            return true;
        }

        private void StartDiscovering(float iHeadNoPosition, float iNoVelocity, bool iHumanDetected = false)
        {
            // if already in discovering mode dont do anything
            if (mDiscoveringEnabled)
                return;

            // Check if the rotation is finished
            if (mTotalRotationAbsolute > 360F && mTotalRotationAbsolute >= 0) {
                HelloWorldUtils.DebugColor("-- 360 REACHED! --", "red");
                mTotalRotationAbsolute = -1000F;
                TransitionToSoundLoc();
                return;
            }

            if (iHumanDetected) {
                HelloWorldUtils.DebugColor("-- HUMAN WAS DETECTED ---", "red");
                
                // Have to lose human on the F.O.V
                float lRotation = mHalfFov;
                if ((mTotalRotationAbsolute + lRotation) > 360F) {
                    lRotation = 360F - mTotalRotationAbsolute;
                    HelloWorldUtils.DebugColor("DONT GO TOO FAR -- THIS IS THE LAST ROTATION: " + lRotation, "blue");
                }
                HelloWorldUtils.DebugColor("ROTATION ORDER: " + lRotation, "blue");
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lRotation, HelloWorldUtils.ANGULAR_VELOCITY, () =>
                {
                    HelloWorldUtils.DebugColor("-- ROTATION ORDER END  ---", "red");
                    HelloWorldUtils.DebugColor("TOTAL ANGLE + ROT:" + mTotalRotationAbsolute + " + " + Mathf.Abs(lRotation) + " = " + (mTotalRotationAbsolute - Mathf.Abs(lRotation)), "blue");
                    mTotalRotationAbsolute -= Mathf.Abs(lRotation);

                    // End restart the discovering (with some delay to avoid replace command issue)
                    StartCoroutine(HelloWorldUtils.WaitTimeAsync(0.150F, () => { Buddy.Actuators.Wheels.SetVelocities(0F, HelloWorldUtils.ANGULAR_VELOCITY); }));
                    mDiscoveringEnabled = true;
                    mHumanDetectEnabled = true;
                });
                Debug.LogError("-- ASK HEAD NO WITH: " + iHeadNoPosition + " " + iNoVelocity);
                StartCoroutine(HelloWorldUtils.WaitTimeAsync(0.100F, () => { Buddy.Actuators.Head.No.SetPosition(iHeadNoPosition, iNoVelocity); }));
            }
            else {
                HelloWorldUtils.DebugColor("-- HUMAN WAS NOT DETECTED ---", "red");
                Debug.LogError("-- ASK HEAD NO WITH: " + iHeadNoPosition + " " + iNoVelocity);
                Buddy.Actuators.Head.No.SetPosition(iHeadNoPosition, iNoVelocity);
                StartCoroutine(HelloWorldUtils.WaitTimeAsync(0.150F, () =>
                {
                    HelloWorldUtils.DebugColor("-- START WHEELS & DETECT ---", "red");
                    Buddy.Actuators.Wheels.SetVelocities(0F, HelloWorldUtils.ANGULAR_VELOCITY);
                    mDiscoveringEnabled = true;
                    mHumanDetectEnabled = true;
                }));
            }
            HelloWorldUtils.DebugColor("-- STARTING DISCOVERING ---", "red");
        }

        private void PauseDiscovering()
        {
            HelloWorldUtils.DebugColor("-- PAUSE DISCOVERING ---", "red");
            mHumanDetectEnabled = false;
            Buddy.Actuators.Wheels.Stop();
            mDiscoveringEnabled = false;
        }
    }
}
