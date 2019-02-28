using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class ModuleState : AStateMachineBehaviour
    {
        // This state display all available test for the selected module, and ask user what test he wants to run

        private AModuleTest mModuleTest;
        private List<string> mAvailableTestKeys;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ModuleManager.MODULES lModuleIndex = (ModuleManager.MODULES)GetInteger("ModuleTrigger");
            if (lModuleIndex > ModuleManager.MODULES.E_NB_MODULE)
            {
                Debug.LogError("AutomatedTest : ERROR : Module index out of range.");
                return;
            }
            mModuleTest = ModuleManager.GetInstance().Modules[lModuleIndex];
            mAvailableTestKeys = mModuleTest.GetAvailableTest();

            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle(mModuleTest.Name);

            Buddy.GUI.Toaster.Display<ParameterToast>().With((iBuilder) =>
            {
                List<TToggle> lToggles = new List<TToggle>();

                // Create a button to reverse the selection
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
                    lToggle.ToggleValue = mModuleTest.ContainSelectedTest(lTestKey);
                    lToggle.OnToggle.Add((iToggle) =>
                    {
                        if (iToggle)
                            mModuleTest.AddSelectedTest(lTestKey);
                        else
                            mModuleTest.RemoveSelectedTest(lTestKey);
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
                if (mModuleTest.SelectedTestLength() > 0)
                    Trigger("RunTrigger");
            },
            // Right button Name
            Buddy.Resources.GetString("run"));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetInteger("ModuleTrigger", -1);
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}
