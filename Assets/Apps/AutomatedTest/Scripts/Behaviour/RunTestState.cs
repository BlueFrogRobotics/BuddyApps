using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;
using System;

namespace BuddyApp.AutomatedTest
{
    public sealed class RunTestState : AStateMachineBehaviour
    {
        private bool mTestLaunched;

        // Launch all selected test here, maybe an entire state is overkill for that

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mTestLaunched = false;
            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("selectmode"));

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                // Manual mode explication
                TText lManualModeText = iBuilder.CreateWidget<TText>();
                lManualModeText.SetCenteredLabel(true);
                lManualModeText.SetLabel(Buddy.Resources.GetString("manualmode"));

                // Automatic mode explication
                TText lAutoModeText = iBuilder.CreateWidget<TText>();
                lAutoModeText.SetCenteredLabel(true);
                lAutoModeText.SetLabel(Buddy.Resources.GetString("automode"));
            },
            // Left button Callback
            () =>
            {
                Debug.Log("Click manual");
                if (mTestLaunched == false)
                {
                    ModuleManager.GetInstance().ForeachModulesDo((iModule) =>
                    {
                        iModule.Value.Mode = AModuleTest.TestMode.M_MANUAL;
                    });
                    StartCoroutine(RunTest());
                    mTestLaunched = true;
                }
            },
            // Left button Name
            Buddy.Resources.GetString("manual"),
            // Right Button Callback
            () =>
            {
                Debug.Log("Click automatic");
                if (mTestLaunched == false)
                {
                    ModuleManager.GetInstance().ForeachModulesDo((iModule) =>
                    {
                        iModule.Value.Mode = AModuleTest.TestMode.M_AUTO;
                    });
                    StartCoroutine(RunTest());
                    mTestLaunched = true;
                }
            },
            // Right button Name
            Buddy.Resources.GetString("auto"));
        }

        private IEnumerator RunTest()
        {
            // Hide precedent UI
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();

            // Run each test of each module
            foreach (KeyValuePair<ModuleManager.MODULES, AModuleTest> lModule in ModuleManager.GetInstance().Modules)
            {
                Debug.LogWarning("-- RUN SELECTED TEST OF:" + lModule.Key.ToString() + " --");
                yield return lModule.Value.RunSelectedTest();
            }
            Trigger("TestLogTrigger");
            Debug.LogWarning("-- END RUN TEST --");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}