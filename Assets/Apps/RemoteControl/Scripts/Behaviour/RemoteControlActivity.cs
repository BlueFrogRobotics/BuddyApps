using BlueQuark;

using UnityEngine.UI;

using UnityEngine;

using System;

using System.Collections;

namespace BuddyApp.RemoteControl
{
    /* This class contains useful callback during your app process */
    public class RemoteControlActivity : AAppActivity
    {

        public override void OnLoading(object[] iArgs)
        {
            return;
            if (iArgs != null && iArgs.Length > 0)
                if ((int)iArgs[0] == 1)
                    RemoteControlData.Instance.IsWizardOfOz = true;
                else
                    RemoteControlData.Instance.IsWizardOfOz = false;
        }

        public override void OnQuit()
        {
            Debug.LogError("------------- INSIDE ON QUIT ----------------");
            Debug.Log("----- TOASTER HIDE ----");
            Buddy.GUI.Toaster.Hide();
            Debug.LogError("----- INST:" + RemoteControlData.Instance);
//            StartCoroutine(CloseApp());
        }

        public IEnumerator CloseApp()
        {
            Debug.LogError("------------- INSIDE CLOSE APP CO ROUTINE ----------------");
            yield return new WaitUntil(() => {
                return RemoteControlData.Instance.CustomToastIsBusy;
                // This code was a try to avoid black square on quit
                // I don't delete it, to get your feedback, and to show what i have already test
                //Debug.Log("----- WAITING ... ----");
                //if (Buddy.GUI.Toaster.IsBusy || RemoteControlData.Instance.CustomToastIsBusy)
                //{
                //    Debug.Log("----- TOASTER: " + Buddy.GUI.Toaster.IsBusy  + "CUSTOM: " + RemoteControlData.Instance.CustomToastIsBusy + " ----");
                //    return true;
                //}
                //Debug.Log("----- ! QUIT ! ----");
                //return false;
            });
        }
    }
}