using UnityEngine.UI;
using UnityEngine;
using Buddy;
using Buddy.UI;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    public class ParametersState : AStateMachineBehaviour
    {
        DetectionLayout mDetectionLayout;

        public override void Start()
        {
            mDetectionLayout = new DetectionLayout();
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            BYOS.Instance.Toaster.Display<ParameterToast>().With(mDetectionLayout, 
                () => { Trigger("NextStep"); },
                () => { Trigger("Back"); });
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }
    }
}