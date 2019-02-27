using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    /*
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
        // Mail subject
        private const string mMailSubject = "Semi Automated Test report";
        // Default mail
        private string mFileLogDest = "abara@bluefrogrobotics.com";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            string lFileLog = null;

            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("testlog"));

            // FileLog Header - replace all field by data
            lFileLog = mHeaderFileLog.Replace("date", DateTime.Today.ToShortDateString())
                                                            .Replace("time", DateTime.Today.ToShortTimeString())
                                                            .Replace("robotUid", Buddy.Platform.RobotUID)
                                                            .Replace("bre", Buddy.Platform.RobotSoftwareVersion);

            Buddy.GUI.Toaster.Display<VerticalListToast>().With((iBuilder) =>
            {
                // Display result for each Modules
                for (AutomatedTestData.MODULES lEntry = 0; lEntry < AutomatedTestData.MODULES.E_NB_MODULE; lEntry++)
                {
                    // Skip the module if it's result pool is empty
                    if (AutomatedTestData.Instance.Modules.ContainsKey(lEntry) == false)
                        continue;
                    AModuleTest lModule = AutomatedTestData.Instance.Modules[lEntry];
                    Dictionary<string, bool> lModuleResult = lModule.GetResult();
                    if (lModuleResult == null || (lModuleResult != null && lModuleResult.Count == 0))
                        continue;

                    // Display the name of the module
                    TVerticalListBox lModuleName = iBuilder.CreateBox();
                    lModuleName.SetLabel(lModule.Name);
                    lModuleName.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_list"));
                    lModuleName.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));

                    // Add the module name to the FileLog
                    lFileLog += Environment.NewLine + mModuleNameTemplate.Replace("moduleName", lModule.Name) + Environment.NewLine;

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
                            lFileLog += mTestNameTemplate.Replace("testName", Buddy.Resources.GetString(lResult.Key))
                                                            .Replace("testResult", "OK") + Environment.NewLine;
                        }
                        else
                        {
                            // TEST KO - Name of the test + feedback
                            lBox.SetLabel(Buddy.Resources.GetString(lResult.Key) + ": " + lModule.GetErrorByTest(lResult.Key));
                            lBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_close"));
                            lBox.LeftButton.SetBackgroundColor(Color.red);
                            // Add the test name & result to the FileLog
                            lFileLog += mTestNameTemplate.Replace("testName", Buddy.Resources.GetString(lResult.Key))
                                                            .Replace("testResult", "KO")
                                                            .Replace("NoFeedback", lModule.GetErrorByTest(lResult.Key)) + Environment.NewLine;
                        }
                    }
                }

                //  Then user can send log to an email or go back to menu
                TVerticalListBox lEndBox = iBuilder.CreateBox();
                lEndBox.SetLabel(Buddy.Resources.GetString("sendlogorback"));
                lEndBox.SetCenteredLabel(true);
                lEndBox.OnClick.Add(() => { Trigger("MenuTrigger"); });
                TRightSideButton lRightEnd = lEndBox.CreateRightButton();
                lRightEnd.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_arrow_right"));
                lRightEnd.OnClick.Add(() => { Trigger("MenuTrigger"); });

                // Send mail button
                lEndBox.LeftButton.SetBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1F));
                lEndBox.LeftButton.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
                lEndBox.LeftButton.OnClick.Add(() => SendFileLog(lFileLog));
            });
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Clear all selected test before going back to the menu.
            for (AutomatedTestData.MODULES lModule = 0; lModule < AutomatedTestData.MODULES.E_NB_MODULE; lModule++)
            {
                if (AutomatedTestData.Instance.Modules.ContainsKey(lModule))
                {
                    AutomatedTestData.Instance.Modules[lModule].ClearSelectedTest();
                    AutomatedTestData.Instance.Modules[lModule].ClearError();
                }
            }
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
        }

        // Show Send mail UI - User can provide an email
        private void SendFileLog(string iFileLog)
        {
            Debug.LogWarning(iFileLog);
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
            {
                // Email field
                TTextField lMailField = iBuilder.CreateWidget<TTextField>();
                lMailField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
                lMailField.SetPlaceHolder("Your email");
                lMailField.SetText(mFileLogDest);
                lMailField.OnChangeValue.Add((iTextChanges) => { mFileLogDest = iTextChanges; });
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
                Debug.Log("Click send");
                SendEmail(mFileLogDest, mMailSubject, iFileLog);
                Trigger("MenuTrigger");
            },
            // Right button Name
            Buddy.Resources.GetString("send"));
        }

        private void SendEmail(string iDest, string iSubject, string iBoddy)
        {
            EMail lMail = new EMail(iSubject, iBoddy);
            lMail.Addresses.Clear();
            lMail.Addresses.Add(iDest);
            Buddy.WebServices.EMailSender.Send("notif.buddy@gmail.com", "autruchemagiquebuddy", SMTP.GMAIL, lMail, OnEndSending);
        }

        private void OnEndSending(bool iSuccess)
        {
            if (iSuccess)
                Debug.LogWarning("SendLog to " + mFileLogDest + ": SUCCESS");
            else
                Debug.LogWarning("SendLog to " + mFileLogDest + ": FAIL");
        }
    }
}
