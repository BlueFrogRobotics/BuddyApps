using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buddy;
using Buddy.UI;

namespace BuddyApp.FreezeDance
{
    public class DetectingMotionState : AStateMachineBehaviour
    {
        private float mTime;
        private bool mLost;
        public bool mSkipFrame = true;

        public override void Start()
        {
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Perception.MovementTracker.enabled = true;
            //Perception.MovementDetector.enabled = true;
            //Primitive.RGBCam.Resolution = RGBCamResolution.W_176_H_144;
            if (!Primitive.RGBCam.IsOpen)
                Primitive.RGBCam.Open(RGBCamResolution.W_176_H_144);
            Debug.Log("resolutioncam: " + Primitive.RGBCam.Resolution);
            mTime = 0.0f;
            mLost = false;
            mSkipFrame = true;
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTime += Time.deltaTime;
            //Debug.Log("detection: " + Perception.MovementDetector.IsMovementDetected);
            //if (!mLost && mTime>1.1f && Perception.MovementDetector.IsMovementDetected)
            //{
            //        Trigger("Lose");
            //        mLost = true;
            //}

            if (!mLost && mTime > 5.0f)
            {
                mLost = true;
                Toaster.Hide();
                Trigger("Detection");
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            ResetTrigger("Detection");
        }
    }
}