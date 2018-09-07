using UnityEngine.UI;
using UnityEngine;
using BlueQuark;
using System;
using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the detection sensibility, test them and set the head orientation
    /// </summary>
    public class FireDetectionParametersState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private TToggle mToggleFireDetection;
        private TButton mButtonTestSensibility;


        public override void Start()
        {
            //mDetectionLayout = new GuardianLayout();
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {


            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);

            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mToggleFireDetection = iBuilder.CreateWidget<TToggle>();
                mToggleFireDetection.SetLabel("setup fire detection");
                mToggleFireDetection.ToggleValue = GuardianData.Instance.FireDetection;

                mButtonTestSensibility = iBuilder.CreateWidget<TButton>();
                mButtonTestSensibility.SetLabel("Test Sensibility");
                mButtonTestSensibility.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cog"));
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, "Cancel",
            () => { SaveParam(); Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, "Next"
            );


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void SaveParam()
        {
            GuardianData.Instance.FireDetection = mToggleFireDetection.ToggleValue;
        }

    }
}