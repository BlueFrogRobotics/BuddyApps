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

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                // Create a toggle button for each available test
                foreach (string lTestKey in mAvailableTestKeys)
                {
                    TToggle lToggle = iBuilder.CreateWidget<TToggle>();
                    lToggle.SetLabel(lTestKey);
                    lToggle.ToggleValue = mMotionTest.GetSelectedKey().Contains(lTestKey);
                    lToggle.OnToggle.Add((iToggle) =>
                    {
                        if (iToggle)
                            mMotionTest.GetSelectedKey().Add(lTestKey);
                        else
                            mMotionTest.GetSelectedKey().Remove(lTestKey) ;
                    });
                }
            },
            // Left button Callback
            () =>
            {
                Debug.Log("Click cancel");
                Trigger("MenuTrigger");
            },
            // Left button Name
            "Back",
            // Right Button Callback
            () =>
            {
                Debug.Log("Click next");
                if (mMotionTest.GetSelectedKey().Count > 0)
                    Trigger("RunTrigger");
            },
            // Right button Name
            "Run");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}