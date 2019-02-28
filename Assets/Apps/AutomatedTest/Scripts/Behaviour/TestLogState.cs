using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    /*
     * This state build the file log, and display all results.
     * 
     *  TestLogFile Template:
     *  
     *  logFile_[date]_[time].txt:
     *  
     *  SemiAutomated Test [date] - [time]
     *  Robot Info - [RobotUID] - [BRE]  
     *   
     *  --- [ModuleName] : ---
     *  Test [TestName] :    
     *  feedback
     *  
     *  Test [TestName] :
     *  feedback
     *  
     *  --- [ModuleName] : ---
     *  Test [TestName] :
     *  feedback
     */

    public sealed class TestLogState : AStateMachineBehaviour
    {
        // The file always start with the header file log.
        private readonly string mHeaderFileLog = "Semi Automated Test [date] - [time]" + Environment.NewLine + "Robot Info: [UID:robotUid] - [bre]" + Environment.NewLine;
        // Template for the module name field.
        private const string mModuleNameTemplate = "--- [moduleName] : ---";
        // Template for the test name field.
        private const string mTestNameTemplate = "[testName] : [testResult] [NoFeedback]";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ModuleManager.GetInstance().FileLog = null;

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("testlog"));

            // FileLog Header - replace all field by data
            ModuleManager.GetInstance().FileLog = mHeaderFileLog.Replace("date", DateTime.Now.ToShortDateString())
                                                            .Replace("time", DateTime.Now.ToShortTimeString())
                                                            .Replace("robotUid", Buddy.Platform.RobotUID)
                                                            .Replace("bre", Buddy.Platform.RobotSoftwareVersion);

            // Send file log button - don't clear result / error / filelog
            FButton lSendButton = Buddy.GUI.Footer.CreateOnLeft<FButton>();
            lSendButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
            lSendButton.OnClick.Add(() => { Trigger("SendFileLogTrigger"); });

            // Repeat test suite button
            FButton lRepeatButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lRepeatButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_reload"));
            lRepeatButton.OnClick.Add(() =>
            {
                // Clear all data except selectedTest before going back to the menu.
                ModuleManager.GetInstance().ClearModulesData(false);
                Trigger("RunTrigger");
            });

            // Back to menu button - clear result / error / filelog
            FButton lMenuButton = Buddy.GUI.Footer.CreateOnRight<FButton>();
            lMenuButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_right"));
            lMenuButton.OnClick.Add(() =>
            {
                // Clear all data before going back to the menu.
                ModuleManager.GetInstance().ClearModulesData();
                Trigger("MenuTrigger");
            });

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                // Display result for each Modules
                ModuleManager.GetInstance().ForeachModulesDo((iModule) =>
                {
                    Dictionary<string, bool> lModuleResult = iModule.Value.GetResult();
                    if (lModuleResult == null || (lModuleResult != null && lModuleResult.Count == 0))
                        return;

                    // Display the name of the module
                    TVerticalListBox lModuleName = iBuilder.CreateBox();
                    lModuleName.SetLabel(iModule.Value.Name);
                    lModuleName.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_list"));
                    lModuleName.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));

                    // Add the module name to the FileLog
                    ModuleManager.GetInstance().FileLog += Environment.NewLine + mModuleNameTemplate.Replace("moduleName", iModule.Value.Name) + Environment.NewLine;

                    // Then display all test result for this module
                    foreach (KeyValuePair<string, bool> lResult in lModuleResult)
                    {
                        TVerticalListBox lBox = iBuilder.CreateBox();
                        lBox.LeftButton.SetIconColor(Color.white);
                        // Status of the test
                        if (lResult.Value)
                        {
                            // TEST OK - Name of the test
                            lBox.SetLabel(Buddy.Resources.GetString(lResult.Key));
                            lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_check"));
                            lBox.LeftButton.SetBackgroundColor(Color.green);
                            // Add the test name & result to the FileLog
                            ModuleManager.GetInstance().FileLog += mTestNameTemplate.Replace("testName", Buddy.Resources.GetString(lResult.Key))
                                                            .Replace("testResult", "OK") + Environment.NewLine;
                        }
                        else
                        {
                            // TEST KO - Name of the test + feedback
                            lBox.SetLabel(Buddy.Resources.GetString(lResult.Key) + ": " + iModule.Value.GetErrorByTest(lResult.Key));
                            lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_close"));
                            lBox.LeftButton.SetBackgroundColor(Color.red);
                            // Add the test name & result to the FileLog
                            ModuleManager.GetInstance().FileLog += mTestNameTemplate.Replace("testName", Buddy.Resources.GetString(lResult.Key))
                                                            .Replace("testResult", "KO")
                                                            .Replace("NoFeedback", iModule.Value.GetErrorByTest(lResult.Key)) + Environment.NewLine;
                        }
                    }
                });
                Debug.LogWarning("-- END TEST LOG UI--");
            });
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
        }
    }
}
