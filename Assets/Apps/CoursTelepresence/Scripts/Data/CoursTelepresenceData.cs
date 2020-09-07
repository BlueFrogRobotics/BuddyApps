using BlueQuark;
using System.Collections.Generic;

namespace BuddyApp.CoursTelepresence
{
    public class CoursTelepresenceData : AAppData
    {
        public string Ping { get; set; }

        public ConnectivityProblem ConnectivityProblem { get; set; }

        public bool InitializeDone { get; set; } 

        public bool IsQualityNetworkGood { get; set; }

        public List<string> AllPlanning { get; set; }

        public enum States
        {
            CALLING_STATE,
            CALL_STATE,
            CONNECTING_STATE,
            IDLE_STATE,
            INCOMMING_CALL_STATE,
            PARAMETER_STATE
        }

        public States CurrentState { get; set; }

        public static CoursTelepresenceData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CoursTelepresenceData>();
                return sInstance as CoursTelepresenceData;
            }
        }
    }
}
