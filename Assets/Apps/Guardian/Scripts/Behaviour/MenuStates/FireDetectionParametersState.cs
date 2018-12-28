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
    public sealed class FireDetectionParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;
        private bool mIsTesting = false;

        private TToggle mToggleFireDetection;
        private TButton mButtonTestSensibility;


        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            // If was displaying FireDetectionState => Toggle true. Else, take FireDetection value.
            bool lBIsToggleChecked = mIsTesting || GuardianData.Instance.FireDetection;
            mIsTesting = false;

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("heatdetection")); 

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                mToggleFireDetection = iBuilder.CreateWidget<TToggle>();
                mToggleFireDetection.SetLabel(Buddy.Resources.GetString("activation"));
                mToggleFireDetection.ToggleValue = lBIsToggleChecked;
                mToggleFireDetection.OnToggle.Add(OnToggleParam);

                mButtonTestSensibility = iBuilder.CreateWidget<TButton>();
                mButtonTestSensibility.SetLabel(Buddy.Resources.GetString("viewheatdetection"));
                mButtonTestSensibility.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cog"));
                mButtonTestSensibility.SetActive(mToggleFireDetection.ToggleValue);
                mButtonTestSensibility.OnClick.Add(() => { Trigger("Test"); mIsTesting = true; Buddy.GUI.Toaster.Hide(); });
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { SaveParam(); Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("save")
            );
        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.HideTitle();
        }

        private void SaveParam()
        {
            GuardianData.Instance.FireDetection = mToggleFireDetection.ToggleValue;
        }

        private void OnToggleParam(bool iValue)
        {
            mButtonTestSensibility.SetActive(iValue);
        }

    }
}