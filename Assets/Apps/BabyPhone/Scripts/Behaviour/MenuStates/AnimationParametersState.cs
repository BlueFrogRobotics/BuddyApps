using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.BabyPhone
{

    public sealed class AnimationParametersState : AStateMachineBehaviour
    {
        private TSlider mDurationSlider;
        private TToggle mActivateToggle;
        private TToggle mReplayToggle;

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("lullaby"));

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                mActivateToggle = iBuilder.CreateWidget<TToggle>();
                mActivateToggle.SetLabel(Buddy.Resources.GetString("activatelullaby"));
                mActivateToggle.ToggleValue = BabyPhoneData.Instance.PlayLullaby;

                mDurationSlider = iBuilder.CreateWidget<TSlider>();
                mDurationSlider.MaxSlidingValue = 10;
                mDurationSlider.SlidingValue = BabyPhoneData.Instance.LullabyDuration;

                mReplayToggle = iBuilder.CreateWidget<TToggle>();
                mReplayToggle.SetLabel(Buddy.Resources.GetString("replayonawake"));
                mReplayToggle.ToggleValue = BabyPhoneData.Instance.ReplayOnDetection;
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("cancel"),
            () => { SaveValues(); Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, Buddy.Resources.GetString("save")
            );


        }

        private void SaveValues()
        {
            BabyPhoneData.Instance.PlayLullaby = mActivateToggle.ToggleValue;
            BabyPhoneData.Instance.LullabyDuration = mDurationSlider.SlidingValue;
            BabyPhoneData.Instance.ReplayOnDetection = mReplayToggle.ToggleValue;
        }
    }
}
