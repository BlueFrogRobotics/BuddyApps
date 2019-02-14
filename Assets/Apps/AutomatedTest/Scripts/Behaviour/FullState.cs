using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public class FullState : AStateMachineBehaviour
    {
        // Select all available test on each available module, and ask the user if we can launch all test.

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle("Full Test");

            for (AutomatedTestData.MODULES i = 0; i < AutomatedTestData.MODULES.E_NB_MODULE; i++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(i))
                    AutomatedTestData.Instance.Modules[i].SelectAllTest();
            }

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
            TText lText = iBuilder.CreateWidget<TText>();
            lText.SetCenteredLabel(true);
            lText.SetLabel("All available test will run. Click 'Next' to continue");
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

