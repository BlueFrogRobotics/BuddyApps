using UnityEngine;
using System.Collections;
using BuddyOS.App;

namespace BuddyApp.Diagnostic
{
    public class DiagnosticData : AAppData
    {
        public static DiagnosticData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<DiagnosticData>();
                return sInstance as DiagnosticData;
            }
        }
    }
}