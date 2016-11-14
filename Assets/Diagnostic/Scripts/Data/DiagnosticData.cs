using UnityEngine;
using System.Collections;
using BuddyOS;

namespace BuddyApp.Diagnostic
{
    public class DiagnosticData : AAppData
    {
        public int Lol { get; set; }
        public string Poney { get; set; }

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