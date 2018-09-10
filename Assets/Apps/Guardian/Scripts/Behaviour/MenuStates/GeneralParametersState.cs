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
    public class GeneralParametersState : AStateMachineBehaviour
    {
        //private GuardianLayout mDetectionLayout;
        private bool mHasSwitchState = false;

        private TToggle mToggleMobileGuard;
        private TToggle mToggleMobileHead;
        private TToggle mToggleMailNotif;


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
                //iBuilder.CreateWidget<TText>().SetLabel("setup sound detection");
                //iBuilder.CreateWidget<TText>().SetLabel("test");
                mToggleMobileGuard = iBuilder.CreateWidget<TToggle>();
                mToggleMobileGuard.SetLabel("mobile guard");
                mToggleMobileGuard.ToggleValue = GuardianData.Instance.MobileDetection;

                mToggleMobileHead = iBuilder.CreateWidget<TToggle>();
                mToggleMobileHead.SetLabel("mobile head");
                mToggleMobileHead.ToggleValue = GuardianData.Instance.ScanDetection;

                mToggleMailNotif = iBuilder.CreateWidget<TToggle>();
                mToggleMailNotif.SetLabel("mail notification");
                mToggleMailNotif.ToggleValue = GuardianData.Instance.SendMail;

                TButton lButtonRecipient = iBuilder.CreateWidget<TButton>();
                lButtonRecipient.SetLabel("who to contact");
                lButtonRecipient.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_sort_down"));
                lButtonRecipient.OnClick.Add(() => { Trigger("RecipientChoice"); Buddy.GUI.Toaster.Hide(); });
                //iBuilder.CreateWidget<TText>().SetLabel("test2");
            },
            () => { Trigger("Parameter"); Buddy.GUI.Toaster.Hide(); }, "Cancel",
            () => { SaveValues(); Trigger("Parameter");  Buddy.GUI.Toaster.Hide(); }, "Next"
            );


        }

        public override void OnStateUpdate(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        public override void OnStateExit(Animator iAnimator, AnimatorStateInfo iStateInfo, int iLayerIndex)
        {

        }

        private void SaveValues()
        {
            GuardianData.Instance.MobileDetection = mToggleMobileGuard.ToggleValue;
            GuardianData.Instance.ScanDetection = mToggleMobileHead.ToggleValue;
            GuardianData.Instance.SendMail = mToggleMailNotif.ToggleValue;
        }

    }
}