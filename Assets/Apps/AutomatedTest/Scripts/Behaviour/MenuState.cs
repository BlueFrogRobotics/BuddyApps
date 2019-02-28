using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class MenuState : AStateMachineBehaviour
    {
        //  Display all available test module

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayParametersButton(false);
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("automatedtest"));

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                // Display all available module for testing
                ModuleManager.GetInstance().ForeachModulesDo((iModule) =>
                {
                    TVerticalListBox lBox = iBuilder.CreateBox();
                    lBox.SetLabel(iModule.Value.Name);
                    lBox.LeftButton.Hide();
                    lBox.SetCenteredLabel(true);
                    lBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                    lBox.OnClick.Add(() =>
                    {
                        Debug.Log("Click ModuleTrigger: " + (int)iModule.Key);
                        SetInteger("ModuleTrigger", (int)iModule.Key);
                    });
                });

                // Full test
                TVerticalListBox lFullTestBox = iBuilder.CreateBox();
                lFullTestBox.SetLabel(Buddy.Resources.GetString("fulltest"));
                lFullTestBox.LeftButton.Hide();
                lFullTestBox.SetCenteredLabel(true);
                lFullTestBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                lFullTestBox.OnClick.Add(() => { Debug.Log("Click fulltest"); Trigger("FullTrigger"); });
            });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}

