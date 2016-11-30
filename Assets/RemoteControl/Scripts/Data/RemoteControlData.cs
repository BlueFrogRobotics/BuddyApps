using UnityEngine;
using BuddyOS.App;

namespace BuddyApp.Remote
{
    public class RemoteControlData : AAppData
    {
        public bool IsActive { get; set; }
        
        public static RemoteControlData Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = GetInstance<RemoteControlData>();
                return sInstance as RemoteControlData;
            }
        }
    }
}
