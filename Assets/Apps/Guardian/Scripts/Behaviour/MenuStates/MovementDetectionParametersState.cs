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
    public sealed class MovementDetectionParametersState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private TSliderToggle mSliderToggle;
        private TButton mButton;
        private TToggle mToggleKidnapping;


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
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("motiondetection"));
            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("setupmovement"));
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mSliderToggle = iBuilder.CreateWidget<TSliderToggle>();
                mSliderToggle.OnSlide.Add((iVal) => Debug.Log(iVal));
                mSliderToggle.ToggleValue = GuardianData.Instance.MovementDetection;
                mSliderToggle.SlidingValue = GuardianData.Instance.MovementDetectionThreshold;
                mSliderToggle.OnToggle.Add(OnToggleKidnapping);
                //iBuilder.CreateWidget<TToggle>();
                mButton = iBuilder.CreateWidget<TButton>();
                mButton.SetLabel(Buddy.Resources.GetString("setmotionsensitivity"));
                mButton.SetActive(mSliderToggle.ToggleValue);
                mButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cog"));
                mButton.OnClick.Add(() => { SaveParam(); Trigger("Test"); Buddy.GUI.Toaster.Hide(); });

                mToggleKidnapping = iBuilder.CreateWidget<TToggle>();
                mToggleKidnapping.OnToggle.Add((iVal) => Debug.Log(iVal));
                mToggleKidnapping.SetLabel(Buddy.Resources.GetString("kidnappingdetection"));
                mToggleKidnapping.ToggleValue = GuardianData.Instance.KidnappingDetection;
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
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
            GuardianData.Instance.MovementDetection = mSliderToggle.ToggleValue;
            GuardianData.Instance.MovementDetectionThreshold = (int)mSliderToggle.SlidingValue;
            GuardianData.Instance.KidnappingDetection = mToggleKidnapping.ToggleValue;
        }

        private void OnToggleKidnapping(bool iValue)
        {
            mButton.SetActive(iValue);
        }

    }
}