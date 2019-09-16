using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using BlueQuark;

namespace BuddyApp.OutOfBoxV3
{
    public class Detect : AStateMachineBehaviour
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

        //Takephoto variable test
        private int mPhotoTakenCount;
        private Sprite mPhotoSprite;

        public override void Start()
        {
            base.Start();

            mBehaviour = GetComponent<OutOfBoxV3Behaviour>();
            mBehaviour.PhaseDropDown.onValueChanged.AddListener((iInput) => Trigger("TRANSITION"));
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OutOfBoxUtilsVThree.DebugColor("START DETECT STATE ", "blue");
            Buddy.Vocal.SayKey("whoisaround");
            mPhotoTakenCount = 0;
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
            if (mTotalAngle < 359F/* && mTotalAngle >= 0F*/)
            {
                mTotalAngle += Math.Abs(OutOfBoxUtilsVThree.WrapAngle(Buddy.Actuators.Wheels.Angle - mAngleSequence));
                mAngleSequence = Buddy.Actuators.Wheels.Angle;
                //OutOfBoxUtils.DebugColor("TOTAL ANGLE : " + mTotalAngle, "blue");
            }
            else
            {
                Buddy.Actuators.Wheels.Stop();
                Buddy.Navigation.Stop();
                Buddy.Actuators.Head.No.ResetPosition();
                OutOfBoxUtilsVThree.DebugColor("PHASE DONE ", "blue");
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
                    if ((mTotalAngle + mRotation) > 359F)
                    {
                        mRotation = 359F - mTotalAngle;
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
            if (!mFirstStep)
            {
                mFirstStep = true;
                if (Buddy.Sensors.RGBCamera.Width > 0)
                    Buddy.Sensors.RGBCamera.TakePhotograph(TakePhoto, false, true);
                StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() =>
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
                    if (Buddy.Sensors.RGBCamera.Width > 0)
                        Buddy.Sensors.RGBCamera.TakePhotograph(TakePhoto, false, true);
                    Buddy.Vocal.SayKey("humandetected");
                    StartCoroutine(OutOfBoxUtilsVThree.PlayBIAsync(() =>
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
            OutOfBoxUtilsVThree.DebugColor("Human detected", "blue");
            return true;
        }
        
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

        private void TakePhoto(Photograph iMyPhoto)
        {
            mPhotoSprite = iMyPhoto.Image;
            iMyPhoto.Save();
        }
    }

}

                        //if (Buddy.Sensors.RGBCamera.Width > 0)
                        //{
                        //    mPictureSound.Play();
                        //    Buddy.Sensors.RGBCamera.TakePhotograph(OnFinish, false, true);
                        //    mPhotoTaken = true; 
                        //}