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
            if (iArgs != null && iArgs.Length > 0)
            {
                switch ((int)iArgs[0])
                {
                    case 0:
                        RemoteControlData.Instance.RemoteMode = RemoteControlData.AvailableRemoteMode.REMOTE_CONTROL;
                        break;
                    case 1:
                        RemoteControlData.Instance.RemoteMode = RemoteControlData.AvailableRemoteMode.WOZ;
                        break;
                    case 2:
                        RemoteControlData.Instance.RemoteMode = RemoteControlData.AvailableRemoteMode.TAKE_CONTROL;
                        break;
                }
            }
        }

        public override void OnQuit()
        {
            Buddy.GUI.Toaster.Hide();
        }
    }
}