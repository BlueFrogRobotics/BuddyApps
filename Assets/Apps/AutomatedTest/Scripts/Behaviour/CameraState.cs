using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class CameraState :  AStateMachineBehaviour
    {
        // This state display all available test for the camera module, and ask user what test he wants to run

        private AModuleTest mCameraTest;
        private List<string> mAvailableTestKeys;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mCameraTest = AutomatedTestData.Instance.Modules[AutomatedTestData.MODULES.E_CAMERA];
            mAvailableTestKeys = mCameraTest.GetAvailableTest();

            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle("Camera Test");

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
                // Create a toggle button for each available test
                foreach (string lTestKey in mAvailableTestKeys)
                {
                    TToggle lToggle = iBuilder.CreateWidget<TToggle>();
                    lToggle.SetLabel(lTestKey);
                    lToggle.ToggleValue = false;
                    lToggle.OnToggle.Add((iToggle) => 
                    {
                        if (iToggle)
                            mCameraTest.mSelectedTests.Add(lTestKey);
                        else
                            mCameraTest.mSelectedTests.Remove(lTestKey);
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
            "Cancel",
            // Right Button Callback
            () =>
            {
                 Debug.Log("Click next");
                if (mCameraTest.mSelectedTests.Count > 0)
                     Trigger("RunTrigger");
            },
            // Right button Name
            "Next");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

