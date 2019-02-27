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
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("fulltest"));

            for (AutomatedTestData.MODULES lModule = 0; lModule < AutomatedTestData.MODULES.E_NB_MODULE; lModule++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(lModule))
                    AutomatedTestData.Instance.Modules[lModule].SelectAllTest();
            }

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) => {
            TText lText = iBuilder.CreateWidget<TText>();
            lText.SetCenteredLabel(true);
            lText.SetLabel(Buddy.Resources.GetString("alltest"));
            },
            // Left button Callback
            () =>
            {
                Debug.Log("Click cancel");
                Trigger("MenuTrigger");
            },
            // Left button Name
            Buddy.Resources.GetString("cancel"),
            // Right Button Callback
            () =>
            {
                Debug.Log("Click next");
                Trigger("RunTrigger");
            },
            // Right button Name
            Buddy.Resources.GetString("next"));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

