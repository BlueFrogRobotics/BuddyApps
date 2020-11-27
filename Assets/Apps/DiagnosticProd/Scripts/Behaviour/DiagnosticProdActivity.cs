using BlueQuark;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

namespace BuddyApp.DiagnosticProd
{
    /* This class contains useful callback during your app process */
    public class DiagnosticProdActivity : AAppActivity
    {
        public override void OnLoading(object[] iInputArgs)
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.LOADING, "On loading...");
        }

        public override void OnStart()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.SUCCESS, LogInfo.LOADING, "On start...");

            Buddy.GUI.Header.DisplayParametersButton(false);
        }

        public override void OnQuit()
        {
            ExtLog.I(ExtLogModule.APP, GetType(), LogStatus.START, LogInfo.UNLOADING, "On quit...");
        }
    }
}