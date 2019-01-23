using BlueQuark;
using UnityEngine;

namespace BuddyApp.RemoteControl
{
    /* Data are stored in xml file for persistent data purpose */
    public class RemoteControlData : AAppData
    {
        public enum AvailableRemoteMode { REMOTE_CONTROL, WOZ, TAKE_CONTROL};
        /*
         * Data getters / setters
         */
        public bool DiscreteMode{ get; set; }

        public AvailableRemoteMode RemoteMode { get; set; }

        /*
         * Data singleton access
         */
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
