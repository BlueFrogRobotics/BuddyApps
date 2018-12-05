using BlueQuark;

using UnityEngine;

using System.Collections.Generic;

namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the sound sensibility
    /// </summary>
    public sealed class SoundDetectionParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        private TSliderToggle mSliderToggle;
        private TButton mButton;

        private float mTimer = 0.0F;
        private bool mToasterVisible;


        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            mToasterVisible = false;
            mTimer = 0.0F;
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
            
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                mSliderToggle = iBuilder.CreateWidget<TSliderToggle>();
                mSliderToggle.OnSlide.Add((iVal) => Debug.Log(iVal));
                mSliderToggle.ToggleValue = GuardianData.Instance.SoundDetection;
                mSliderToggle.SlidingValue = GuardianData.Instance.SoundDetectionThreshold;
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