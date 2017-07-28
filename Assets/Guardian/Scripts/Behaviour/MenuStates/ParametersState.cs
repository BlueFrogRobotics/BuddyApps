using UnityEngine.UI;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the detection sensibility, test them and set the head orientation
    /// </summary>
    public class ParametersState : AStateMachineBehaviour
    {
        private DetectionLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        public override void Start()
        {
            mDetectionLayout = new DetectionLayout();
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            GuardianData.Instance.HeadOrientation = false;
            GuardianData.Instance.MovementDebug = false;
            GuardianData.Instance.SoundDebug = false;
            GuardianData.Instance.FireDebug = false;

            mDetectionLayout = new DetectionLayout();
            mHasSwitchState = false;
            BYOS.Instance.Toaster.Display<ParameterToast>().With(mDetectionLayout, 
                () => { Trigger("NextStep"); },
                () => { Trigger("Back"); });
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            if (GuardianData.Instance.HeadOrientation && !mHasSwitchState)
            {
                Toaster.Display<BackgroundToast>().With();
                mHasSwitchState = true;
                iAnimator.SetInteger("DebugMode", 0);
            }

            if (GuardianData.Instance.MovementDebug && !mHasSwitchState)
            {
                Toaster.Display<BackgroundToast>().With();
                mHasSwitchState = true;
                iAnimator.SetInteger("DebugMode", 1);
            }

            if (GuardianData.Instance.SoundDebug && !mHasSwitchState)
            {
                Toaster.Display<BackgroundToast>().With();
                mHasSwitchState = true;
                iAnimator.SetInteger("DebugMode", 2);
            }

            if (GuardianData.Instance.FireDebug && !mHasSwitchState)
            {
                Toaster.Display<BackgroundToast>().With();
                mHasSwitchState = true;
                iAnimator.SetInteger("DebugMode", 3);
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            SetThreshold();
            Detection.NoiseStimulus.Enable();
        }

        private void SetThreshold()
        {
            Detection.NoiseStimulus.Threshold= (100.0f - GuardianData.Instance.SoundDetectionThreshold)/100.0f * 0.3f;
        }
    }
}