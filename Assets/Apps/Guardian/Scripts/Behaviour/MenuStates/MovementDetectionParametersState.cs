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
    public class MovementDetectionParametersState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private Dictionary<string, string> mButtonContent = new Dictionary<string, string>();

        /// <summary>
        /// Enum of the different sub parameters windows
        /// </summary>
        private enum ParameterWindow : int
        {
            HEAD_ORIENTATION = 0,
            MOVEMENT = 1,
            SOUND = 2,
            FIRE = 3
        }

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
                iBuilder.CreateWidget<TText>().SetLabel("setup movement detection");
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                iBuilder.CreateWidget<TSliderToggle>().OnSlide.Add((iVal) => Debug.Log(iVal));
                //iBuilder.CreateWidget<TToggle>();
                iBuilder.CreateWidget<TButton>().SetLabel("Test Sensibility");
                TToggle lToggleKidnapping = iBuilder.CreateWidget<TToggle>();
                lToggleKidnapping.OnToggle.Add((iVal) => Debug.Log(iVal));
                lToggleKidnapping.SetLabel("kidnapping detection");
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, "Cancel",
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, "Next"
            );


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {
           
        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

    }
}