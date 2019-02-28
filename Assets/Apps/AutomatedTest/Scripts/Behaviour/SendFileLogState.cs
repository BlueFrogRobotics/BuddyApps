using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public class SendFileLogState : AStateMachineBehaviour
    {
        // Mail subject
        private const string mMailSubject = "Semi Automated Test report";
        // Default mail
        private string mFileLogDest = "studio@bluefrogrobotics.com";

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.LogWarning(ModuleManager.GetInstance().FileLog);
            Buddy.GUI.Header.DisplayLightTitle(Buddy.Resources.GetString("sendlog"));
            Buddy.GUI.Toaster.Display<ParameterToast>().With(iBuilder =>
            {
                // Email field
                TTextField lMailField = iBuilder.CreateWidget<TTextField>();
                lMailField.SetIcon(Buddy.Resources.Get<Sprite>("os_icon_mail"));
                lMailField.SetPlaceHolder(Buddy.Resources.GetString("mailfield"));
                lMailField.SetText(mFileLogDest);
                lMailField.OnChangeValue.Add((iTextChanges) => { mFileLogDest = iTextChanges; });
            },
            // Left button Callback
            () =>
            {
                Debug.Log("Click cancel");
                Trigger("TestLogTrigger");
            },
            // Left button Name
            Buddy.Resources.GetString("cancel"),
            // Right Button Callback
            () =>
            {
                if (string.IsNullOrEmpty(mFileLogDest))
                    return;
                Debug.Log("Click send");
                SendEmail(mFileLogDest, mMailSubject, ModuleManager.GetInstance().FileLog);
                // Clear all selected test before going back to the menu.
                ModuleManager.GetInstance().ClearModulesData();
                Trigger("MenuTrigger");
            },
            // Right button Name
            Buddy.Resources.GetString("send"));
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Buddy.GUI.Header.HideTitle();
            Buddy.GUI.Toaster.Hide();
            Buddy.GUI.Footer.Hide();
        }

        private void SendEmail(string iDest, string iSubject, string iBoddy)
        {
            EMail lMail = new EMail(iSubject, iBoddy);
            lMail.Addresses.Clear();
            lMail.Addresses.Add(iDest);
            Buddy.WebServices.EMailSender.Send("demo@buddytherobot.com", "DemoBuddy", SMTP.GMAIL, lMail, OnEndSending);
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
