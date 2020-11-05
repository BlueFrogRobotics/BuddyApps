using BlueQuark;
using System.Collections.Generic;

namespace BuddyApp.TeleBuddyQuatreDeux
{
    public class TeleBuddyQuatreDeuxData : AAppData
    {
        public string Ping { get; set; }

        public ConnectivityProblem ConnectivityProblem { get; set; }

        public bool InitializeDone { get; set; } 

        public bool IsQualityNetworkGood { get; set; }

        public List<string> AllPlanning { get; set; }

        public int IndexTablet { get; set; }

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

        public static TeleBuddyQuatreDeuxData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<TeleBuddyQuatreDeuxData>();
                return sInstance as TeleBuddyQuatreDeuxData;
            }
        }
    }
}
