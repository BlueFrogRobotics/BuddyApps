using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class MotionsState : AStateMachineBehaviour
    {
        // This state display all available test for the motion module, and ask user what test he wants to run

        private AModuleTest mMotionTest;
        private List<string> mAvailableTestKeys;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mMotionTest = AutomatedTestData.Instance.Modules[AutomatedTestData.MODULES.E_MOTION];
            mAvailableTestKeys = mMotionTest.GetAvailableTest();

            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle(mMotionTest.Name);

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                List<TToggle> lToggles = new List<TToggle>();

                TButton lSelectAll = iBuilder.CreateWidget<TButton>();
                lSelectAll.SetLabel(Buddy.Resources.GetString("reverseselection"));
                lSelectAll.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_retweet"));
                lSelectAll.OnClick.Add(() =>
                {
                    foreach (TToggle lToggle in lToggles)
                        lToggle.ToggleValue = !lToggle.ToggleValue;
                });

                // Create a toggle button for each available test
                foreach (string lTestKey in mAvailableTestKeys)
                {
                    TToggle lToggle = iBuilder.CreateWidget<TToggle>();
                    lToggle.SetLabel(Buddy.Resources.GetString(lTestKey));
                    lToggle.ToggleValue = mMotionTest.ContainSelectedTest(lTestKey);
                    lToggle.OnToggle.Add((iToggle) =>
                    {
                        if (iToggle)
                            mMotionTest.AddSelectedTest(lTestKey);
                        else
                            mMotionTest.RemoveSelectedTest(lTestKey);
                    });
                    lToggles.Add(lToggle);
                }
            },
            // Left button Callback
            () =>
            {
                Debug.Log("Click cancel");
                Trigger("MenuTrigger");
            },
            // Left button Name
            Buddy.Resources.GetString("back"),
            // Right Button Callback
            () =>
            {
                Debug.Log("Click next");
                if (mMotionTest.SelectedTestLength() > 0)
                    Trigger("RunTrigger");
            },
            // Right button Name
            Buddy.Resources.GetString("run"));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}