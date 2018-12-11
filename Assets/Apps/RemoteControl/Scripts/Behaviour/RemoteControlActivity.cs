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
            Debug.Log("------------- INSIDE ON QUIT ----------------");
            Debug.Log("----- TOASTER HIDE ----");
            Buddy.GUI.Toaster.Hide();
        }
    }
}