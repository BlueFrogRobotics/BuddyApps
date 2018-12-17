using BlueQuark;

using UnityEngine;


namespace BuddyApp.Guardian
{
    /// <summary>
    /// State where the user can set the movement detection sensibility
    /// </summary>
    public sealed class MovementDetectionParametersState : AStateMachineBehaviour
    {
        private bool mHasSwitchState = false;

        private TSliderToggle mSliderToggle;
        private TButton mButtonMotionTest;
        //private TToggle mToggleKidnapping;


        public override void Start()
        {
            mHasSwitchState = false;
        }

        public override void OnStateEnter(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("motiondetection"));
            
            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                mSliderToggle = iBuilder.CreateWidget<TSliderToggle>();
                mSliderToggle.OnSlide.Add((iVal) => Debug.Log(iVal));
                mSliderToggle.ToggleValue = GuardianData.Instance.MovementDetection;
                mSliderToggle.SlidingValue = GuardianData.Instance.MovementDetectionThreshold;
                mSliderToggle.OnToggle.Add(OnToggleMotionDetection);
                
                mButtonMotionTest = iBuilder.CreateWidget<TButton>();
                mButtonMotionTest.SetLabel(Buddy.Resources.GetString("setmotionsensitivity"));
                mButtonMotionTest.SetActive(mSliderToggle.ToggleValue);
                mButtonMotionTest.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_cog"));
                mButtonMotionTest.OnClick.Add(() => { SaveParam(); Trigger("Test"); Buddy.GUI.Toaster.Hide(); });

                //mToggleKidnapping = iBuilder.CreateWidget<TToggle>();
                //mToggleKidnapping.OnToggle.Add((iVal) => Debug.Log(iVal));
                //mToggleKidnapping.SetLabel(Buddy.Resources.GetString("kidnappingdetection"));
                //mToggleKidnapping.ToggleValue = GuardianData.Instance.KidnappingDetection;
                
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
            //GuardianData.Instance.KidnappingDetection = mToggleKidnapping.ToggleValue;
        }

        private void OnToggleMotionDetection(bool iValue)
        {
            mButtonMotionTest.SetActive(iValue);
        }

    }
}