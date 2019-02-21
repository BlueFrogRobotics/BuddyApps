using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class TestLogState : AStateMachineBehaviour
    {
        /*
         *  -- TestLogFile Ideas --
         *  logFile_[date]_[time].txt:
         *  SemiAutomated Test [date] - [time]
         *  [RobotName] - [RobotUID] - [BRE]
         *  
         *  --- [ModuleName] : ---
         *  Test [TestName] :
         *  feedback
         *  feedback
         *  
         *  Test [TestName] :
         *  feedback
         *  feedback
         *  
         *  --- [ModuleName] : ---
         *  Test [TestName] :
         *  feedback
         *  feedback 
         */

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.DisplayLightTitle("Test Log");

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                // Display result for each Modules
                for (AutomatedTestData.MODULES lEntry = 0; lEntry < AutomatedTestData.MODULES.E_NB_MODULE; lEntry++)
                {
                    // Skip the module if it's result pool is empty
                    if (AutomatedTestData.Instance.Modules[lEntry].GetResult() == null)
                        continue;

                    // Display the name of the module
                    TVerticalListBox lModuleName = iBuilder.CreateBox();
                    lModuleName.SetLabel(AutomatedTestData.Instance.Modules[lEntry].Name);
                    lModuleName.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_list"));
                    lModuleName.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));

                    // Then display all test result for this module
                    foreach (KeyValuePair<string, bool> lResult in AutomatedTestData.Instance.Modules[lEntry].GetResult())
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        lBox.SetLabel(lResult.Key);
                        lBox.SetCenteredLabel(true);
                        lBox.LeftButton.SetIconColor(Color.white);
                        if (lResult.Value)
                        {
                            lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
                            lBox.LeftButton.SetBackgroundColor(Color.green);
                        }
                        else
                        {
                            lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_close"));
                            lBox.LeftButton.SetBackgroundColor(Color.red);
                        }
                    }
                }
                //  Then user can send log to an email or go back to menu
                TVerticalListBox lEndBox = iBuilder.CreateBox();
                lEndBox.SetLabel("SenLog or Back to menu");
                lEndBox.SetCenteredLabel(true);
                lEndBox.OnClick.Add(() => { Trigger("MenuTrigger"); });
                TRightSideButton lRightEnd = lEndBox.CreateRightButton();
                lRightEnd.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_right"));
                lRightEnd.OnClick.Add(() => { Trigger("MenuTrigger"); });

                // Send mail button
                lEndBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                lEndBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
                lEndBox.LeftButton.OnClick.Add(() => { Debug.LogWarning("Send log not implemented yet"); });
            });
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Clear all selected test before going back to the menu.
            for (AutomatedTestData.MODULES lModule = 0; lModule < AutomatedTestData.MODULES.E_NB_MODULE; lModule++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(lModule))
                    AutomatedTestData.Instance.Modules[lModule].ClearSelectedTest();
            }
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }
    }
}
