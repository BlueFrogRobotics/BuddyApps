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
        private float mRotation = 30F;


        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxBehaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("Base"));
        }

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
            //OutOfBoxUtils.DebugColor("ANGLE ODOM Z : " + Buddy.Actuators.Wheels.Angle, "red");
            mTimer += Time.deltaTime;
            if (mTimer > 2.5F && !mFirstStep)
            {
                mFirstStep = true;
                mTotalAngle = 0F;
                Buddy.Actuators.Head.No.SetPosition(-90F, 45F, (iOut) => { Buddy.Actuators.Head.No.SetPosition(90F, 45F, (iSpeechOut) => { StartDetect(); }); });
            }

            //add angle to the global angle to know if it > 360°
            if (mTotalAngle < 325F/* && mTotalAngle >= 0F*/)
            {
                mTotalAngle += Math.Abs(OutOfBoxUtils.WrapAngle(Buddy.Actuators.Wheels.Angle - mAngleSequence));
                mAngleSequence = Buddy.Actuators.Wheels.Angle;
                //OutOfBoxUtils.DebugColor("TOTAL ANGLE : " + mTotalAngle, "blue");
            }
            else
            {
                Buddy.Actuators.Wheels.Stop();
                Buddy.Navigation.Stop();
                Buddy.Actuators.Head.No.ResetPosition();
                OutOfBoxUtils.DebugColor("PHASE DONE ", "blue");
                mHumanDetectEnabled = false;
                mDetectEnabled = false;

                mTotalAngle = -1000F;
                if (mNumberOfDetect > 0)
                {
                    Buddy.Vocal.SayKey("phasetwoend", (iOut) => {
                        StartCoroutine(WaitTimeBeforeChangingstate());
                        if (!iOut.IsInterrupted)
                            mBehaviour.PhaseDropDown.value = 2;
                    });
                }
                else if (mNumberOfDetect == 0)
                {
                    Buddy.Vocal.SayKey("phasetwonodetection", (iOut) => {
                        StartCoroutine(WaitTimeBeforeChangingstate());
                        if (!iOut.IsInterrupted)
                            mBehaviour.PhaseDropDown.value = 2;
                    });
                    
                }               
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StopAllCoroutines();
            
        }

        //private IEnumerator ChangeState()
        //{
        //    yield return new WaitForSeconds(0.5F);
        //    if (mNumberOfDetect > 0)
        //    {
        //        mBehaviour.PhaseDropDown.value = 2;
        //        Buddy.Vocal.SayKey("phasetwoend", (iOut) => { StartCoroutine(WaitTimeBeforeChangingstate()); if (!iOut.IsInterrupted) Trigger("Base"); });

        //    }
        //    else if (mNumberOfDetect == 0)
        //    {
        //        mBehaviour.PhaseDropDown.value = 2;
        //        Buddy.Vocal.SayKey("phasetwonodetection", (iOut) => { StartCoroutine(WaitTimeBeforeChangingstate()); if (!iOut.IsInterrupted) Trigger("Base"); });

        //    }
        //}

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

                Buddy.Actuators.Head.No.SetPosition(iAngle, 45F, (iFloat) =>
                {
                    if ((mTotalAngle + mRotation) > 325F)
                    {
                        mRotation = 325F - mTotalAngle;
                    }
                    //Buddy.Navigation.Run<DisplacementStrategy>().Rotate(mRotation, 70F, () =>
                    //{
                    //    mTotalAngle -= Mathf.Abs(mRotation);
                        StartCoroutine(WaitTimeStartDetect());
                    //    mDetectEnabled = true;
                    //    mHumanDetectEnabled = true;
                    //});
                });
            }
            else
            {
                //Robot turn with a velocity but won't stop before doing 360°
                Buddy.Actuators.Wheels.SetVelocities(0F, 70F);
                mDetectEnabled = true;
                mHumanDetectEnabled = true;
                
            }
        }
        

        private bool OnHumanDetect(HumanEntity[] iHumanEntity)
        {
            if(!mFirstStep)
            {
                mFirstStep = true;
                StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                {
                    Buddy.Actuators.Head.No.SetPosition(-90F, 45F, (iOut) => { Buddy.Actuators.Head.No.SetPosition(90F, 45F, (iSpeechOut) => { StartDetect(); }); });

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
                //mNumberOfDetect++;
                PauseDetect();
                //store the value of the angle No to move the body to it
                float lHeadLastAngle = Buddy.Actuators.Head.No.Angle;
                // Alignement with detected human
                Buddy.Navigation.Run<DisplacementStrategy>().Rotate(lHeadLastAngle, 70F, () => {
                    StartCoroutine(OutOfBoxUtils.PlayBIAsync(() =>
                    {
                        //if (lHeadLastAngle > 0)
                        //{
                        //    OutOfBoxUtils.DebugColor("1 : mtotal angle : " + mTotalAngle +  " lheadlastangle : " + lHeadLastAngle + " mTotalAngle -= lHeadLastAngle : " + (mTotalAngle -= lHeadLastAngle), "red");
                        //    mTotalAngle -= lHeadLastAngle;
                        //}
                        //else
                        //{
                        //    OutOfBoxUtils.DebugColor("2 : mtotal angle : " + mTotalAngle + " lheadlastangle : " + lHeadLastAngle + " mTotalAngle += Mathf.Abs(lHeadLastAngle) : " + (mTotalAngle += Mathf.Abs(lHeadLastAngle)), "red");
                        //    mTotalAngle += Mathf.Abs(lHeadLastAngle);
                        //}
                        mHumanDetected = true;
                        StartDetect(90F);
                    }));
                });
                StartCoroutine(WaitTimeAndResetNo());
            }
            mNumberOfDetect++;
            OutOfBoxUtils.DebugColor("Human detected", "blue");
            return true;
        }

        //private void EndPhaseDetect()
        //{
        //    Buddy.Vocal.SayKey("phasetwoend");
        //    mBehaviour.PhaseDropDown.value = 2;
        //    Trigger("Base");
        //}

        private void PauseDetect()
        {
            Buddy.Actuators.Wheels.Stop();
            mHumanDetectEnabled = false;
            mDetectEnabled = false;
        }

        //Couldn't use the WaitTimeAsync from OutOfBoxutils, don't know why so we did 4 similar function, waiting for more tests about WaitTimeAsync
        //before deleting those 4 similars function
        private IEnumerator WaitTimeAndResetNo()
        {
            yield return new WaitForSeconds(0.1F);
            Buddy.Actuators.Head.No.ResetPosition();
        }

        private IEnumerator WaitTimeStartDetect()
        {
            yield return new WaitForSeconds(0.1F);
            Buddy.Navigation.Run<DisplacementStrategy>().Rotate(mRotation, 70F, () =>
            {
                //mTotalAngle -= Mathf.Abs(mRotation);
               
                StartCoroutine(WaitTime());
                mDetectEnabled = true;
                mHumanDetectEnabled = true;
            });
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(0.15F);
            Buddy.Actuators.Wheels.SetVelocities(0F, 70F);
        }

        private IEnumerator WaitTimeBeforeChangingstate()
        {
            yield return new WaitForSeconds(3F);
        }
    }

}
