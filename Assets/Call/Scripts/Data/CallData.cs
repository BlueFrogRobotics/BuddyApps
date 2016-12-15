using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Call
{
    public class CallData : AAppData
    {
        public bool IsActive { get; set; }
        
        public static CallData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<CallData>();
                return sInstance as CallData;
            }
        }
    }
}
