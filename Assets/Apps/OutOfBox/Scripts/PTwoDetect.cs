using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using BlueQuark;

namespace BuddyApp.OutOfBox
{
    public class PTwoDetect : AStateMachineBehaviour
    {
        private bool mDetectEnabled;
        private bool mHumanDetectEnabled;
        private bool mHumanDetected;
        private float mAngleSequence = 0F;
        private float mTotalAngle = 0F;
        private bool mFirstStep;
        private float mTimer;
        private int mNumberOfDetect;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OutOfBoxUtils.DebugColor("SECOND PHASE : ", "blue");
            mNumberOfDetect = 0;
            mHumanDetectEnabled = false;
            mFirstStep = false;
            mTimer = 0F;
            HeadPositionDetect();
            mDetectEnabled = false;
            mHumanDetected = false;
            Buddy.Perception.HumanDetector.OnDetect.AddP(OnHumanDetect,
            new HumanDetectorParameter { SensorMode = SensorMode.VISION });
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTimer += Time.deltaTime;
            if (mTimer > 2.5F && !mFirstStep)
            {
                mFirstStep = true;
                Buddy.Actuators.Head.No.SetPosition(-90F, 45F, (iOut) => { StartDetect(); });
            }

            //add angle to the global angle to know if it > 360°
            if (mTotalAngle < 360F && mTotalAngle >= 0F)
            {
                mTotalAngle += Math.Abs(OutOfBoxUtils.WrapAngle(Buddy.Actuators.Wheels.Angle - mAngleSequence));
                mAngleSequence = Buddy.Actuators.Wheels.Angle;
                OutOfBoxUtils.DebugColor("TOTAL ANGLE : " + mTotalAngle, "blue");
            }
            else
            {
                mHumanDetectEnabled = false;
                mDetectEnabled = false;
                Buddy.Actuators.Wheels.Stop();
                Buddy.Navigation.Stop();
                StopAllCoroutines();
                Buddy.Actuators.Head.No.ResetPosition();
                mTotalAngle = -1000F;
                if (mNumberOfDetect > 0)
                    Buddy.Vocal.Say("phasetwoend", (iOut) => { Trigger("SoundLoc"); });
                else if (mNumberOfDetect == 0)
                    Buddy.Vocal.Say("phasetwonodetection", (iOut) => { Trigger("SoundLoc"); });
            }
        }

        private void HeadPositionDetect()
        {
            if (Buddy.Actuators.Head.No.Angle != 0F)
                Buddy.Actuators.Head.No.ResetPosition();
            if(Buddy.Actuators.Head.Yes.Angle != 5F)
                Buddy.Actuators.Head.Yes.SetPosition(5F, 45F);
        }

        private void StartDetect(float iAngle = 0F)
        {
            if (mDetectEnabled)
                return;

            if (mHumanDetected)
            {
                mHumanDetected = false;
                float lRotation = 30F;
                Buddy.Actuators.Head.No.SetPosition(iAngle, 45F);
                if ((mTotalAngle + lRotation) > 360F)
                {
                    lRotation = 360 - mTotalAngle;
                    
                }
                StartCoroutine(OutOfBoxUtils.WaitTimeAsync(0.100F, () =>
                {
                    Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lRotation, 70F, () =>
                    {
                        mTotalAngle -= Mathf.Abs(lRotation);
                        StartCoroutine(OutOfBoxUtils.WaitTimeAsync(0.150F, () => { Buddy.Actuators.Wheels.SetVelocities(0F, 70F); }));
                        mDetectEnabled = true;
                        mHumanDetectEnabled = true;
                    });
                }));
            }
            else
            {
                StartCoroutine(OutOfBoxUtils.WaitTimeAsync(0.150F, () =>
                {
                    //Robot turn with a velocity but won't stop before doing 360°
                    Buddy.Actuators.Wheels.SetVelocities(0F, 70F);
                    mDetectEnabled = true;
                    mHumanDetectEnabled = true;
                }));
            }
        }

        private bool OnHumanDetect(HumanEntity[] iHumanEntity)
        {
            if(!mFirstStep)
            {
                mFirstStep = true;
                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                {
                    Buddy.Actuators.Head.No.SetPosition(-90F, 45F, (iOut) => { StartDetect(); });
                     
                }));
                return true;
            }

            if (!mHumanDetectEnabled)
                return true;

            double lMaxArea = iHumanEntity.Max(h => h.BoundingBox.area());
            //Detect the human in front of Buddy, to avoid problem when people walks behind the human in front of Buddy.
            HumanEntity lBiggestHuman = iHumanEntity.First(h => h.BoundingBox.area() == lMaxArea);
            // Value between -0.5 & 0.5, that represent target's position, (abscissa) were 0.5 is the center on the image.
            float lHorizontalRatio = (float)(lBiggestHuman.Center.x / Buddy.Sensors.RGBCamera.Width) - 0.5F;
            if (lHorizontalRatio > -0.2F && lHorizontalRatio < 0.2F)
            {
                mNumberOfDetect++;
                PauseDetect();
                //store the value of the angle No to move the body to it
                float lHeadLastAngle = Buddy.Actuators.Head.No.Angle;
                // Alignement with detected human
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lHeadLastAngle, 70F, () => {
                    StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                    {
                        if (lHeadLastAngle > 0)
                        {
                            mTotalAngle -= lHeadLastAngle;
                        }
                        else
                        {
                            mTotalAngle += Mathf.Abs(lHeadLastAngle);
                        }
                        mHumanDetected = true;
                        StartDetect(-80F);
                    }));
                });
                StartCoroutine(OutOfBoxUtils.WaitTimeAsync(0.1F, () => Buddy.Actuators.Head.No.ResetPosition()));
            }
            OutOfBoxUtils.DebugColor("Human detected", "blue");
            return true;
        }

        private void EndPhaseDetect()
        {
            Buddy.Vocal.Say("phasetwoend");
            OutOfBoxData.Instance.Phase = OutOfBoxData.PhaseId.PhaseThree;
            Trigger("Base");
        }

        private void PauseDetect()
        {
            Buddy.Actuators.Wheels.Stop();
            mHumanDetectEnabled = false;
            mDetectEnabled = false;
        }

    }

}
