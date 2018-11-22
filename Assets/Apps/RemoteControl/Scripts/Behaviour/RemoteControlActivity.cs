using UnityEngine.UI;
using UnityEngine;

using BlueQuark;
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

        public override void OnStart()
        {
        }

        public override void OnQuit()
        {
            Debug.Log("----- TOASTER HIDE ----");
            Buddy.GUI.Toaster.Hide();
            Debug.Log("----- TOASTER HIDE WAS CALLED ----");
            StartCoroutine(CloseApp());
            Debug.Log("----- END ON QUIT ----");
        }

        public IEnumerator CloseApp()
        {
            Debug.Log("------ DEBUG: " + RemoteControlData.Instance.CustomToastIsBusy + " --------");
            Debug.Log("----- WAITING ... ----");
            yield return new WaitForSeconds(5f);
            Debug.Log("----- WAITING 2 ... ----");
            yield return new WaitUntil(() => RemoteControlData.Instance.CustomToastIsBusy);
            Debug.Log("----- END CLOSE APP ... ----");
        }
    }
}