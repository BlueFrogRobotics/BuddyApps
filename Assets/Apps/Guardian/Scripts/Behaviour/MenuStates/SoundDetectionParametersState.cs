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
    public sealed class SoundDetectionParametersState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private TSliderToggle mSliderToggle;
        private TButton mButton;

        private float mTimer = 0.0F;
        private bool mToasterVisible;


        public override void Start()
        {
            //mDetectionLayout = new GuardianLayout();
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mToasterVisible = false;
            mTimer = 0.0F;
            //Buddy.GUI.Toaster.Display<ParameterToast>().With(mDetectionLayout,
            //	() => { Trigger("NextStep"); }, 
            //	null);
            


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mTimer += Time.deltaTime;
            if(!mToasterVisible && mTimer > 1.0F)
            {
                ShowToaster();
                mToasterVisible = true;
            }
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.HideTitle();
        }

        private void ShowToaster()
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("noisedetection"));
            //PARAMETER OF GUARDIAN : need to wait for the discussion between Antoine Marc and Delphine 
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                //iBuilder.CreateWidget<TText>().SetLabel(Buddy.Resources.GetString("setupsound"));
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mSliderToggle = iBuilder.CreateWidget<TSliderToggle>();
                mSliderToggle.OnSlide.Add((iVal) => Debug.Log(iVal));
                mSliderToggle.ToggleValue = GuardianData.Instance.SoundDetection;
                mSliderToggle.SlidingValue = GuardianData.Instance.SoundDetectionThreshold;
                //iBuilder.CreateWidget<TToggle>();
                mButton = iBuilder.CreateWidget<TButton>();
                mButton.SetLabel(Buddy.Resources.GetString("setnoisesensitivity"));
                mButton.SetActive(mSliderToggle.ToggleValue);
                mButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cog"));
                mButton.OnClick.Add(() => { SaveParam(); Trigger("Test"); Buddy.GUI.Toaster.Hide(); });

                mSliderToggle.OnToggle.Add((iValue) => { mButton.SetActive(iValue); });
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { SaveParam(); Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("save")
            );
        }

        private void SaveParam()
        {
            GuardianData.Instance.SoundDetection = mSliderToggle.ToggleValue;
            GuardianData.Instance.SoundDetectionThreshold = (int)mSliderToggle.SlidingValue;
        }

    }
}